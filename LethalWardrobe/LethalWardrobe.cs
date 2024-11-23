using BepInEx;
using LethalWardrobe.Model;
using LethalWardrobe.Model.Config;
using LethalWardrobe.Patches;
using UnityEngine;

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
        InitAPI();
        StartOfRoundPatches.Init(terminal);
    }

    private void InitConfig()
    {
        ConfigHandler.Instance.Initialize(Config);
    }

    private void InitAPI()
    {
        var apiLoaderObject = new GameObject("APILoader");
        apiLoaderObject.AddComponent<APILoader>();
        DontDestroyOnLoad(apiLoaderObject);

    }
}