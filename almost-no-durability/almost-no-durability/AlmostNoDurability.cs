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

    [HarmonyPatch(typeof(Slot), "IncrementUses")]
    public class HarmonyPatch_Slot_IncrementUses
    {
        [HarmonyPrefix]
        static void Prefix(ref Slot __instance, ref int amountOfUsesToAdd)
        {
            bool prevent = false;

            if (__instance.itemInstance.settings_consumeable.FoodType == FoodType.None && amountOfUsesToAdd != null)
            {
                if (modifier > 0) {
                    int hash = __instance.GetHashCode();

                    if (counterCache.ContainsKey(hash)) {
                        counterCache[hash] = counterCache[hash] + 1;
                    } else {
                        counterCache.Add(hash, 1);
                    }

                    if (counterCache[hash] < modifier) {
                        prevent = true;
                    } else {
                        counterCache[hash] = 0;
                    }
                } else {
                    prevent = true;
                }

                if (amountOfUsesToAdd < 0 && prevent)
                {
                    amountOfUsesToAdd = 0;
                }
            }
        }
    }

    /***************
    *   SETTINGS   *
    ***************/
    static HarmonyLib.Traverse ExtraSettingsAPI_Traverse;
    static bool ExtraSettingsAPI_Loaded = false;

    
    public void ExtraSettingsAPI_Load()
    {
        modifier = ExtraSettingsAPI_GetComboboxSelectedIndex("Durability Modifier");
    }

    public void ExtraSettingsAPI_SettingsOpen()
    {
        if (modifier > 0) {
            ExtraSettingsAPI_SetComboboxSelectedIndex("Durability Modifier", modifier);
        } else {
            ExtraSettingsAPI_SetComboboxSelectedIndex("Durability Modifier", 0);
        }
    }

    public void ExtraSettingsAPI_SettingsClose()
    {
        modifier = ExtraSettingsAPI_GetComboboxSelectedIndex("Durability Modifier");
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

}