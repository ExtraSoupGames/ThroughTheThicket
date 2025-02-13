using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : StackInventory
{
    public TestInventory(bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY, new HashSet<Items> { Items.Stone})
    {
    }
}
public class TestShapeInventory : ShapeInventory
{
    public TestShapeInventory(bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY, new HashSet<Items> { Items.ShapeItem1 })
    {
    }

    public override void SwapItem(ref Item heldItem, InventorySlot hoveredSlot)
    {
        if(heldItem != null)
        {
            //if an item is held then insert it
            ShapeItem insertItem = heldItem as ShapeItem;
            int x = hoveredSlot.x;
            int y = hoveredSlot.y;
            bool[,] insertItemShape = insertItem.GetShape();
            for (int i = 0; i < insertItemShape.GetLength(0); i++)
            {
                for (int j = 0; j < insertItemShape.GetLength(1); j++)
                {
                    //no slot validation should be needed as this should all be already checked
                    if (insertItemShape[i, j])
                    {
                        GetSlot(x + i, y + j).item = insertItem;
                    }
                }
            }
            heldItem = null;
            return;
        }
        if (!hoveredSlot.IsEmpty())
        {
            // if no item is held and the hovered slot is filled, pick it up
            int x = hoveredSlot.x;
            int y = hoveredSlot.y;
            heldItem = hoveredSlot.item;
            bool[,] removeItemShape = (hoveredSlot.item as ShapeItem).GetShape();
            for (int i = 0; i < removeItemShape.GetLength(0); i++)
            {
                for (int j = 0; j < removeItemShape.GetLength(1); j++)
                {
                    //all slots filled by the item being removed need to be emptied
                    if (removeItemShape[i, j])
                    {
                        GetSlot(x + i, y + j).item = null;
                    }
                }
            }
        }
    }
}
