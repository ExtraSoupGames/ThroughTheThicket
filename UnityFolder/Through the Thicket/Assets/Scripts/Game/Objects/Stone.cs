using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Stone : IItem
{
    public Items GetItemType()
    {
        return Items.Stone;
    }

    public void PopulateSlot(VisualElement slot)
    {
        throw new System.NotImplementedException();
    }
}
