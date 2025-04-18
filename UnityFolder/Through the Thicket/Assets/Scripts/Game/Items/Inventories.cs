using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : StackInventory
{
    public TestInventory(bool[,] shape, string inventoryName) : base(shape, ItemHelper.AllItemSet(), inventoryName)
    {
    }
    public TestInventory(List<PersistentSlot> slots, int width, int height, string invenName) : base(slots, width, height, invenName, ItemHelper.AllItemSet())
    {

    }
}
public class WeaponsInventory : StackInventory
{
    public WeaponsInventory(bool[,] shape, string inventoryName) : base(shape, ItemHelper.WeaponItemSet(), inventoryName)
    {

    }
    public WeaponsInventory(List<PersistentSlot> slots, int width, int height, string invenName) : base(slots, width, height, invenName, ItemHelper.WeaponItemSet())
    {

    }
}