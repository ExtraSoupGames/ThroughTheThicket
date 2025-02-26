using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SquirrelAttack : Attack
{
    public SquirrelAttack() : base(3)
    {
    }
}
public class Squirrel : Enemy
{
    public Squirrel()
    {
        health = 10;
        attacks = new List<Attack> { new SquirrelAttack() };
    }

    public override string GetDescription()
    {
        return "A bushy fiend, Watch out!";
    }

    public override string GetName()
    {
        return "Squirrel";
    }

    public override Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Squirrel").texture;
    }
}
