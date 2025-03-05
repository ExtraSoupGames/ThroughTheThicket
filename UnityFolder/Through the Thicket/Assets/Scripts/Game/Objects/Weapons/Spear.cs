using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class Spear : Weapon
{
    public override IItem Clone()
    {
        return new Spear();
    }

    public override Attack GetAttack()
    {
        return new BasicAttack(5);
    }

    public override Items GetItemType()
    {
        return Items.Spear;
    }

    public override Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Shaped00").texture;
    }
}
