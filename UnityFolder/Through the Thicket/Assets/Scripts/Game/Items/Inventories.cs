using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInventory : ShapeInventory
{
    public TestInventory(bool[,] shape, int topLeftX, int topLeftY) : base(shape, topLeftX, topLeftY)
    {
    }
}
