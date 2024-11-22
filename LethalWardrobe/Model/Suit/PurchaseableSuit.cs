using UnityEngine;

namespace LethalWardrobe.Model.Suit.Store;

public class PurchaseableSuit : ISuit, IPurchasable
{
    public string UnlockableName { get; set; }
    public Material SuitMaterial { get; set; }
    public bool IsDefault { get; set; }
    public int Price { get; set; }
}