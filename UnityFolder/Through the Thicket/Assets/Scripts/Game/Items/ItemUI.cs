using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbedItem
{
    public Item item;
    public int X;
    public int Y;
    public GrabbedItem(Item itemGrabbed)
    {
        item = itemGrabbed;
    }
}
public class UIItemManipulator
{
    UIHolder UIHolder;
    RectTransform canvas;
    Camera cam;
    GrabbedItem heldItem;
    private void GetMousePos(out int mouseX, out int mouseY)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        //we pass null as the camera because the canvas uses overlay mode
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, mousePosition, null, out Vector2 canvasSpaceMousePos);
        mouseX = (int)canvasSpaceMousePos.x;
        mouseY = (int)canvasSpaceMousePos.y;
    }
    public void OnClick()
    {
        Inventory hoveredInventory;
        InventorySlot hoveredSlot = GetHoveredElement(out hoveredInventory);
        if(hoveredSlot == null || hoveredInventory == null)
        {
            return;
        }
        hoveredInventory.ClickAt(ref heldItem, hoveredSlot);
    }
    public InventorySlot GetHoveredElement(out Inventory slotSource) 
    {
        GetMousePos(out int mouseX, out int mouseY);
        foreach(Inventory inventory in UIHolder.inventories)
        {
            inventory.ScreenCoordsToInventory(mouseX, mouseY, out int invenX, out int invenY);
            int slotX = Mathf.FloorToInt(invenX / inventory.SlotSize());
            int slotY = Mathf.FloorToInt(invenY / inventory.SlotSize());
            if (inventory.ContainsSlotAt(slotX, slotY))
            {
                slotSource = inventory;
                return inventory.GetSlot(slotX , slotY);
            }
        }
        slotSource = null;
        return null;
    }

    public void ApplyUIHolder(UIHolder holder, RectTransform canvasRect, Camera camera)
    {
        UIHolder = holder;
        canvas = canvasRect;
        cam = camera;
        heldItem = null;
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
            }, 100, 100
         ));
        inventories.Add(new TestInventory(
        new bool[,]
        {
                    {true, true },
                    {true, false }
        }, 100, 200
         ));
    }
}