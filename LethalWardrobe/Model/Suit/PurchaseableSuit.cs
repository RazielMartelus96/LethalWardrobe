using LethalWardrobe.Model.Suit.Store;
using UnityEngine;

namespace LethalWardrobe.Model.Suit;


public class PurchaseableSuit : ISuit, IPurchasable
{
    public ulong Id { get; set; }
    public string UnlockableName { get; set; }
    public Material SuitMaterial { get; set; }
    public bool IsDefault { get; set; }
    public bool IsUnlocked { get; set; }
    public int Price { get; set; }
}