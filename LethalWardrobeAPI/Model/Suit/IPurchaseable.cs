namespace LethalWardrobe.Model.Suit.Store;
/// <summary>
/// Interface representing the general functionality of something that is purchasable from the terminal in game. 
/// </summary>
public interface IPurchasable
{
    /// <summary>
    /// The price the instance will cost if purchased from the store. 
    /// </summary>
    int Price { get; set; }
}