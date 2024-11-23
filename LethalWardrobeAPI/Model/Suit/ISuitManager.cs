using System.Collections.Generic;
using LethalWardrobe.Model.Suit;

namespace LethalWardrobeAPI.Model.Suit;

public interface ISuitManager
{
    void RegisterSuit(ISuit suit);
    void UnregisterSuit(ulong id);
    ISuit? GetSuit(ulong id);
    List<ISuit> GetSuits();
}