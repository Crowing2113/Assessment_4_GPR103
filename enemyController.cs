using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{

    //TODO:
    //check if a player character is within a certain range
    //make that player the target and follow

    public float speed;
    public float jumpHeight;
    public float attackRange, followRange;
    public bool isWalking = false;
    public int coinsDrop;//this is how score is calculated
    public int hp, startHp, atk, def;//stats, HP will be set at start
    public GameObject target;
    public GameObject hitBox;
    public string[] enemyAnim;
    private Rigidbody2D rb;
    private Vector2 currVel;

    [SerializeField]
    private bool grounded = true;
    [SerializeField]
    private float groundedTimer = 0.0f;
    [SerializeField]
    private float[] keepDistanceRand;
    GameObject[] go;//array to store game objects in the scene, mainly used for getTarget

    Animator anim;
    AnimationCycle animCyc = 0;

    GameManager gm;//gamemanager script reference
    enum AnimationCycle
    {
        IDLE = 0,
        WALK,
        JUMP,
        ATTACK
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = startHp;
        if (hitBox.activeSelf)
        {
            hitBox.SetActive(false);
        }
        keepDistanceRand = new float[3];
        for (int i = 0; i < keepDistanceRand.Length; i++)
        {
            keepDistanceRand[i] = Random.Range(0.5f, followRange);
        }
        anim = GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();//give it a reference to the GameManager
        rb = this.gameObject.GetComponent<Rigidbody2D>();//reference to rigidbody
        this.transform.parent = gm.stage.transform;
        getTarget();
    }
    private void FixedUpdate()
    {
        if (target == null && GameManager.playerCount > 0)
        {
            getTarget();
        }
        else if (GameManager.playerCount <= 0)
        {
            gm.quitButton();
        }
    }
    // Update is called once per frame
    void Update()
    {
        currVel = rb.velocity;
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        ///IF target is within range then attack

        if (distanceToTarget <= followRange)

            followTarget(distanceToTarget);


        if (distanceToTarget <= attackRange)
            Attack(target);
        else

            animCyc = AnimationCycle.IDLE;

        ///check if the target is within a certain range
        if (isWalking)
            animCyc = AnimationCycle.WALK;

        anim.Play(enemyAnim[(int)animCyc]);

        if (hp <= 0)
            Die();
        groundedTimer += Time.deltaTime;
        rb.velocity = currVel;
    }

    //gets called once during start up and when it kills it's target to give it a player to go after
    void getTarget()
    {
        //maybe a public reference to each character and check if there is one in the scene-
        go = GameObject.FindGameObjectsWithTag("Player");
        //check if there is more than 1 player
        if (go.Length > 1)
        {
            //calculate the distance between each character
            float d1 = Vector2.Distance(go[0].transform.position, this.transform.position);
            float d2 = Vector2.Distance(go[1].transform.position, this.transform.position);
            //check which distance is greater
            if (d1 > d2)
                target = go[0];//if distance 1 (Player 1 position) is greater then player 1is the target
            else
                target = go[1];//if distance 2 (player 2 position) is greater then player 2 is the target
        }
        else
            //if there is only 1 player the make player 1 the target
            target = go[0];

        print("Target is - " + target.name);
    }

    void followTarget(float targetDistance)
    {
        int i = Random.Range(0, keepDistanceRand.Length - 1);
        float yDist = target.transform.position.y - this.transform.position.y;//get the distance between the two y positions
        float xDist = target.transform.position.x - this.transform.position.x;//get the distance between the two x positions
        //if the y distance is greater than 0.1 (any less should be on the same level, then jump to get closer to the player, otherwise move along the x axis
        if (yDist > 0.5 && grounded && (groundedTimer > 2.5f))
        {
            currVel.y = jumpHeight;
            grounded = false;
            groundedTimer = 0;
        }

        else
        {
            if (xDist > keepDistanceRand[i])
            {
                currVel.x = speed;
                print("xDist: " + xDist + " and Velocity: " + currVel);
            }
            else if (xDist < -keepDistanceRand[i])
            {
                currVel.x = -speed;
                print("xDist: " + xDist + " and Velocity: " + currVel);
            }
            else
                currVel.x = 0;
        }
    }

    void Attack(GameObject target)
    {
        animCyc = AnimationCycle.ATTACK;
        if (target.GetComponent<playerController>().hp <= 0 || target == null)
            getTarget();
    }

    void Die()
    {
        gm.enemiesSpawned--;
        //anim.Play("death");
        Destroy(this.gameObject);
    }
    /// <summary>
    /// This function is called from an outside attakcing source that collides with this object
    /// Calculates the total damage by subtracting the defense from the damage input
    /// </summary>
    /// <param name="dmg"></param>
    public void TakeDamage(int dmg)
    {
        int tDmg = dmg - def;
        //this means we will always do at least 1 damage
        if (tDmg <= 0)
            tDmg = 1;
        hp -= tDmg;
        print(name + " took " + tDmg + " damage and has " + hp + " hp left");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        for (int i = 0; i < other.contacts.Length; i++)
        {
            if (Vector2.Dot(other.contacts[i].normal, Vector2.up) > 0.9f)
            {
                
                grounded = true;
            }
        }
    }
}
