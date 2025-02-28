using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemHelper
{
    public static IItem GetItemFromType(Items itemType)
    {
        switch (itemType)
        {
            case Items.Stone:
                return new Stone();
            case Items.MegaStoneTest:
                return new MegaStoneTesting();
            case Items.ErrorItem:
                return null;
            default:
                Debug.Log("Item found in enum but not in helper, add me to this method");
                return null;
        }
    }
}
