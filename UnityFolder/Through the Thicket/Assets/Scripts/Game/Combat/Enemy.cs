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
    public int GetDamage()
    {
        return damage;
    }
}
public class BasicAttack : Attack
{
    public BasicAttack(int damage) : base(damage) { }
}
//This is used to store info about any fighter (player included)
public abstract class Fighter
{
    protected int health;
    public abstract string GetName();
    public abstract Texture2D GetTexture();
    public void Damage(int damage)
    {
        health -= damage;
    }
    public bool IsDead()
    {
        return health < 1;
    }

}
//this is for all enemies but not the player
public abstract class Enemy : Fighter
{
    protected List<Attack> attacks;
    public abstract string GetDescription();
    public abstract string GetVoiceLine();
}
//This is just used for providing combat related data for the player
public class PlayerFighter : Fighter
{
    public PlayerFighter()
    {
        health = 15;
    }
    public override string GetName()
    {
        return "Shrooman Herbank HP:" + health;
    }

    public override Texture2D GetTexture()
    {
        return Resources.Load<Sprite>("Player").texture;
    }
    public int GetDamage()
    {
        return 3;
    }
}
