using UnityEngine;
using System.Collections;

public abstract class Abstract_ALL  
{
    private int hp;
    private Vector3 destination;
    public bool isbroken = false;
    public int HP 
    {
        get { return hp; }
        set
        {
            hp = value;
            if (HP <= 0)
            {
                isbroken = true;
            }
        }
    }
    public Vector3 Destination {
        get { return destination; }
        set { destination = value; }

    }
    public abstract void BeHited(int damage);
}
public class ScenceGate : Abstract_ALL 
{
    //public FirstGate(int maxhp, Vector3 des)
    //{
    //    HP = maxhp;
    //    Destination = des;
    //}
    public override void BeHited(int damage)
    {
      //  Debug.Log("第一道门减血"+damage+"剩余血量"+HP);
        HP -= damage;
    }
}

public abstract class Person 
{
    private string name;
    private int hp;
    public string NAME 
    {
        set { name = value; }
        get { return name; }
    }
    public int HP 
    {
        set { hp = value; }
        get { return hp; }
    }
    protected Weapon weapon;
    public void SetWeapon(Weapon _weapon) 
    {
        this.weapon = _weapon;
    }

}

public class Pioneer : Person 
{
    public Pioneer(int hp, string name)
    {
        this.HP = hp;
        this.NAME = name;
    }
}


public abstract class Weapon 
{

    protected Bullet bullet;

}

public class OldBow:Weapon
{
    //public void sho
}

public abstract class Bullet 
{
    protected int damage;
}
public class NormalArrow:Bullet
{

}
public class FireArrow:Bullet
{

}
public class IceArrow : Bullet
{

}