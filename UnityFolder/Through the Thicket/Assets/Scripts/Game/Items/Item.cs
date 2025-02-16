using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public abstract class Item
{
    protected Items itemType;
    public Item(Items item)
    {
        itemType = item;
    }
    public Items GetItemType()
    {
        return itemType;
    }
    public abstract void PopulateSlot(VisualElement slot);
}
public class StackItem : Item
{
    int count;
    int maxStackCount;
    //bool returned is true if any items were moved
    public StackItem(Items item) : base(item)
    {
        count = 1;
        maxStackCount = 5;
    }
    public bool AddToStack(ref StackItem inItem)
    {
        count += inItem.count;
        if(count <= maxStackCount)
        {
            //if the whole item fit inside the stack, the item will be put in and held item will be set to null
            inItem = null;
            return true;
        }
        //otherwise, remove the overflow and return it to the held item
        int oldCount = count;
        int overflow = (count - maxStackCount);
        count -= overflow;
        inItem.count = overflow;
        return count != oldCount;
    }

    public override void PopulateSlot(VisualElement slot)
    {
        //display the item
        VisualElement slotItem = new VisualElement();
        slotItem.pickingMode = PickingMode.Ignore;
        slotItem.AddToClassList("item-image");
        slotItem.style.backgroundImage = Resources.Load<Sprite>("TestSprite").texture;
        slot.Add(slotItem);
        if(count != 1)
        {
            Debug.Log("Trying to create count label");
            Label countLabel = new Label(count.ToString());
            countLabel.AddToClassList("count-label");
            slot.Add(countLabel);
        }
    }
}
public class ShapeItem : Item
{
    int sourceSlotX;
    int sourceSlotY;
    int offsetX;
    int offsetY;
    public ShapeItem(Items item, int offsetX, int offsetY) : base(item)
    {
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }
    public bool[,] GetShape()
    {
        switch (itemType)
        {
            case Items.ShapeItem1:
                return new bool[2, 2]
                {
                    {true, true },
                    {true, true }
                };
        }
        return null;
    }
    public override void PopulateSlot(VisualElement slot)
    {
        //display the item
        //TODO find relevant sprite for the section
        VisualElement slotItem = new VisualElement();
        slotItem.pickingMode = PickingMode.Ignore;
        slotItem.AddToClassList("item-image");
        //TODO update this to a way of finding whatever texture is needed
        string itemName = "Shaped" + offsetX.ToString() + offsetY.ToString();
        Debug.Log("Loading item with name: " + itemName);
        slotItem.style.backgroundImage = Resources.Load<Sprite>(itemName).texture;
        slot.Add(slotItem);
    }
    public void SetSourceSlot(int x, int y)
    {
        sourceSlotX = x;
        sourceSlotY = y;
    }
    public void GetSourceSLot(out int x, out int y)
    {
        x = sourceSlotX;
        y = sourceSlotY;
    }
}
