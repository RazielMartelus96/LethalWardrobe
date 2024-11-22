using System;
using System.Collections.Generic;
using System.Linq;
using LethalWardrobe.Model.Config;
using LethalWardrobe.Model.Suit.Store;
using LethalWardrobe.Model.Util;
using UnityEngine;

namespace LethalWardrobe.Model.Suit;

public class SuitHandler
{
    /// <summary>
    /// Lazy singleton instance of the Config Handler.
    /// </summary>
    private static readonly Lazy<SuitHandler> LazyInstance = new(() => new SuitHandler());
    public static SuitHandler Instance => LazyInstance.Value;
    private Dictionary<string, ISuit> _suits = new();
    private TerminalNode _cancelPurchase;
    public void RegisterSuits(List<ISuit> suits, StartOfRound instance, Terminal terminal)
    {
        int suitCount = 0;
        suits.ForEach(suit =>
        {
            var originalUnlockablesCount = instance.unlockablesList.unlockables.Count;
            _suits.Add(suit.UnlockableName, suit);
            RegisterSuit(suit, instance);
            if (suit is IPurchasable purchasable)
            {
                HandlePurchasable(purchasable, instance, terminal,suitCount);
            }
            suitCount++;
            var dummySuit = SuitUtils.InitDummySuitForRack(ref instance);
            while (instance.unlockablesList.unlockables.Count < originalUnlockablesCount +
                   ConfigHandler.Instance.GetConfigValue<int>(ConfigKey.MaxSuits))
            {
                instance.unlockablesList.unlockables.Add(dummySuit);

            }
                
        });
        
    }
    
    private void RegisterSuit(ISuit suit, StartOfRound instance)
    {
        UnlockableItem originalSuit = SuitUtils.GetOriginalSuit(ref instance);
        var unlockableSuit = suit.IsDefault ? originalSuit :
            JsonUtility.FromJson<UnlockableItem>(JsonUtility.ToJson(originalSuit));
        unlockableSuit.suitMaterial = suit.SuitMaterial;
        unlockableSuit.unlockableName = suit.UnlockableName;
        instance.unlockablesList.unlockables.Add(unlockableSuit);
    }
    private void HandlePurchasable(IPurchasable purchasable, StartOfRound instance, Terminal terminal,int unlockableID)
    {
        UnlockableItem purchasableSuit = instance.unlockablesList.unlockables.Last();
        TerminalKeyword buyKeyword = null;
        for (var i = 0; i < terminal.terminalNodes.allKeywords.Length; i++)
            if (terminal.terminalNodes.allKeywords[i].name == "Buy")
            {
                buyKeyword = terminal.terminalNodes.allKeywords[i];
                break;
            }

        if (buyKeyword == null)
        {
            throw new InvalidOperationException($"Buy keyword could not be found for registering purchasable!");
        }
        purchasableSuit.alreadyUnlocked = false;
        purchasableSuit.hasBeenMoved = false;
        purchasableSuit.placedPosition = Vector3.zero;
        purchasableSuit.placedRotation = Vector3.zero;

        purchasableSuit.shopSelectionNode = ScriptableObject.CreateInstance<TerminalNode>();
        purchasableSuit.shopSelectionNode.name = purchasableSuit.unlockableName + "SuitBuy1";
        purchasableSuit.shopSelectionNode.creatureName = purchasableSuit.unlockableName + " suit";
        purchasableSuit.shopSelectionNode.displayText = "You have requested to order " + purchasableSuit.unlockableName +
                                                " suits.\nTotal cost of item: [totalCost].\n\nPlease CONFIRM or DENY.\n\n";
        purchasableSuit.shopSelectionNode.clearPreviousText = true;
        purchasableSuit.shopSelectionNode.shipUnlockableID = unlockableID;
        purchasableSuit.shopSelectionNode.itemCost = purchasable.Price;
        purchasableSuit.shopSelectionNode.overrideOptions = true;
        
        var confirm = new CompatibleNoun();
        confirm.noun = ScriptableObject.CreateInstance<TerminalKeyword>();
        confirm.noun.word = "confirm";
        confirm.noun.isVerb = true;

        confirm.result = ScriptableObject.CreateInstance<TerminalNode>();
        confirm.result.name = purchasableSuit.unlockableName + "SuitBuyConfirm";
        confirm.result.creatureName = "";
        confirm.result.displayText =
            "Ordered " + purchasableSuit.unlockableName + " suits! Your new balance is [playerCredits].\n\n";
        confirm.result.clearPreviousText = true;
        confirm.result.shipUnlockableID = unlockableID;
        confirm.result.buyUnlockable = true;
        confirm.result.itemCost = purchasable.Price;
        confirm.result.terminalEvent = "";
        
        var deny = new CompatibleNoun();
        deny.noun = ScriptableObject.CreateInstance<TerminalKeyword>();
        deny.noun.word = "deny";
        deny.noun.isVerb = true;

        if (_cancelPurchase == null)
            _cancelPurchase =
                ScriptableObject.CreateInstance<TerminalNode>(); // we can use the same Cancel Purchase node
        deny.result = _cancelPurchase;
        deny.result.name = "MoreSuitsCancelPurchase";
        deny.result.displayText = "Cancelled order.\n";

        purchasableSuit.shopSelectionNode.terminalOptions = new[] { confirm, deny };

        var suitKeyword = ScriptableObject.CreateInstance<TerminalKeyword>();
        suitKeyword.name = purchasableSuit.unlockableName + "Suit";
        suitKeyword.word = purchasableSuit.unlockableName.ToLower() + " suit";
        suitKeyword.defaultVerb = buyKeyword;

        var suitCompatibleNoun = new CompatibleNoun();
        suitCompatibleNoun.noun = suitKeyword;
        suitCompatibleNoun.result = purchasableSuit.shopSelectionNode;
        var buyKeywordList = buyKeyword.compatibleNouns.ToList();
        buyKeywordList.Add(suitCompatibleNoun);
        buyKeyword.compatibleNouns = buyKeywordList.ToArray();

        var allKeywordsList = terminal.terminalNodes.allKeywords.ToList();
        allKeywordsList.Add(suitKeyword);
        allKeywordsList.Add(confirm.noun);
        allKeywordsList.Add(deny.noun);
        terminal.terminalNodes.allKeywords = allKeywordsList.ToArray();
    }


}