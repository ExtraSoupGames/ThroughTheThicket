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
        return new BasicAttack(4);
    }

    public override Items GetItemType()
    {
        return Items.Club;
    }

    public override Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Shaped00").texture;
    }
}
