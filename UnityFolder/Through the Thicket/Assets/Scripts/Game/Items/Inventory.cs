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
    public Inventory(bool[,] shape, int topLeftX, int topLeftY)
    {
        slots = new InventorySlot[shape.GetLength(0), shape.GetLength(1)];
        for (int itemX = 0; itemX < shape.GetLength(0); itemX++)
        {
            for (int itemY = 0; itemY < shape.GetLength(1); itemY++)
            {
                slots[itemX, itemY] = new InventorySlot(shape[itemX, itemY]);
            }
        }
        x = topLeftX;
        y = topLeftY;
    }
    public int SlotSize()
    {
        //this determines the slot size in pixels ( including padding ect ) as this is used to determine which slot is being hovered
        //also defined in UIRenderer
        return 32;
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
        if(!(0 <= gridX && gridX <= slots.GetLength(0)))
        {
            return false;
        }
        if (!(0 <= gridY && gridY <= slots.GetLength(1)))
        {
            return false;
        }
        return slots[gridX, gridY].IsValid();
    }
    public InventorySlot GetSlot(int gridX, int gridY)
    {
        if (!(0 <= gridX && gridX <= slots.GetLength(0)))
        {
            return null;
        }
        if (!(0 <= gridY && gridY <= slots.GetLength(1)))
        {
            return null;
        }
        return slots[gridX, gridY];
    }
}
public abstract class StackInventory : Inventory
{
    private HashSet<Items> allowedItems;
    private StackItem[] items;
    public StackInventory(HashSet<Items> itemsAllowed, bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY)
    {
        //initialize the parameter values
        allowedItems = itemsAllowed;
    }
}
public abstract class ShapeInventory : Inventory
{
    public ShapeInventory(bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY)
    {
    }
    public bool ItemCanFit(int itemX, int itemY, GrabbedItem item, out GrabbedItem snappedToFit)
    {
        //ensure the item being held is shaped and not a stack item
        if(!(item.item is ShapeItem))
        {
            snappedToFit = item;
            return false;
        }
        //cast the grabbed item to a shape item
        bool[,] itemShape = ((ShapeItem)(item.item)).GetShape();
        //TODO snap the item to the grid and or snap it to a valid location in the inventory
        snappedToFit = item;
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
    Item item;
    public InventorySlot(bool valid)
    {
        isValid = valid;
        item = null;
    }
    public bool IsEmpty()
    {
        return isValid && item == null;
    }
    public bool IsValid()
    {
        return isValid;
    }
}