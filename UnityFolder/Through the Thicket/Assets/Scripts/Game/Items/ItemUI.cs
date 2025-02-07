using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GrabbedItem
{
    public Item item;
    public int X;
    public int Y;
}
public class UIItemManipulator
{
    UIHolder UIHolder;
    private void GetMousePos(out int mouseX, out int mouseY)
    {
        mouseX = 5;
        mouseY = 5;
    }
    public void OnClick()
    {

    }
    public InventorySlot GetHoveredElement() 
    {
        GetMousePos(out int mouseX, out int mouseY);
        foreach(Inventory inventory in UIHolder.inventories)
        {
            inventory.ScreenCoordsToInventory(mouseX, mouseY, out int invenX, out int invenY);
            if(inventory.ContainsSlotAt(invenX / inventory.SlotSize(), invenY / inventory.SlotSize()))
            {
                return inventory.GetSlot(invenX / inventory.SlotSize(), invenY / inventory.SlotSize());
            }
        }
        return null;
    }
}
public class UIHolder
{
    public List<Inventory> inventories;
    public UIHolder()
    {
        inventories = new List<Inventory>();
        inventories.Add(new TestInventory(
            new bool[,]
            {
                {true, false },
                {true, false }
            }, 15, 15
         ));
    }
}