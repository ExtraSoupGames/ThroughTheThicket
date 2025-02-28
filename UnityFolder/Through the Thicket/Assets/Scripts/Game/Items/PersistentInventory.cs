using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct PersistentInventories
{
    public List<PersistentInventory> inventories;
    public PersistentInventories(Inventory mainInventory, Inventory craftingInventory, List<Inventory> subInventories)
    {
        inventories = new List<PersistentInventory>();
        inventories.Add(new PersistentInventory(mainInventory));
        inventories.Add(new PersistentInventory(craftingInventory));
        foreach(Inventory i in subInventories)
        {
            inventories.Add(new PersistentInventory(i));
        }
    }

}
[System.Serializable]
public struct PersistentInventory
{
    public List<PersistentSlot> slots;
    public int slotsWidth;
    public int slotsHeight;
    public string inventoryName;
    public PersistentInventory(Inventory inven)
    {
        slots = inven.GetPersistentItems(out slotsWidth, out slotsHeight);
        inventoryName = inven.GetName();
    }
    public StackInventory GetInventory(bool isCraftingArea)
    {
        if (isCraftingArea)
        {
            return new CraftingArea(slots, slotsWidth, slotsHeight);
        }
        return new TestInventory(slots, slotsWidth, slotsHeight, inventoryName);
    }
}
[System.Serializable]
public struct PersistentSlot
{
    //true if there "is a slot there", I.E. if the slot would not be a placeholder in the UI
    public bool isValid;
    public Items itemType;
    public int count;
    public PersistentSlot(bool isValid, InventorySlot slot = null)
    {
        this.isValid = isValid;
        if (isValid && slot.item != null)
        {
            itemType = slot.item.GetItemType();
            count = slot.item.GetCount();
            return;
        }
        itemType = Items.ErrorItem;
        count = 0;
    }
}