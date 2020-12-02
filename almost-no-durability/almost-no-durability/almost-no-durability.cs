using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class AlmostNoDurability : Mod
{

    private IDictionary<int, int> defaultMaxUses = new Dictionary<int, int>();
    Harmony harmony;

    public void Start()
    {
        Log("loaded :)");
        //harmony = new Harmony("de.zeropublix.raftmods.SimplifiedPipes");
        //harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public void OnModUnload()
    {
        //harmony.UnpatchAll();
        //Destroy(gameObject);
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

    [HarmonyPatch(typeof(Equipment_AirBottle), "UpdateEquipment")]
    public class HarmonyPatch_Equipment_AirBottle_UpdateEquipment
    {
        [HarmonyPrefix]
        static void Prefix(Equipment_AirBottle __instance, Slot_Equip ___equippedSlot)
        {
            Log("incrementn uses by 1 before update");
            
            if (___equippedSlot != null)
            {
                ___equippedSlot.IncrementUses(+1, true);
            }
        }

        static void Log(string msg)
        {
            Debug.Log("[almost-no-durability MOD - HarmonyPatch_Equipment_AirBottle_UpdateEquipment]\t" + msg);
        }
    }
}