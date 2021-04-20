using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{

    //TODO:
    //Write function to locate nearest player
    //go after that player and attack

    public float speed;
    public float jumpHeight;

    public int coinsDrop;//this is how score is calculated
    public int hp, atk, def;
    GameObject[] go;
    public GameObject target;
    
    GameManager gm;//gamemanager script reference

    //gets called once during start up and when it kills it's target to give it a player to go after
    void getTarget()
    {
        //maybe a public reference to each character and check if there is one in the scene-
        go = GameObject.FindGameObjectsWithTag("Player");
        print(go.Length);
        //check if there is more than 1 player
        if (go.Length > 1)
        {
            //calculate the distance between each character
            float d1 = Vector2.Distance(go[0].transform.position, this.transform.position);
            float d2 = Vector2.Distance(go[1].transform.position, this.transform.position);
            //check which distance is greater
            if (d1 > d2)
            {
                target = go[0];//if distance 1 (Player 1 position) is greater then player 1is the target
            }
            else
                target = go[1];//if distance 2 (player 2 position) is greater then player 2 is the target
        }
        else
        {
            //if there is only 1 player the make player 1 the target
            target = go[0];
        }
        print("Target is - "+target.name);
}

    void attack(GameObject target)
    {
        ///TODO:
        ///do damage to target
        ///check if the target is dead
        ///     If so then get a new target otherwise keep going
    }
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();//give it a reference to the GameManager
        this.transform.parent = gm.stage.transform;
        getTarget();
    }

    void Die()
    {
        gm.enemiesSpawned--;
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {


        ///check if the target is within a certain range
        ///depending if this enemy is ranged or melee
        ///IF target is within range then attack


        if (hp <= 0)
        {

            Die();
        }


    }

}
