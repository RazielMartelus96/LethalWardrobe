using UnityEngine;

namespace LethalWardrobe.Model.Suit;

/// <summary>
/// Instance containing the key data of a Custom Suit within the Game World. 
/// </summary>
public class CustomSuit : ISuit
{
    /// <inheritdoc/>
    public string UnlockableName { get; set; }
    /// <inheritdoc/>
    public Material SuitMaterial { get; set; }
    /// <inheritdoc/>
    public bool IsDefault { get; set; }
}