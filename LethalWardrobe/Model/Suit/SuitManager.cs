using System;
using System.Collections.Generic;
using System.Linq;
using LethalWardrobeAPI.Model.Suit;

namespace LethalWardrobe.Model.Suit;

public class SuitManager : ISuitManager
{
    
    /// <summary>
    /// Singleton pattern call via a lazy implementation of the manager.
    /// </summary>
    private static readonly Lazy<SuitManager> instance = new Lazy<SuitManager>(() => new SuitManager());

    
    /// <summary>
    /// Gets the singleton instance of the manager.
    /// </summary>
    public static SuitManager Instance => instance.Value;
    
    private Dictionary<ulong,ISuit> _suits = new();
    public void RegisterSuit(ISuit suit)
    {
        _suits.Add(suit.Id, suit);
    }

    public void UnregisterSuit(ulong id)
    {
        _suits.Remove(id);
    }

    public ISuit GetSuit(ulong id) => _suits[id];

    public List<ISuit> GetSuits() => _suits.Values.ToList();
}