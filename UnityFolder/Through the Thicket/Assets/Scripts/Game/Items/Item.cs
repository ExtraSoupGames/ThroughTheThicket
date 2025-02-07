using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Item
{

}
public abstract class StackItem : Item
{
    int count;
    int maxStackCount;
    //bool returned is true if any items were moved
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
}
public abstract class ShapeItem : Item
{
    public abstract bool[,] GetShape();
}
