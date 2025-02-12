using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public abstract Sprite GetSprite();
}
public class StackItem : Item
{
    int count;
    int maxStackCount;
    //bool returned is true if any items were moved
    public StackItem(Items item) : base(item)
    {

    }
    public bool AddToStack(StackItem inItem, out StackItem outItem)
    {
        count += inItem.count;
        if(count <= maxStackCount)
        {
            outItem = null;
            return true;
        }
        int oldCount = count;
        outItem = inItem;
        int overflow = (count - maxStackCount);
        count -= overflow;
        outItem.count = overflow;
        return count != oldCount;
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("TestSprite");
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
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("TestSprite");
    }
}
