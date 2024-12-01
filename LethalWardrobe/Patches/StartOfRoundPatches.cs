using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using LethalWardrobe.Model.Config;
using LethalWardrobe.Model.Factories;
using LethalWardrobe.Model.Persistence;
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
        On.StartOfRound.LoadUnlockables += StartOfRoundOnLoadUnlockables;
        On.StartOfRound.PositionSuitsOnRack += StartOfRoundOnPositionSuitsOnRack;
        On.GameNetworkManager.SaveGame += GameNetworkManagerOnSaveGame;
    }

    private static void GameNetworkManagerOnSaveGame(On.GameNetworkManager.orig_SaveGame orig, GameNetworkManager self)
    {
        Debug.Log("LethalWardrobe: ON DISABLE!!!!!!");
        List<ISuit> suits = SuitManager.Instance.GetSuits();
        List<UnlockableItem> unlockables = StartOfRound.Instance.unlockablesList.unlockables;
        List<UnlockableItem> itemsToRemove = [];
        foreach (var unlockable in unlockables)
        {
            if (suits.Any(SuitDataMatchesName))
            {
                itemsToRemove.Add(unlockable);
            }

            bool SuitDataMatchesName(ISuit suit) => suit.UnlockableName == unlockable.unlockableName;
        }
        
        unlockables.RemoveAll(unlockable => itemsToRemove.Contains(unlockable));
        orig(self);    
    }
    


    private static void StartOfRoundOnLoadUnlockables(On.StartOfRound.orig_LoadUnlockables orig, StartOfRound self)
    {
        orig(self);
        var dataList = PersistenceManager.Instance.GetSuitData();
        Debug.Log("Got to checking suit count");
        if (dataList is { Suits.Count: 0 }) 
            return;
        Debug.Log($"Got to for loop after finding out dataList size is: {dataList.Suits.Count} ");

        foreach (var unlockable in self.unlockablesList.unlockables)
        {
            Func<SuitData, bool> suitDataMatchesName = data => data.Name == unlockable.unlockableName;

            if (!dataList.Suits.Any(suitDataMatchesName)) continue;
            Debug.Log("Passed first if statement and found matching names");

            if (ConfigHandler.Instance.GetConfigValue<bool>(ConfigKey.AllSuitsUnlocked))
            {
                unlockable.alreadyUnlocked = true;
                continue;
            }
            Debug.Log("Got past the config handler");

            var matchingSuit = dataList.Suits.FirstOrDefault(suitDataMatchesName);
            Debug.Log("About to cehck unlockable to see if its already unlocked.");

            unlockable.alreadyUnlocked = matchingSuit is { IsUnlocked: true };
        }
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