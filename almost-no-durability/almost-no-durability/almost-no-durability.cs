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
        /*
        if (RAPI.GetLocalPlayer() == null) return;

        // Refresh item lists
        defaultMaxUses = new Dictionary<int, int>();
        List<Item_Base> allItems = ItemManager.GetAllItems();

        foreach (Item_Base item in allItems)
        {
            if (item.MaxUses > 1)
            {
                if (item.settings_equipment.EquipType != EquipSlotType.None)
                    defaultMaxUses.Add(item.UniqueIndex, item.MaxUses);
            }
        }

        SetToolDurabilityMultiplier(0.0000001f);
        SetEquipmentMaxUses(999999999);
        */
    }

    private void SetToolDurabilityMultiplier(float multiplier)
    {
        GameModeValueManager.GetCurrentGameModeValue().toolVariables.toolDurabilityLossMultiplier = multiplier;
        Log("set tool-durability-multiplier to: " + multiplier);
    }

    private void SetEquipmentMaxUses(int maxUses)
    {
        foreach (var kvp in defaultMaxUses)
        {
            Item_Base item = ItemManager.GetItemByIndex(kvp.Key);
            AccessTools.Field(typeof(Item_Base), "maxUses").SetValue(item, maxUses);
        };

        List<Slot> allSlots = new List<Slot>();
        allSlots.AddRange(RAPI.GetLocalPlayer().Inventory.allSlots.Where(slot => slot.HasValidItemInstance()));
        allSlots.AddRange(RAPI.GetLocalPlayer().Inventory.backpackSlots.Where(slot => slot.HasValidItemInstance()));
        allSlots.AddRange(RAPI.GetLocalPlayer().Inventory.equipSlots.Where(slot => slot.HasValidItemInstance()));
        foreach (var storage in StorageManager.allStorages)
            allSlots.AddRange(storage.GetInventoryReference().allSlots.Where(slot => slot.HasValidItemInstance()));

        // refresh instantiated items
        foreach (Slot slot in allSlots)
        {
            ItemInstance item = slot.itemInstance;
            Item_Base baseItem = item.baseItem;
            slot.SetUses(maxUses);
        }
        Log("set equipment-durability-max-uses to: " + maxUses);
    }


    private void Log(String msg)
    {
        Debug.Log("[AlmostNoDurabilityMod]\t" + msg);
    }

    [HarmonyPatch(typeof(PlayerInventory), "RemoveDurabillityFromHotSlot")]
    public class HarmonyPatch_PlayerInventory_RemoveDurabillityFromHotSlot
    {
        [HarmonyPostfix]
        static void PostFix(PlayerInventory __instance)
        {
        }

        static void Log(string msg)
        {
            Debug.Log("[almost-no-durability MOD - RemoveDurabillityFromHotSlot]\t" + msg);
        }
    }
}