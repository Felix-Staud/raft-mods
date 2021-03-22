using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;

public class AlmostNoDurability : Mod
{
    static JsonModInfo modInfo;
    static Dictionary<int, int> counterCache = new Dictionary<int, int>();
    static int modifier = 0;
    

    Harmony harmony;    

    public static void Log(object message)
    {
        Debug.Log("[" + modInfo.name + "]: " + message.ToString());
    }

    public static void ErrorLog(object message)
    {
        Debug.LogError("[" + modInfo.name + "]: " + message.ToString());
    }

    public static void setModifier(int value) 
    {
        modifier = value;
        Log("Modifier: "+ modifier);
    }

    public static bool slotShouldBeAffected(Slot slot) {
        return slot.itemInstance.settings_equipment != null 
            && slot.itemInstance.settings_equipment.EquipType != EquipSlotType.None;
    }

    /**
     * returns true if durability loss has to be prevented
     */
    public static bool doCache(object instance) {
        if (modifier > 0) {
            int hash = instance.GetHashCode();

            if (counterCache.ContainsKey(hash)) {
                counterCache[hash] = counterCache[hash] + 1;
            } else {
                counterCache.Add(hash, 1);
            }

            if (counterCache[hash] < modifier) {
                return true;
            } else {
                counterCache[hash] = 0;
                return false;
            }
        } else {
            return true;
        }
    }

    public void Start()
    {
        modInfo = modlistEntry.jsonmodinfo;
        harmony = new Harmony("de.felixpuetz.raftmods.almostnodurability");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Log("loaded :) [v"+ modInfo.version +"]");
    }

    public void OnModUnload()
    {
        harmony.UnpatchAll();
        Destroy(gameObject);
        Log("unloaded :(");
    }

    /*******************************
    *   EXTRA-SETTINGS-API STUFF   *
    ********************************/
    static HarmonyLib.Traverse ExtraSettingsAPI_Traverse;
    static bool ExtraSettingsAPI_Loaded = false;

    
    public void ExtraSettingsAPI_Load()
    {
        setModifier(ExtraSettingsAPI_GetComboboxSelectedIndex("durability"));
    }

    public void ExtraSettingsAPI_SettingsOpen()
    {
        if (modifier > 0) {
            ExtraSettingsAPI_SetComboboxSelectedIndex("durability", modifier);
        } else {
            ExtraSettingsAPI_SetComboboxSelectedIndex("durability", 0);
        }
    }

    public void ExtraSettingsAPI_SettingsClose()
    {
        setModifier(ExtraSettingsAPI_GetComboboxSelectedIndex("durability"));
    }

        public void ExtraSettingsAPI_SetComboboxSelectedIndex(string SettingName, int value)
    {
        if (ExtraSettingsAPI_Loaded)
            ExtraSettingsAPI_Traverse.Method("setComboboxSelectedIndex", new object[] { this, SettingName, value }).GetValue<int>();
    }

    public int ExtraSettingsAPI_GetComboboxSelectedIndex(string SettingName)
    {
        if (ExtraSettingsAPI_Loaded)
            return ExtraSettingsAPI_Traverse.Method("getComboboxSelectedIndex", new object[] { this, SettingName }).GetValue<int>();
        return -1;
    }

    /********************
    *   HARMONY STUFF   *
    *********************/
    [HarmonyPatch(typeof(Slot), "IncrementUses")]
    public class HarmonyPatch_Slot_IncrementUses
    {
        [HarmonyPrefix]
        static void Prefix(ref Slot __instance, ref int amountOfUsesToAdd)
        {
            if (slotShouldBeAffected(__instance)){
                if (amountOfUsesToAdd < 0 && doCache(__instance)){
                    amountOfUsesToAdd = 0;
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerInventory), "RemoveDurabillityFromHotSlot")]
    public class HarmonyPatch_PlayerInventory_RemoveDurabillityFromHotSlot
    {
        [HarmonyPrefix]
        static void Prefix(ref PlayerInventory __instance, ref int durabilityStacksToRemove)
        {
            if (durabilityStacksToRemove > 0 && doCache(__instance)){
                durabilityStacksToRemove = 0;
            }
        }
    }
}