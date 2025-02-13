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
public abstract class ShapeItem : Item
{
    public ShapeItem(Items item) : base(item)
    {

    }
    public abstract bool[,] GetShape();
}
public class TestShapeItem : ShapeItem
{
    public TestShapeItem(Items item) : base(item)
    {

    }
    public override bool[,] GetShape()
    {
        return new bool[,]
        {
            { true, false },
            { true, true }
        };
    }
    public override void PopulateSlot(VisualElement slot)
    {
        //TODO fix this lol
        VisualElement slotItem = new VisualElement();
        slotItem.AddToClassList("item-image");
        slotItem.style.backgroundImage = Resources.Load<Sprite>("TestSprite").texture;
        slot.Add(slotItem);
    }
}
