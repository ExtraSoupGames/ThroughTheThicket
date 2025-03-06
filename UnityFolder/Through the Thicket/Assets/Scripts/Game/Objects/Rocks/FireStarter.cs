using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FireStarter : IItem
{
    public IItem Clone()
    {
        return new FireStarter();
    }

    public Items GetItemType()
    {
        return Items.FireStarter;
    }

    public int GetMaxStackCount()
    {
        return 10;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("TestSprite").texture;
    }
}
