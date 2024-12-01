using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LethalWardrobe.Model.Suit;
using LethalWardrobe.Model.Suit.Store;
using LethalWardrobe.Model.Util;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace LethalWardrobe.Model.Factories;

public class SuitFactory : MonoBehaviour, ISuitFactory
{
    private List<string> _assetPaths = [];
    private readonly Dictionary<string, Material> _customMaterialCache = [];
    private StartOfRound _instance;
    private List<string> _texturePaths = [];
    public List<string> SuitFolderPaths { get; set; }
    private readonly Dictionary<string, Material> _materialCache = new();
    public List<ISuit> Create()
    {
        List<ISuit> suits = new();
        SetPaths();
        SortPaths();
        foreach (var suitPath in _assetPaths)
        {
            var assetBundle = AssetBundle.LoadFromFile(suitPath);
            var assets = assetBundle.LoadAllAssets();
            foreach (var asset in assets)
            {
                if (asset is Material material) _customMaterialCache.Add(material.name, material);
            }
        }
        ulong suitsCount = 0;
        foreach (var texturePath in _texturePaths)
        {
            ISuit suit = HandleSuitModifications(InitSuitFromPath(texturePath, suitsCount), texturePath);
            SuitManager.Instance.RegisterSuit(suit);
            suits.Add(suit);
            suitsCount++;
        }
        

        Debug.Log($"Created {suits.Count} suits!!!");
        return suits.ToList();
    }

    public void AddFolderPaths(List<string> paths)
    {
        SuitFolderPaths = paths;
        Debug.Log($"Paths found: {SuitFolderPaths.Count}");
    }

    public void AddPatchInstance(StartOfRound instance)
    {
        _instance = instance;
    }

    private ISuit InitSuitFromPath(string texturePath, ulong suitsCount)
    {
        if (string.IsNullOrEmpty(texturePath))
        {
            Debug.LogError("Texture path is null or empty.");
            return null;
        }
        
        ISuit suit = new CustomSuit();
        suit.Id = suitsCount;
        suit.UnlockableName = Path.GetFileNameWithoutExtension(texturePath);
        suit.SuitMaterial = InitMaterialFromPath(texturePath);
        if (suit.SuitMaterial == null) Debug.LogError($"Failed to initialize material from path: {texturePath}");

        return suit;
    }

    private Material InitMaterialFromPath(string texturePath)
    {
        if (_materialCache.TryGetValue(texturePath, out var cachedMaterial))
            return cachedMaterial;

        var originalSuit = SuitUtils.GetOriginalSuit(ref _instance);
        var material = Path
            .GetFileNameWithoutExtension(texturePath)
            .ToLower() == "default"
            ? GetDefaultMaterial()
            : Instantiate(JsonUtility.FromJson<UnlockableItem>(JsonUtility.ToJson(originalSuit)).suitMaterial);

        material.mainTexture = InitSuitTextureFromPath(texturePath);
        _materialCache[texturePath] = material;

        return material;
    }
    private Material _defaultMaterial;

    private Material GetDefaultMaterial()
    {
        if (_defaultMaterial == null)
            _defaultMaterial = SuitUtils.GetOriginalSuit(ref _instance).suitMaterial;
        return _defaultMaterial;
    }
    private Texture2D InitSuitTextureFromPath(string texturePath)
    {
        var fileData = File.ReadAllBytes(texturePath);
        var texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        texture.Apply(true, true);
        return texture;
    }


    private void SetPaths()
    {
        var paths = InitPaths();
        _texturePaths = paths.texturePaths;
        _assetPaths = paths.assetPaths;
    }

    private void SortPaths()
    {
        _texturePaths.Sort();
        _assetPaths.Sort();
    }

    private (List<string> texturePaths, List<string> assetPaths) InitPaths()
    {
        List<string> texturePaths = [];
        List<string> assetPaths = [];

        foreach (var suitsFolderPath in SuitFolderPaths)
        {
            if (suitsFolderPath == "") continue;
            var pngFiles = Directory.GetFiles(suitsFolderPath, "*.png");
            var bundleFiles = Directory.GetFiles(suitsFolderPath, "*.matbundle");
            texturePaths.AddRange(pngFiles);
            assetPaths.AddRange(bundleFiles);
            Debug.Log($"Found {texturePaths.Count} texture paths. Found {assetPaths.Count} asset paths.");
        }

        return (texturePaths, assetPaths);
    }

    private ISuit HandleSuitModifications(ISuit suit, string texturePath)
    {
        {
            var advancedJsonPath = Path.Combine(Path.GetDirectoryName(texturePath) ?? throw new InvalidOperationException(), "advanced",
                suit.UnlockableName + ".json");


            if (!File.Exists(advancedJsonPath)) return suit;


            foreach (var line in File.ReadAllLines(advancedJsonPath))
            {
                var keyValue = line.Trim().Split(':');
                if (keyValue.Length != 2) continue;

                var keyData = keyValue[0].Trim('"', ' ', ',');
                var valueData = keyValue[1].Trim('"', ' ', ',');

                if (valueData.EndsWith(".png"))
                    LoadAdvancedTexture(texturePath, valueData, keyData, suit.SuitMaterial);
                else
                {
                    switch (valueData)
                    {
                        case "KEYWORD":
                            suit.SuitMaterial.EnableKeyword(keyData);
                            break;

                        case "DISABLEKEYWORD":
                            suit.SuitMaterial.DisableKeyword(keyData);
                            break;

                        case "SHADERPASS":
                            suit.SuitMaterial.SetShaderPassEnabled(keyData, true);
                            break;

                        case "DISABLESHADERPASS":
                            suit.SuitMaterial.SetShaderPassEnabled(keyData, false);
                            break;

                        default:
                            suit.SuitMaterial = ApplyNumericOrVectorValue(keyData, valueData, suit.SuitMaterial);
                            break;
                    }

                    switch (keyData)
                    {
                        case "PRICE" when int.TryParse(valueData, out var price):
                            suit = new PurchaseableSuit
                            {
                                Id = suit.Id,
                                UnlockableName = suit.UnlockableName,
                                SuitMaterial = suit.SuitMaterial,
                                Price = price
                                
                            };
                            break;
                        case "SHADER":
                            var shader = Shader.Find(valueData);
                            suit.SuitMaterial.shader = shader;
                            break;
                        case "MATERIAL":
                            suit.SuitMaterial = ApplyCustomMaterial(valueData, suit.SuitMaterial.mainTexture);
                            break;
                    }
                }
                
            }
        }
        return suit;
    }

    private void LoadAdvancedTexture(string baseTexturePath, string textureFileName, string textureKey,
        Material material)
    {
        var texturePath = Path.Combine(Path.GetDirectoryName(baseTexturePath) ?? throw new InvalidOperationException(), "advanced", textureFileName);
        var textureData = File.ReadAllBytes(texturePath);
        var advancedTexture = new Texture2D(2, 2);
        advancedTexture.LoadImage(textureData);
        advancedTexture.Apply(true, true);
        material.SetTexture(textureKey, advancedTexture);
    }

    private Material ApplyCustomMaterial(string materialName, Texture mainTexture)
    {
        var customMaterial = Instantiate(_customMaterialCache[materialName]);
        customMaterial.mainTexture = mainTexture;
        return customMaterial;
    }

    private Material ApplyNumericOrVectorValue(string key, string value, Material material)
    {
        if (float.TryParse(value, out var floatValue))
        {
            material.SetFloat(key, floatValue);
        }
        else if (TryParseVector4(value, out var vectorValue))
        {
            material.SetVector(key, vectorValue);
        }
        return material;
    }
    

    private static bool TryParseVector4(string input, out Vector4 vector)
    {
        vector = Vector4.zero;

        var components = input.Split(',');

        if (components.Length == 4)
            if (float.TryParse(components[0], out var x) &&
                float.TryParse(components[1], out var y) &&
                float.TryParse(components[2], out var z) &&
                float.TryParse(components[3], out var w))
            {
                vector = new Vector4(x, y, z, w);
                return true;
            }

        return false;
    }
}