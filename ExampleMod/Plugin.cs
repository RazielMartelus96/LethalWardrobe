using BepInEx;
using LethalWardrobeAPI.Model;
using MonoMod;
namespace ExampleMod;
[BepInPlugin(ModGuid, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string ModGuid = "Curiosity-Core.Lethal_Wardrobe_TestMod";
    private const string ModName = "Lethal Wardrobe Test Mod";
    private const string ModVersion = "0.0.1";

    private void Awake()
    {
        LethalWardrobeApi.Instance.Initialize(this);
        Patches.Init();
    }
    
}