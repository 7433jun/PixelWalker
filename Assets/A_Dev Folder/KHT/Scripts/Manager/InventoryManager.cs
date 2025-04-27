using CSVData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager
{
    private static InventoryManager instance;
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
                instance = new InventoryManager();
            return instance;
        }
    }

    public readonly int equipmentSize = 4;
    public readonly int maxCost = 100;
    public readonly int inventorySize = 20;

    public int[] equipment { get; private set; }
    public int currentCost { get; private set; }
    public int[] inventory { get; private set; }
    public bool[] isInventoryUnlocked { get; private set; }

    private Dictionary<int, IItemFunc> itemFuncTable;

    private InventoryManager()
    {
        equipment = new int[equipmentSize];
        currentCost = 0;
        inventory = new int[inventorySize];
        isInventoryUnlocked = new bool[inventorySize];
        itemFuncTable = new Dictionary<int, IItemFunc>();

        // init itemFunc
        foreach (var kv in GameManager.Data.ItemDataTable)
        {
            if (kv.Value.ItemType == Project.Enums.ItemType.StableCore)
            {
                inventory[kv.Value.InventorySlotIndex - 1] = kv.Key;
        
                string itemFuncName = $"Item{kv.Key}";
                Type itemFuncType = Type.GetType(itemFuncName);
                if (itemFuncType != null)
                {
                    IItemFunc foundItemFunc = (IItemFunc)Activator.CreateInstance(itemFuncType);
        
                    itemFuncTable.Add(kv.Key, foundItemFunc);
                }
                else
                {
                    itemFuncTable.Add(kv.Key, new EmptyItem());
                }
            }
        }

        // test
        for (int i = 0; i < itemFuncTable.Count; i++)
        {
            isInventoryUnlocked[i] = true;
        }

        //for (int i = 0; i < inventorySize; i++)
        //{
        //    Debug.Log(inventory[i]);
        //}
    }

    // equipment right click
    public void Equip(int inventorySlotIndex)
    {
        int inventoryItemIndex = inventory[inventorySlotIndex];

        // already equiped
        for (int i = 0; i < equipmentSize; i++)
        {
            if (equipment[i] == inventoryItemIndex)
                return;
        }

        for (int i = 0; i < equipmentSize; i++)
        {
            if (equipment[i] == 0)
            {
                if (currentCost + GameManager.Data.ItemDataTable[inventoryItemIndex].Cost <= maxCost)
                {
                    equipment[i] = inventoryItemIndex;
                    currentCost += GameManager.Data.ItemDataTable[inventoryItemIndex].Cost;
                    itemFuncTable[inventoryItemIndex].Equip();

                    GameManager.UI.InventoryUI.DrawInventory();
                    return;
                }
                else
                {
                    // over cost

                    return;
                }
            }
        }

        // not empty slot
    }

    // drag to slot & drag to equipment
    public void Equip(int inventorySlotIndex, int equipmentSlotIndex)
    {
        int inventoryItemIndex = inventory[inventorySlotIndex];
        int equipmentItemIndex = equipment[equipmentSlotIndex];

        // empty equipment slot
        if (equipmentItemIndex == 0)
        {
            // already equiped
            for (int i = 0; i < equipmentSize; i++)
            {
                if (equipment[i] == inventoryItemIndex)
                {
                    equipment[i] = 0;
                    equipment[equipmentSlotIndex] = inventoryItemIndex;

                    GameManager.UI.InventoryUI.DrawInventory();
                    return;
                }
            }

            if (currentCost + GameManager.Data.ItemDataTable[inventoryItemIndex].Cost <= maxCost)
            {
                equipment[equipmentSlotIndex] = inventoryItemIndex;
                currentCost += GameManager.Data.ItemDataTable[inventoryItemIndex].Cost;
                itemFuncTable[inventoryItemIndex].Equip();
            }
            else
            {
                // over cost
            }
        }
        // not empty equipment slot
        else
        {
            // already equiped
            for (int i = 0; i < equipmentSize; i++)
            {
                if (equipment[i] == inventoryItemIndex)
                {
                    if (i != equipmentSlotIndex)
                    {
                        Unequip(equipmentSlotIndex);
                        equipment[i] = 0;
                        equipment[equipmentSlotIndex] = inventoryItemIndex;
                    }

                    GameManager.UI.InventoryUI.DrawInventory();
                    return;
                }
            }

            if (currentCost + GameManager.Data.ItemDataTable[inventoryItemIndex].Cost - GameManager.Data.ItemDataTable[equipmentItemIndex].Cost <= maxCost)
            {
                Unequip(equipmentSlotIndex);
                equipment[equipmentSlotIndex] = inventoryItemIndex;
                currentCost += GameManager.Data.ItemDataTable[inventoryItemIndex].Cost;
                itemFuncTable[inventoryItemIndex].Equip();
            }
            else
            {
                // over cost
            }
        }

        GameManager.UI.InventoryUI.DrawInventory();
    }
    
    // slot right click & drag slot to other
    public void Unequip(int equipmentSlotIndex)
    {
        int equipmentItemIndex = equipment[equipmentSlotIndex];

        itemFuncTable[equipmentItemIndex].Unequip();
        currentCost -= GameManager.Data.ItemDataTable[equipmentItemIndex].Cost;
        equipment[equipmentSlotIndex] = 0;

        GameManager.UI.InventoryUI.DrawInventory();
    }

    // drag slot to slot
    public void ChangeSlot(int equipmentSlotIndex1, int equipmentSlotIndex2)
    {
        int temp = equipment[equipmentSlotIndex1];
        equipment[equipmentSlotIndex1] = equipment[equipmentSlotIndex2];
        equipment[equipmentSlotIndex2] = temp;

        GameManager.UI.InventoryUI.DrawInventory();
    }

    public void UnlockEquipmenet(int EquipmentIndex)
    {
        isInventoryUnlocked[EquipmentIndex] = true;
        GameManager.UI.InventoryUI.DrawInventory();
    }
}
