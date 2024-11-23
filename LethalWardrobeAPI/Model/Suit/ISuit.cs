using UnityEngine;

namespace LethalWardrobe.Model.Suit;

/// <summary>
/// Interface representing the general functionality of a Suit within the game. This interface has fields for the name,
/// Unity Material, and a boolean that represents if the suit is a default suit or not.  
/// </summary>
public interface ISuit
{
    
    /// <summary>
    /// The unique id of the suit. Set within the suit's files. 
    /// </summary>
    ulong Id { get; set; }
    /// <summary>
    /// The Unlocklable name of the Suit.
    /// </summary>
    string UnlockableName { get; set; }
    
    /// <summary>
    /// The Material of the Suit's asset.
    /// </summary>
    Material SuitMaterial { get; set; }
    
    /// <summary>
    /// Boolean check representing whether the suit is unlocked by default or not. 
    /// </summary>
    bool IsDefault { get; set; }

}