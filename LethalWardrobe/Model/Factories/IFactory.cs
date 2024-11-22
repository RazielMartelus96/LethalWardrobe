using System.Collections.Generic;
using System.Threading.Tasks;
using LethalWardrobe.Model.Suit;

namespace LethalWardrobe.Model.Factories;

/// <summary>
/// Interface representing the general functionality of a "List Factory" (A Factory pattern for specifically creating a
/// list of a given type). 
/// </summary>
/// <typeparam name="T">The type parameter of the created list.</typeparam>
public interface IListFactory<T>
{
    /// <summary>
    /// Creates the list of specified type. 
    /// </summary>
    /// <returns>The created list.</returns>
    List<T> Create();
}