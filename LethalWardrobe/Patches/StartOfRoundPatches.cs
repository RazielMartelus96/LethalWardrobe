using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using LethalWardrobe.Model.Config;
using LethalWardrobe.Model.Factories;
using LethalWardrobe.Model.Suit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalWardrobe.Patches;

public class StartOfRoundPatches
{
    private static Terminal _terminal;
    private static ISuitFactory _suitFactory;
    public static void Init(Terminal terminal)
    {
    
        _terminal = terminal;
        Debug.Log("LethalWardrobe: StartOfRoundPatches!!!!!!");
        On.StartOfRound.Start += StartOfRoundOnStart;
        On.StartOfRound.PositionSuitsOnRack += StartOfRoundOnPositionSuitsOnRack;
    }
    private static void StartOfRoundOnStart(On.StartOfRound.orig_Start orig, StartOfRound self)
    {
        _suitFactory = InitializeSuitFactory(ref self);
        _suitFactory
            .AddFolderPaths(Directory.
                GetDirectories(Paths.PluginPath, "moresuits", SearchOption.AllDirectories)
                .ToList());
        _suitFactory.AddPatchInstance(self);
        SuitHandler.Instance.RegisterSuits(_suitFactory.Create(),self, _terminal);
        orig(self);
    }
    
    
    private static void StartOfRoundOnPositionSuitsOnRack(On.StartOfRound.orig_PositionSuitsOnRack orig,
        StartOfRound self)
    {
        var suits = LethalWardrobe
            .FindObjectsOfType<UnlockableSuit>()
            .ToList()
            .OrderBy(suit => suit.syncedSuitID.Value)
            .ToList();
        var index = 0;
        foreach (var suit in suits)
        {
            var component = suit.gameObject.GetComponent<AutoParentToShip>();
            component.overrideOffset = true;

            var offsetModifier = 0.18f;
            if (ConfigHandler.Instance.GetConfigValue<bool>(ConfigKey.AutoFitSuitsOnRack) && suits.Count > 13)
                offsetModifier =
                    offsetModifier /
                    (Math.Min(suits.Count, 20) / 12f);

            component.positionOffset = new Vector3(-2.45f, 2.75f, -8.41f) +
                                       self.rightmostSuitPosition.forward * offsetModifier * index;
            component.rotationOffset = new Vector3(0f, 90f, 0f);

            index++;
        }
    }

   

    private static ISuitFactory InitializeSuitFactory(ref StartOfRound startOfRound)
    {
        return startOfRound.gameObject.AddComponent<SuitFactory>();
    }

    
}