using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : StackInventory
{
    public TestInventory(bool[,] shape, string inventoryName) : base(shape, new HashSet<Items> { Items.Stone, Items.MegaStoneTest, Items.Club, Items.Foliage}, inventoryName)
    {
    }
    public TestInventory(List<PersistentSlot> slots, int width, int height, string invenName) : base(slots, width, height, invenName, new HashSet<Items> { Items.Stone, Items.MegaStoneTest, Items.Club, Items.Foliage })
    {

    }
}
public class WeaponsInventory : StackInventory
{
    public WeaponsInventory(bool[,] shape, string inventoryName) : base(shape, new HashSet<Items> { Items.Club }, inventoryName)
    {

    }
    public WeaponsInventory(List<PersistentSlot> slots, int width, int height, string invenName) : base(slots, width, height, invenName, new HashSet<Items> { Items.Club })
    {

    }
}