using System.Collections;
using LethalWardrobeAPI.Model;
using On.GameNetcodeStuff;
using UnityEngine;

namespace ExampleMod;

public class Patches
{
    public static void Init()
    {
        On.GameNetcodeStuff.PlayerControllerB.PlayerJumpedServerRpc += PlayerControllerBOnPlayerJumpedServerRpc;
        On.GameNetcodeStuff.PlayerControllerB.PlayerJump += PlayerControllerBOnPlayerJump;
    }

    private static IEnumerator PlayerControllerBOnPlayerJump(PlayerControllerB.orig_PlayerJump orig,
        GameNetcodeStuff.PlayerControllerB self)
    {
        IEnumerator originalMethod = orig(self);
        PrintSuitInfo();
        return originalMethod;
    }

    private static void PlayerControllerBOnPlayerJumpedServerRpc(PlayerControllerB.orig_PlayerJumpedServerRpc orig,
        GameNetcodeStuff.PlayerControllerB self)
    {
        orig(self);
        PrintSuitInfo();
    }

    private static void PrintSuitInfo()
    {
        foreach (var suit in LethalWardrobeApi.Instance.GetSuits())
            Debug.Log($"Suit id: {suit.Id}" +
                      $", Suit Name: {suit.UnlockableName}" +
                      $", Suit Material Name: {suit.SuitMaterial.name}");
    }
}