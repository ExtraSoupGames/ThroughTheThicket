using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
//there will be multiple inventories, for different item types, that all use this base class for item storing methods and data
public abstract class Inventory
{
    // allows for custom shaped inventories
    protected InventorySlot[,] slots;
    private int x;
    private int y;
    private HashSet<Items> allowedItems;
    protected string inventoryName;
    public Inventory(bool[,] shape, int topLeftX, int topLeftY, HashSet<Items> itemsAllowed, string inventoryName)
    {
        slots = new InventorySlot[shape.GetLength(0), shape.GetLength(1)];
        for (int itemX = 0; itemX < shape.GetLength(0); itemX++)
        {
            for (int itemY = 0; itemY < shape.GetLength(1); itemY++)
            {
                slots[itemX, itemY] = new InventorySlot(this, shape[itemX, itemY], itemX, itemY);
            }
        }
        x = topLeftX;
        y = topLeftY;
        allowedItems = itemsAllowed;
        this.inventoryName = inventoryName;
    }
    protected virtual VisualElement ConstructAsTab(out VisualElement inventoryGrid, int tabOffset, InventoryManager invenManager, int tabIndex, bool isSelectedTab)
    {
        VisualElement newTab = new VisualElement();
        newTab.pickingMode = PickingMode.Ignore;
        newTab.AddToClassList("inventory-tab");

        Button tabSelector = new Button();
        //TODO assign clickableness to button
        tabSelector.clicked += () => invenManager.SelectTab(tabIndex);
        tabSelector.AddToClassList("tab-selector");
        tabSelector.style.top = new Length(tabOffset, LengthUnit.Percent);
        newTab.Add(tabSelector);

        if (!isSelectedTab)
        {
            inventoryGrid = null;
            return newTab;
        }
        VisualElement inventoryHolder = new VisualElement();
        inventoryHolder.AddToClassList("tab-inventory-holder");
        newTab.Add(inventoryHolder);

        Label inventoryNameLabel = new Label(inventoryName);
        inventoryNameLabel.AddToClassList("inventory-title");

        VisualElement inventoryHeader = new VisualElement();
        inventoryHeader.AddToClassList("header");
        inventoryHeader.Add(inventoryNameLabel);

        inventoryHolder.Add(inventoryHeader);

        inventoryGrid = new VisualElement();
        inventoryGrid.AddToClassList("inventory-grid");
        inventoryHolder.Add(inventoryGrid);

        return newTab;
    }
    public int SlotSize()
    {
        //this determines the slot size in pixels ( including padding ect ) as this is used to determine which slot is being hovered
        //also defined in UIRenderer
        return 64;
    }
    public void ScreenCoordsToInventory(int inX, int inY, out int invenX, out int invenY)
    {
        invenX = inX - x;
        invenY = inY - y;
    }
    public Vector2 GetLocation()
    {
        return new Vector2(x, y);
    }
    public InventorySlot[,] GetSlots()
    {
        return slots;
    }
    public string GetName()
    {
        return inventoryName;
    }
    public bool ContainsSlotAt(int gridX, int gridY)
    {
        if(!(0 <= gridX && gridX < slots.GetLength(0)))
        {
            return false;
        }
        if (!(0 <= gridY && gridY < slots.GetLength(1)))
        {
            return false;
        }
        return slots[gridX, gridY].IsValid();
    }
    public InventorySlot GetSlot(int gridX, int gridY)
    {
        if (!(0 <= gridX && gridX < slots.GetLength(0)))
        {
            return null;
        }
        if (!(0 <= gridY && gridY < slots.GetLength(1)))
        {
            return null;
        }
        return slots[gridX, gridY];
    }

    public virtual void ClickAt(ref StackItem heldItem, ref InventorySlot hoveredSlot)
    {
        if(heldItem != null)
        {
            if (!allowedItems.Contains(heldItem.GetItemType()))
            {
                //TODO possible feedback / advice to player - this is the wrong inventory for this item
                Debug.Log("Cancelling click - item not allowed");
                return;
            }
            if (!ItemCanFit(hoveredSlot.x, hoveredSlot.y, heldItem))
            {
                //TODO possible feedback / advice to player - this item cannot fit
                Debug.Log("Cancelling click - item does not fit");
                return;
            }
        }
        SwapItem(ref heldItem, hoveredSlot);
    }
    public void ClickAtSlot(InventoryManager invenManager, InventorySlot slot, VisualElement slotVisual)
    {
        StackItem heldItem = invenManager.GetHeldItem();
        ClickAt(ref heldItem, ref slot);
        //update the whole inventory visual
        invenManager.SetHeldItem(heldItem);
        invenManager.UpdateHeldItem();
        invenManager.RefreshInventory();
    }
    public abstract void PopulateGrid(VisualElement inventoryContainer, InventoryManager invenManager, int tabOffset = 0, int tabIndex = 0, bool isSelectedTab = false);
    public abstract bool ItemCanFit(int slotX, int slotY, StackItem item);
    public abstract void SwapItem(ref StackItem heldItem, InventorySlot hoveredSlot);

    public List<PersistentSlot> GetPersistentItems(out int slotsWidth, out int slotsHeight)
    {
        List<PersistentSlot> persistentSlots = new List<PersistentSlot>();
        for (int itemX = 0; itemX < slots.GetLength(0); itemX++)
        {
            for (int itemY = 0; itemY < slots.GetLength(1); itemY++)
            {
                persistentSlots.Add(new PersistentSlot(slots[itemX, itemY].IsValid(), slots[itemX, itemY]));
            }
        }
        slotsWidth = slots.GetLength(0);
        slotsHeight = slots.GetLength(1);
        return persistentSlots;
    }
    public Inventory(List<PersistentSlot> loadSlots, int width, int height, string inventoryName, HashSet<Items> allowedItems)
    {

        x = 0;
        y = 0;
        this.allowedItems = allowedItems;
        this.inventoryName = inventoryName;
    }
}
public abstract class StackInventory : Inventory
{
    public StackInventory(bool[,] shape, int topLeftX, int topLeftY, HashSet<Items> itemsAllowed, string inventoryName) : base(shape, topLeftX, topLeftY, itemsAllowed, inventoryName)
    {
    }
    public StackInventory(List<PersistentSlot> dataSlots, int width, int height, string invenName, HashSet<Items> itemsAllowed) : base(dataSlots, width, height, invenName, itemsAllowed)
    {
        slots = new InventorySlot[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PersistentSlot dataSlot = dataSlots[(y*width) + x];
                slots[x, y] = new InventorySlot(this, dataSlot.isValid, x, y);
                if (dataSlot.itemType != Items.ErrorItem)
                {
                    slots[x, y].item = new StackItem(ItemHelper.GetItemFromType(dataSlot.itemType), dataSlot.count);
                }
            }
        }
        inventoryName = invenName;
    }

    public override bool ItemCanFit(int slotX, int slotY, StackItem item)
    {
        //In a stack inventory, if the slot is valid, then it can fit
        return true;
    }
    public override void SwapItem(ref StackItem heldItem, InventorySlot hoveredSlot)
    {
        //if the items are the same, add them to the stack
        if (heldItem != null && hoveredSlot.item != null && heldItem.GetItemType() == hoveredSlot.item.GetItemType())
        {
            StackItem item = (StackItem)heldItem;
            ((StackItem)hoveredSlot.item).AddToStack(ref item);
            heldItem = item;
            return;
        }
        StackItem tempItem = hoveredSlot.item == null ? null : hoveredSlot.item;
        hoveredSlot.item = heldItem == null ? null : heldItem;
        heldItem = tempItem;
    }
    public override void PopulateGrid(VisualElement inventoryContainer, InventoryManager invenManager, int tabOffset = 0, int tabIndex = 0, bool isSelectedTab = false)
    {
        VisualElement inventoryGrid;
        if (tabOffset == 0)
        {
            isSelectedTab = true;
            inventoryGrid = inventoryContainer.Q<VisualElement>("ItemGrid");
        }
        else
        {
            inventoryContainer.Add(ConstructAsTab(out inventoryGrid, tabOffset, invenManager, tabIndex, isSelectedTab));
        }
        if (!isSelectedTab)
        {
            return;
        }
        for (int i = 0; i < GetSlots().GetLength(0); i++)
        {
            //Create a new row
            VisualElement newRow = new VisualElement();
            newRow.AddToClassList("item-row");
            inventoryGrid.Add(newRow);
            //Populate it with slots
            for (int j = 0; j < GetSlots().GetLength(1); j++)
            {
                InventorySlot currentSlot = GetSlots()[i, j];
                //Create a new slot (or placeholder if no slot is present)
                if (currentSlot.IsValid())
                {
                    Button newSlot = new Button();
                    newSlot.clicked += (() => ClickAtSlot(invenManager, currentSlot, newSlot));
                    newSlot.AddToClassList("item-slot");

                    if (!currentSlot.IsEmpty())
                    {
                        currentSlot.item.PopulateSlot(newSlot);
                    }
                    newRow.Add(newSlot);
                }
                else
                {
                    VisualElement newSlot = new VisualElement();
                    newSlot.AddToClassList("item-slot-placeholder");
                    newRow.Add(newSlot);
                }
            }
        }
    }
}
public class InventorySlot
{
    bool isValid;
    public StackItem item;
    //relative coordinates of the grid of the inventory it is contained in
    public int x;
    public int y;
    private Inventory inven;
    public InventorySlot(Inventory inventory, bool valid, int X, int Y)
    {
        isValid = valid;
        item = null;
        x = X;
        y = Y;
        inven = inventory;
    }
    public bool IsEmpty()
    {
        return isValid && item == null;
    }
    public bool IsValid()
    {
        return isValid;
    }
    public void TestFill()
    {
        item = new StackItem(new Stone());
    }
    public Inventory GetInventory()
    {
        return inven;
    }
    public void Remove(int removalCount = 1)
    {
        //item.Remove returns true if the slot should now be emptied
        if (item.Remove(removalCount))
        {
            item = null;
        }
    }
}