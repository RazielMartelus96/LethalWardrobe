using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;
using LethalModDataLib.Features;
using LethalWardrobe.Model.Suit;
using UnityEngine;

namespace LethalWardrobe.Model.Persistence;

public class PersistenceManager
{

    private SuitDataList _suitData;
    private static readonly Lazy<PersistenceManager> LazyInstance = new(() => new PersistenceManager());
    public static PersistenceManager Instance => LazyInstance.Value;

    private PersistenceManager()
    {
        _suitData = new SuitDataList
        {
            Suits = []
        };
        ModDataHandler.RegisterInstance(_suitData);
    }

    public SuitDataList GetSuitData() => _suitData;
}
[Serializable]
public class SuitDataList()
{
    [ModData(SaveWhen.OnSave,SaveWhen = SaveWhen.OnAutoSave,SaveLocation = SaveLocation.GeneralSave, LoadWhen = LoadWhen.OnLoad)]
    public List<SuitData> Suits { get; set; }
}
public class SuitData()
{
    public string Name { get; set; }
    public bool IsUnlocked { get; set; }
}


