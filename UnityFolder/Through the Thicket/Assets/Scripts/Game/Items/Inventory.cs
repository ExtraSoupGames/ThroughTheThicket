using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
//there will be multiple inventories, for different item types, that all use this base class for item storing methods and data
public abstract class Inventory
{
    // allows for custom shaped inventories
    protected InventorySlot[,] slots;
    public string name;
    private int x;
    private int y;
    private HashSet<Items> allowedItems;
    public Inventory(bool[,] shape, int topLeftX, int topLeftY, HashSet<Items> itemsAllowed)
    {
        slots = new InventorySlot[shape.GetLength(0), shape.GetLength(1)];
        for (int itemX = 0; itemX < shape.GetLength(0); itemX++)
        {
            for (int itemY = 0; itemY < shape.GetLength(1); itemY++)
            {
                slots[itemX, itemY] = new InventorySlot(shape[itemX, itemY], itemX, itemY);
            }
        }
        x = topLeftX;
        y = topLeftY;
        allowedItems = itemsAllowed;
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

    public void ClickAt(ref Item heldItem, InventorySlot hoveredSlot)
    {
        if(heldItem != null && heldItem != null)
        {
            if (!allowedItems.Contains(heldItem.GetItemType()))
            {
                //TODO possible feedback / advice to player - this is the wrong inventory for this item
                return;
            }
            if (!ItemCanFit(hoveredSlot.x, hoveredSlot.y, heldItem))
            {
                //TODO possible feedback / advice to player - this item cannot fit
                return;
            }
        }

        Item tempItem = hoveredSlot.item == null ? null : hoveredSlot.item;
        hoveredSlot.item = heldItem == null ? null : heldItem;
        heldItem = tempItem;
        Debug.Log("Item SWAPPED");
    }
    public abstract bool ItemCanFit(int slotX, int slotY, Item item);

}
public abstract class StackInventory : Inventory
{
    //TODO convert to inventory slot
    private StackItem[] items;
    public StackInventory(bool[,] shape, int topLeftX, int topLeftY, HashSet<Items> itemsAllowed) : base(shape, topLeftX, topLeftY, itemsAllowed)
    {
    }
    public override bool ItemCanFit(int slotX, int slotY, Item item)
    {
        //TODO IMPLEMENT
        return true;
    }
}
public abstract class ShapeInventory : Inventory
{
    bool[,] filledSlots;
    public ShapeInventory(bool[,] shape, int topLeftX, int topLeftY, HashSet<Items> itemsAllowed) : base(shape, topLeftX, topLeftY, itemsAllowed)
    {
        filledSlots = new bool[shape.GetLength(0), shape.GetLength(1)];
        for(int i = 0; i < shape.GetLength(0); i++)
        {
            Array.Copy(shape, i * shape.GetLength(1),
                       filledSlots, i * shape.GetLength(1),
                       shape.GetLength(1));
        }
    }
    public override bool ItemCanFit(int itemX, int itemY, Item item)
    {
        //ensure the item being held is shaped and not a stack item
        if(!(item is ShapeItem))
        {
            return false;
        }
        //cast the grabbed item to a shape item
        bool[,] itemShape = ((ShapeItem)(item)).GetShape();
        //TODO snap the item to the grid and or snap it to a valid location in the inventory
        for (int x = 0;x< itemShape.GetLength(0); x++)
        {
            for(int y = 0; y < itemShape.GetLength(1); y++)
            {
                if (itemShape[x,y])
                {
                    int gridX = itemX + x;
                    int gridY = itemY + y;
                    if (!slots[gridX, gridY].IsEmpty())
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
public class InventorySlot
{
    bool isValid;
    public Item item;
    //relative coordinates of the grid of the inventory it is contained in
    public int x;
    public int y;
    public InventorySlot(bool valid, int X, int Y)
    {
        isValid = valid;
        item = null;
        x = X;
        y = Y;
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
        item = new StackItem(Items.Stone);
    }
}