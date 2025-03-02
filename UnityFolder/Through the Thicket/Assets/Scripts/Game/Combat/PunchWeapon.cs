using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PunchWeapon : Weapon
{
    public override IItem Clone()
    {
        return null;
    }

    public override Attack GetAttack()
    {
        return new BasicAttack(2);
    }

    public override Items GetItemType()
    {
        return Items.ErrorItem;
    }

    public override void PopulateSlot(VisualElement slot)
    {
        throw new System.NotImplementedException();
    }
}
