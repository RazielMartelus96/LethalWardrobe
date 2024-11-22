using BepInEx;
using LethalWardrobe.Model.Config;
using LethalWardrobe.Patches;

namespace LethalWardrobe;
[BepInPlugin(ModGuid, ModName, ModVersion)]
public class LethalWardrobe : BaseUnityPlugin
{
    private const string ModGuid = "Curiosity-Core.Lethal_Wardrobe";
    private const string ModName = "Lethal Wardrobe";
    private const string ModVersion = "0.0.1";

    private void Awake()
    {
        var terminal = FindObjectOfType<Terminal>();
        InitConfig();
        StartOfRoundPatches.Init(terminal);
    }

    private void InitConfig()
    {
        ConfigHandler.Instance.Initialize(Config);
    }
}