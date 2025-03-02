using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class Club : Weapon
{
    public override IItem Clone()
    {
        return new Club();
    }

    public override Attack GetAttack()
    {
        return new BasicAttack(3);
    }

    public override Items GetItemType()
    {
        return Items.Club;
    }

    public override void PopulateSlot(VisualElement slot)
    {
        throw new System.NotImplementedException();
    }
}
