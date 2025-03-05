using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bark : IItem
{
    public IItem Clone()
    {
        return new Bark();
    }

    public Items GetItemType()
    {
        return Items.Bark;
    }

    public int GetMaxStackCount()
    {
        return 5;
    }

    public Texture2D GetTexture()
    {
        throw new System.NotImplementedException();
    }
}
