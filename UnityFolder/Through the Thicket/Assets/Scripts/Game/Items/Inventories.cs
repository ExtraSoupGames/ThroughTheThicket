using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : StackInventory
{
    public TestInventory(bool[,] shape, int topLeftX, int topLeftY, string inventoryName) : base(shape, topLeftX, topLeftY, new HashSet<Items> { Items.Stone, Items.MegaStoneTest}, inventoryName)
    {
    }
}