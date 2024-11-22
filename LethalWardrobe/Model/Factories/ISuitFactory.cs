using System.Collections.Generic;
using LethalWardrobe.Model.Suit;

namespace LethalWardrobe.Model.Factories;

public interface ISuitFactory : IListFactory<ISuit>
{
    void AddFolderPaths(List<string> paths);
    void AddPatchInstance(StartOfRound instance);
}