using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

public class AlmostNoDurability : Mod
{
    Harmony harmony;

    public void Start()
    {
        Log("loaded :)");
        harmony = new Harmony("de.felixpuetz.raftmods.almostnodurability");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public void OnModUnload()
    {
        harmony.UnpatchAll();
        Destroy(gameObject);
        Log("unloaded :(");
    }

    public override void WorldEvent_WorldLoaded()
    {
        GameModeValueManager.GetCurrentGameModeValue().toolVariables.areToolsIndestructible = true;
    }

    private void Log(String msg)
    {
        Debug.Log("[AlmostNoDurabilityMod]\t" + msg);
    }

    [HarmonyPatch(typeof(Slot), "IncrementUses")]
    public class HarmonyPatch_Equipment_UpdateEquipment
    {
        [HarmonyPrefix]
        static void Prefix(ref Slot __instance, ref int amountOfUsesToAdd)
        {
            if (__instance.slotType == SlotType.Equipment)
            {
                if (amountOfUsesToAdd < 0)
                {
                    amountOfUsesToAdd = 0;
                }
            }
        }
    }
}