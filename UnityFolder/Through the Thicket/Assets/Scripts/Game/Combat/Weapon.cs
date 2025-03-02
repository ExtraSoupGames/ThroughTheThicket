using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Weapon : IItem
{
    public abstract IItem Clone();

    public abstract Items GetItemType();

    public int GetMaxStackCount()
    {
        return 1;
    }

    public abstract void PopulateSlot(VisualElement slot);
    public abstract Attack GetAttack();
}
