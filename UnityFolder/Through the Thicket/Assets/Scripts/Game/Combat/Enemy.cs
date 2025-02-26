using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Attack
{
    protected int damage;
    public Attack(int damage)
    {
        this.damage = damage;
    }
}
//This is used to store info about any fighter (player included)
public abstract class Fighter
{
    protected int health;
    public abstract string GetName();
    public abstract Texture2D GetTexture();

}
//this is for all enemies but not the player
public abstract class Enemy : Fighter
{
    protected List<Attack> attacks;
    public abstract string GetDescription();
}
//This is just used for providing combat related data for the player
public class PlayerFighter : Fighter
{
    public override string GetName()
    {
        return "Shrooman Herbank";
    }

    public override Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Player").texture;
    }
}
