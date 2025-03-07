using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public interface IItem
{
    public Items GetItemType();
    public int GetMaxStackCount();
    public IItem Clone();
    public Texture2D GetTexture();
}
public class StackItem
{
    IItem item;
    int count;
    int maxStackCount;
    //bool returned is true if any items were moved
    public StackItem(IItem item, int count)
    {
        this.item = item;
        this.count = count;
        maxStackCount = item.GetMaxStackCount();
    }
    public static StackItem CopyStackItem(StackItem copyFrom)
    {
        if(copyFrom == null)
        {
            return null;
        }
        return new StackItem(copyFrom.item, copyFrom.count);
    }
    public StackItem(IItem item) : this(item, 1)
    {
    }
    public StackItem(PersistentSlot data)
    {
        item = ItemHelper.GetItemFromType(data.itemType);
        count = data.count;
        maxStackCount = item.GetMaxStackCount();
    }
    public Items GetItemType()
    {
        return item.GetItemType();
    }
    public IItem GetClonedItem()
    {
        return item.Clone();
    }
    public int GetCount()
    {
        return count;
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

    public void PopulateSlot(VisualElement slot)
    {
        //display the item
        VisualElement slotItem = new VisualElement();
        slotItem.pickingMode = PickingMode.Ignore;
        slotItem.AddToClassList("item-image");
        slotItem.style.backgroundImage = item.GetTexture();
        slot.Add(slotItem);
        if(count != 1)
        {
            Label countLabel = new Label(count.ToString());
            countLabel.AddToClassList("count-label");
            slot.Add(countLabel);
        }
    }
    public bool Remove(int removalCount = 1)
    {
        count -= removalCount;
        return count <= 0;
    }
}