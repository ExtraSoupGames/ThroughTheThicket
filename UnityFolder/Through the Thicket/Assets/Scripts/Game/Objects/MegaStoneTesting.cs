using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MegaStoneTesting : IItem
{
    public IItem Clone()
    {
        return new MegaStoneTesting();
    }

    public Items GetItemType()
    {
        return Items.MegaStoneTest;
    }

    public int GetMaxStackCount()
    {
        return 2;
    }
    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Shaped01").texture;
    }
}
