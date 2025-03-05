using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemHelper
{
    public static IItem GetItemFromType(Items itemType)
    {
        switch (itemType)
        {
            case Items.Rock:
                return new Rock();
            case Items.Pebble:
                return new Pebble();
            case Items.ErrorItem:
                return null;
            case Items.Club:
                return new Club();
            case Items.Twigs:
                return new Twigs();
            case Items.Chanterelle:
                return new Chanterelle();
            case Items.Morel:
                return new Morel();
            case Items.Redcap:
                return new Redcap();
            case Items.Portobello:
                return new Portobello();
            case Items.Bark:
                return new Bark();
            case Items.Spear:
                return new Spear();
            case Items.Flint:
                return new Flint();
            case Items.Potato:
                return new Potato();
            case Items.Carrot:
                return new Carrot();

            default:
                Debug.Log("Item found in enum but not in helper, add me to this method");
                return null;
        }
    }
    public static HashSet<Items> AllItemSet()
    {
        return new HashSet<Items> { Items.Rock, Items.Pebble, Items.Club, Items.Twigs, Items.Chanterelle, Items.Morel, Items.Redcap, Items.Portobello, Items.Club, Items.Spear, Items.Bark, Items.Flint, Items.Potato, Items.Carrot};
    }
    public static HashSet<Items> WeaponItemSet()
    {
        return new HashSet<Items> { Items.Club, Items.Spear};
    }
}
