using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : StackInventory
{
    public TestInventory(bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY, new HashSet<Items> { Items.Stone, Items.MegaStoneTest})
    {
    }
}