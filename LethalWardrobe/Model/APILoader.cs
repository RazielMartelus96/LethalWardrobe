using LethalWardrobeAPI.Model;
using LethalWardrobeAPI.Model.Suit;
using UnityEngine;

namespace LethalWardrobe.Model;

public class APILoader : MonoBehaviour, IApiLoader
{
    public ISuitManager SuitManager => Suit.SuitManager.Instance;
}