using HarmonyLib;
using System.Linq;
using System.Reflection;
using UltimateWater;
using UnityEngine;

public class SimplifiedPipes : Mod
{
    Harmony harmony;

    public void Start()
    {
        harmony = new Harmony("de.zeropublix.raftmods.SimplifiedPipes");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        Log("loaded :)");
    }

    public void OnModUnload()
    {
        harmony.UnpatchAll();
        Destroy(gameObject);
        Log("unloaded :(");
    }

    void Log(string msg)
    {
        Debug.Log("[Simplified Pipes MOD]\t" + msg);
    }
}

[HarmonyPatch(typeof(Block_Pipe), "OnFinishedPlacement")]
public class HarmonyPatch_BlockPipe_OnFinishedPlacement
{
    static string[] supportedItems = new string[] {
        "Placeable_Pipe_Fuel", 
        "Placeable_Pipe_Water" 
    };

    [HarmonyPostfix]
    static void PostFix(Block_Pipe __instance)
    {
        if (!supportedItems.Contains(__instance.buildableItem.UniqueName)) return;
        Vector3 pos = __instance.transform.position;

        __instance.transform.position = new Vector3(pos.x, pos.y - .1f, pos.z);
        __instance.transform.localScale = new Vector3(1f, .05f, 1f);
    }

    static void Log(string msg)
    {
        Debug.Log("[Simplified Pipes MOD - OnFinishedPlacement]\t" + msg);
    }
}

/*
[HarmonyPatch(typeof(Block_Pipe), "OnBitmaskTileChange")]
public class HarmonyPatch_BlockPipe_OnBitmaskTileChange
{
    [HarmonyPostfix]
    static void PostFix(GameObject ___currentPipeCollision)
    {
        Vector3 pos = ___currentPipeCollision.transform.position;

        ___currentPipeCollision.transform.position = new Vector3(pos.x, pos.y - .9f, pos.z);
    }

    static void Log(string msg)
    {
        Debug.Log("[Simplified Pipes MOD - OnBitmaskTileChange]\t" + msg);
    }
}*/