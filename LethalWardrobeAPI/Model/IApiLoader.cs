using LethalWardrobeAPI.Model.Suit;

namespace LethalWardrobeAPI.Model;

public interface IApiLoader
{
    ISuitManager SuitManager { get; }
}