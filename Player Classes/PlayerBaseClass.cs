using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseClass : MonoBehaviour
{
    private int hp;
    private int atk;
    private int def;
    private float speed;
    private string type;

    // Start is called before the first frame update
    void Start()
    {
        hp = 0;
        atk = 0;
        def = 0;
        speed = 0;
        type = "";
    }

    //Calculate how much damage to take and take that from hp
    public void TakeDamage(int damage) {
        int tDmg = damage - def;
        if(tDmg <= 0)
            tDmg = 1;
        hp -= tDmg;
    }

    void Attack(int damage, GameObject target)
    {
        target.GetComponent<EnemyBaseClass>().TakeDamage(damage);
    }

    ///========SETTERS AND GETTERS============///
    public void SetHP(int hp)
    {
        this.hp = hp;
    }

    public void SetATK(int atk)
    {
        this.atk = atk;
    }

    public void SetDEF(int def)
    {
        this.def = def;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetPlayerType(string type)
    {
        this.type = type;
    }

    public int GetHP()
    {
        return hp;
    }

    public int GetATK()
    {
        return atk;
    }

    public int GetDEF()
    {
        return def;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public string GetPlayerType()
    {
        return type;
    }
}
