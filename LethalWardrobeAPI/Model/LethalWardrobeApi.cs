using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using LethalWardrobe.Model.Suit;
using LethalWardrobeAPI.Model.Suit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalWardrobeAPI.Model;

public class LethalWardrobeApi
{
    /// <summary>
    /// Singleton pattern call via a lazy implementation of the manager.
    /// </summary>
    private static readonly Lazy<LethalWardrobeApi> instance = new Lazy<LethalWardrobeApi>(() => new LethalWardrobeApi());

    
    /// <summary>
    /// Gets the singleton instance of the manager.
    /// </summary>
    public static LethalWardrobeApi Instance => instance.Value;

    public void Initialize(BaseUnityPlugin plugin)
    {
        IApiLoader apiLoader = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive
                .Include,FindObjectsSortMode.None)
            .OfType<IApiLoader>()
            .ToArray()[0];
        _suitManager = apiLoader.SuitManager;
        Debug.Log($"Initialized Lethal Wardrobe API");
    }
    private ISuitManager _suitManager;

    public ISuit? GetSuit(ulong id) => _suitManager.GetSuit(id);
    
    public List<ISuit> GetSuits() => _suitManager.GetSuits();
    
}