using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Stone : IItem
{
    public IItem Clone()
    {
        return new Stone();
    }

    public Items GetItemType()
    {
        return Items.Stone;
    }

    public int GetMaxStackCount()
    {
        return 5;
    }

    public Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("TestSprite").texture;
    }
}
