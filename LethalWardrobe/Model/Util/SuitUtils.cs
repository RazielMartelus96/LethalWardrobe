using System;
using System.Data;
using System.Linq;
using UnityEngine;

namespace LethalWardrobe.Model.Util;

public class SuitUtils
{
    private static UnlockableItem _originalSuit;
    public static UnlockableItem GetOriginalSuit(ref StartOfRound instance)
    {
        if (_originalSuit == null)
        {
            _originalSuit = instance.unlockablesList.unlockables
                .FirstOrDefault(item => item.suitMaterial != null && item.alreadyUnlocked);

            if (_originalSuit == null)
            {
                throw new InvalidOperationException("Original suit not found: " +
                                                    "The specified suit does not exist in the main code." +
                                                    " Ensure that all suit references are correctly defined " +
                                                    "and loaded.");
            }
        }
        return _originalSuit;
    }

    public static UnlockableItem InitDummySuit(ref StartOfRound instance)
    {
        UnlockableItem originalSuit = GetOriginalSuit(ref instance);
        return JsonUtility.FromJson<UnlockableItem>(JsonUtility.ToJson(originalSuit));
        
    }

    public static UnlockableItem InitDummySuitForRack(ref StartOfRound instance)
    {
        UnlockableItem dummyRackSuit = InitDummySuit(ref instance);
        dummyRackSuit.alreadyUnlocked = false;
        dummyRackSuit.hasBeenMoved = false;
        dummyRackSuit.placedPosition = Vector3.zero;
        dummyRackSuit.placedRotation = Vector3.zero;
        dummyRackSuit.unlockableType = 753;
        return dummyRackSuit;
    }
}