using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace LethalWardrobe.Model.Config;
/// <summary>
/// Singleton that handles set plugin values in a controlled manner. Called via utilisation of ConfigKey enums.
/// </summary>
public class ConfigHandler
{
    /// <summary>
    /// Lazy singleton instance of the Config Handler.
    /// </summary>
    private static readonly Lazy<ConfigHandler> LazyInstance = new(() => new ConfigHandler());
    
    /// <summary>
    /// Getter for the Singleton instance of the handler. 
    /// </summary>
    public static ConfigHandler Instance => LazyInstance.Value;
    
    /// <summary>
    /// Dictionary cache of all the objects retrieved from the config of the plugin. 
    /// </summary>
    private readonly Dictionary<ConfigKey, object> _configValues = new();

    /// <summary>
    /// Initialises the config values in the handler. Needs to be called before runtime or else the handler does not work.
    /// </summary>
    /// <param name="config">The Bepinex plugin config to obtain values from.</param>
    public void Initialize(ConfigFile config)
    {
        _configValues[ConfigKey.MaxSuits] = config.Bind("General", "Max Suits", 5, "Max Suits that can be loaded.").Value;
        _configValues[ConfigKey.AllSuitsUnlocked] = config
            .Bind("General", "All Suits Unlocked", false, "Makes all suits wearable from the start of the game.")
            .Value;
        _configValues[ConfigKey.LoadAllSuits] = config
            .Bind("General", "Load All Suits", false, "Loads all suits regardless of amount.")
            .Value;
        _configValues[ConfigKey.AutoFitSuitsOnRack] = config
            .Bind("General", "Auto Fit Suits On Rack", false)
            .Value;
    }

    /// <summary>
    /// Gets the value of the specified type from the config handler, based on the specified Config Key.
    /// </summary>
    /// <param name="key">The Config Key of the value to get.</param>
    /// <typeparam name="T">The given type parameter of the config value to get.</typeparam>
    /// <returns>The specified value of the given config key.</returns>
    public T GetConfigValue<T>(ConfigKey key)
    {
        return _configValues.TryGetValue(key, out var value) ? (T)value : default;
    }
}
/// <summary>
/// Enum representing the various config values for the plugin.
/// </summary>
public enum ConfigKey
{
    /// <summary>
    /// Integer representing the amount of suits that can be loaded before stopping (to prevent overflow).
    /// </summary>
    MaxSuits,
    /// <summary>
    /// Boolean representing if all the custom suits should be auto unlocked at the beginning of play.
    /// </summary>
    AllSuitsUnlocked,
    /// <summary>
    /// If all the suits in the mod should be loaded, regardless of maximum suits. 
    /// </summary>
    LoadAllSuits,
    /// <summary>
    /// If suits should "squish" together to fit on the rack. This can lead to suits that are difficult to select.
    /// </summary>
    AutoFitSuitsOnRack
}
