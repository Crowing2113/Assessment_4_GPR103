using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseClass : MonoBehaviour
{
    protected int hp;
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

    void Attack(int damage,GameObject target)
    {
        target.GetComponent<PlayerBaseClass>().TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        int calcDmg = damage - def;
        if(calcDmg < 0)
            calcDmg = 0;
        hp -= calcDmg;
    }

    /// ============SETTERS AND GETTERS==========    

    public void SetHP(int hp)
    {
        this.hp = hp;
    }
    public void SetAtk(int atk)
    {
        this.atk = atk;
    }
    public void SetDef(int def)
    {
        this.def = def;
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    
    public int GetHP()
    {
        return hp;
    }
    public int GetAtk()
    {
        return atk;
    }
    public int GetDef()
    {
        return def;
    }
    public string GetEnemyType()
    {
        return type;
    }
}
