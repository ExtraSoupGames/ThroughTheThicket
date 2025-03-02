using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : StackInventory
{
    public TestInventory(bool[,] shape, int topLeftX, int topLeftY, string inventoryName) : base(shape, topLeftX, topLeftY, new HashSet<Items> { Items.Stone, Items.MegaStoneTest, Items.Club}, inventoryName)
    {
    }
    public TestInventory(List<PersistentSlot> slots, int width, int height, string invenName) : base(slots, width, height, invenName, new HashSet<Items> { Items.Stone, Items.MegaStoneTest, Items.Club })
    {

    }
}
public class WeaponsInventory : StackInventory
{
    public WeaponsInventory(bool[,] shape, int topLeftX, int topLeftY, string inventoryName) : base(shape, topLeftX, topLeftY, new HashSet<Items> { Items.Club }, inventoryName)
    {

    }
    public WeaponsInventory(List<PersistentSlot> slots, int width, int height, string invenName) : base(slots, width, height, invenName, new HashSet<Items> { Items.Club })
    {

    }
}