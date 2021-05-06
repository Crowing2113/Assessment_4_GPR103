using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour
{
    //Public variables
    //int
    public int coinsDrop;//this is how score is calculated
    public int hp, startHp;//hp will be set to startHP in start()
    public int atk, def;//stats
    //float
    public float attackCooldown;//what the attack timer has to reach before the enemy can attack again
    public float speed;
    public float jumpHeight;
    public float attackRange, followRange;
    //string
    public string[] enemyAnim; //array of animation names, make sure they match
    //bool
    public bool isWalking = false;
    public bool ranged = false; //whether the enemy is a ranged unit
    //GameObjects
    public GameObject target;
    public GameObject hitBox;
    public GameObject coin;
    public GameObject proj = null; //projectile for ranged enemies, start of as null

    //Private variables
    //float
    private float yDist, xDist;
    public float attackTimer;//the attack timer will go up every frame
    //RigidBody
    private Rigidbody2D rb;
    //Vector2
    private Vector2 currVel;
    private Vector2 scale;
    //GameObjects
    private GameObject[] go;//array to store game objects in the scene, mainly used for getTarget
    //Animator
    private Animator anim;
    //Animation Cycle
    private AnimationCycle animCyc = 0;
    //SerializeFields (these will show up in the inspector despite being private
    [SerializeField]
    private bool grounded = true, isDead = false;
    [SerializeField]
    private float groundedTimer = 0.0f;



    GameManager gm;//gamemanager script reference
    enum AnimationCycle
    {
        IDLE = 0,
        WALK,
        JUMP,
        ATTACK,
        HIT,
        DIE
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = startHp;
        if (hitBox.activeSelf)
        {
            hitBox.SetActive(false);
        }
        speed = Random.Range(speed - 0.5f, speed + 0.5f);
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
            gm.quitGame();
        }
    }
    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;
        yDist = target.transform.position.y - this.transform.position.y;//get the distance between the two y positions
        xDist = target.transform.position.x - this.transform.position.x;//get the distance between the two x positions

        if (Mathf.Abs(xDist) > 0.5 || Mathf.Abs(yDist) > 0.5)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        scale = rb.transform.localScale;
        currVel = rb.velocity;
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        ///IF target is within range then attack
        if (!isDead)
        {
            if (distanceToTarget <= attackRange && attackTimer >= attackCooldown)
                Attack(target);
            else if (distanceToTarget <= followRange)
                followTarget(distanceToTarget);
            else if (!isDead)
                animCyc = AnimationCycle.IDLE;
        }
        if (hp <= 0)
            StartCoroutine(Die());
        anim.Play(enemyAnim[(int)animCyc]);

        groundedTimer += Time.deltaTime;
        rb.velocity = currVel;
        rb.transform.localScale = scale;
        if (anim.GetBool("hit"))
            anim.SetBool("hit", false);
    }

    //gets called once during start up and when it kills it's target to give it a player to go after
    void getTarget()
    {

        go = GameObject.FindGameObjectsWithTag("Player");
        //check if there is more than 1 player
        if (go.Length > 1)
        {
            //calculate the distance between each character
            float d1 = Vector2.Distance(go[0].transform.position, this.transform.position);
            float d2 = Vector2.Distance(go[1].transform.position, this.transform.position);
            //check which distance is smaller
            if (d1 < d2)
                target = go[0];//if distance 1 (Player 1 position) is smaller then player 1is the target
            else
                target = go[1];//if distance 2 (player 2 position) is smaller then player 2 is the target
        }
        else
            //if there is only 1 player the make player 1 the target
            target = go[0];
    }

    void followTarget(float targetDistance)
    {
        //if the y distance is greater than 0.1 (any less should be on the same level, then jump to get closer to the player, otherwise move along the x axis
        if (yDist > jumpHeight / 2 && grounded && (groundedTimer > 2.5f))
        {
            currVel.y = jumpHeight;
            grounded = false;
            groundedTimer = 0;
            animCyc = AnimationCycle.JUMP;
        }
        else
        {
            if (xDist > 0)
            {

                currVel.x = speed;
                scale.x = 1;
                animCyc = AnimationCycle.WALK;
            }
            else if (xDist < 0)
            {
                currVel.x = -speed;
                scale.x = -1;
                animCyc = AnimationCycle.WALK;
            }
            else
                currVel.x = 0;
        }
    }

    void Attack(GameObject target)
    {
        animCyc = AnimationCycle.ATTACK;
        if (ranged)
        {

            Transform t = hitBox.transform;
            GameObject go = Instantiate(proj, t.position, Quaternion.identity);
            
            //if the enemy is facing left then multiply the projectile's speed by -1
            if (transform.localScale.x == -1)
            {
                go.GetComponent<Projectile>().speed *= -1;
                go.GetComponent<Projectile>().pProj = false; ;

                go.transform.localScale = scale;
            }
            //set the damage of the projectile to this enemy's atk value
            go.GetComponent<Projectile>().dmg = atk;
            go.GetComponent<Projectile>().parent = this.gameObject;
        }
        if (target.GetComponent<playerController>().hp <= 0 || target == null)
            getTarget();
        attackTimer = 0;
    }

    IEnumerator Die()
    {
        animCyc = AnimationCycle.DIE;
        anim.SetBool("isDead", true);
        anim.SetBool("hit", false);
        anim.Play(enemyAnim[(int)animCyc]);
        yield return new WaitForSeconds(0.25f);
        //set the coin's parent to the stage
        //if this isn't done the coinc gets deleted along with this game object
        GameObject temp = Instantiate(coin, this.transform);
        temp.transform.parent = gm.stage.transform;
        gm.enemiesSpawned--;
        Destroy(this.gameObject);
    }

    /// <summary>
    /// This function is called from an outside attakcing source that collides with this object
    /// Calculates the total damage by subtracting the defense from the damage input
    /// </summary>
    public void TakeDamage(int dmg)
    {
        int tDmg = dmg - def;
        //this means we will always do at least 1 damage
        if (tDmg <= 0)
            tDmg = 1;
        hp -= tDmg;
        anim.SetBool("hit", true);
        animCyc = AnimationCycle.HIT;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Physics2D.IgnoreCollision(other.collider, this.GetComponent<Collider2D>(), true);
        }
        for (int i = 0; i < other.contacts.Length; i++)
        {
            if (Vector2.Dot(other.contacts[i].normal, Vector2.up) > 0.9f)
            {
                grounded = true;
            }
        }
    }
}
