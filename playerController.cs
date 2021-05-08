using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public float animWaitTime;
    public float attackReset;

    public bool p1 = true;
    public bool p2 = false;
    public bool ranged;
    public int hp, startHp;//current hp and starting HP, this will set the HP when respawning
    public int atk, def;
    public int lives = 3;
    public string[] playerAnim;//array of animations to be set in Inspector NOTE: Make sure the names match the animation names
    public GameObject atkHitBox;
    public GameObject spawn;
    public GameObject proj = null; //projectile for ranged attack (if character is a ranged character

    private KeyCode moveLeft, moveRight, jump, attackKey;
    private Vector3 scale;
    private Vector2 currentVelocity;
    private SpriteRenderer sr; // for flipping the sprite when going left or right
    private Rigidbody2D rb;//rigidbody reference
    private Animator anim;
    private AnimationCycle animCycle; //to control the animation cycle (i.e. when walking will set this to the walking animation)
    [SerializeField]
    private float attackCooldown;

    bool isAttacking = false;
    [SerializeField]
    bool grounded = true;
    [SerializeField]
    float groundedTimer = 0f;
    [SerializeField]
    bool isJumping = false;
    // Start is called before the first frame update

    enum AnimationCycle //animation cycle enum, used to control animations
    {
        IDLE_R = 0,
        IDLE_L,
        WALK_L,
        WALK_R,
        JUMP,
        ATTACK,
        HIT,
        DIE
    }


    void Start()
    {
        attackCooldown = attackReset;
        hp = startHp;
        if (atkHitBox.activeInHierarchy)
            atkHitBox.SetActive(false);
        //sets up controls depending if player 1 or 2 character
        if (p1)
        {
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            jump = KeyCode.W;
            attackKey = KeyCode.S;
        }
        else if (p2)
        {
            moveLeft = KeyCode.J;
            moveRight = KeyCode.L;
            jump = KeyCode.I;
            attackKey = KeyCode.K;
        }

        rb = this.gameObject.GetComponent<Rigidbody2D>();
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentVelocity = rb.velocity;
        scale = transform.localScale;
        attackCooldown += Time.deltaTime;
        groundedTimer += Time.deltaTime;

        if (Input.GetKey(moveLeft))//moving left
        {
            //show walking animation only when the playre is not jumping and is not attacking
            if (!isJumping && !isAttacking)
            {
                animCycle = AnimationCycle.WALK_L;
                SetAnimBool(false, true, false, isAttacking, isJumping);
            }
            currentVelocity.x = -speed;
            scale.x = -1;
        }
        else if (Input.GetKey(moveRight))//moving right
        {
            //show walking animation only when the playre is not jumping and is not attacking
            if (!isJumping && !isAttacking)
            {
                SetAnimBool(false, false, true, isAttacking, isJumping);
                animCycle = AnimationCycle.WALK_R;
            }
            currentVelocity.x = speed;
            scale.x = 1;
        }
        else if (!isJumping && !isAttacking)//if player is not attacking, jumping or moving then change animation to idle
        {
            //checking which way the player is facing to display the right or left facing idle
            if (scale.x == 1)
            {
                anim.SetBool("facingRight", true);
                animCycle = AnimationCycle.IDLE_R;
            }
            else
            {
                animCycle = AnimationCycle.IDLE_L;
                anim.SetBool("facingRight", false);
            }
            SetAnimBool(true, false, false, false, false);
            currentVelocity.x = 0;
        }

        if (Input.GetKey(jump) && !isJumping)//can also jump while moving
        {
            if (grounded && groundedTimer > 0.5f)//checking that we're on a platform so we can jump
            {
                animCycle = AnimationCycle.JUMP;
                isJumping = true;
                grounded = false;
                SetAnimBool(false, false, false, isAttacking, isJumping);
                currentVelocity.y = jumpHeight;
            }
        }
        //checking input for attacking
        if (Input.GetKeyDown(attackKey) && !isAttacking && attackCooldown >= attackReset)
        {
            StartCoroutine(Attack());
        }


        anim.Play(playerAnim[(int)animCycle]); //plays what animCycle has been set to
        rb.velocity = currentVelocity;
        transform.localScale = scale;

    }
    IEnumerator Attack()
    {
        isAttacking = true;
        currentVelocity.x = 0;
        SetAnimBool(false, false, false, isAttacking, isJumping);
        animCycle = AnimationCycle.ATTACK;
        yield return new WaitForSeconds(animWaitTime);
        if (ranged)
        {
            Transform t = atkHitBox.transform;
            GameObject go = Instantiate(proj, t.position, Quaternion.identity);
            //if the character is facing left then multiply the projectile speed by -1
            if (transform.localScale.x == -1)
            {
                go.GetComponent<Projectile>().speed *= -1;
            }
            go.GetComponent<Projectile>().dmg = atk;
            go.GetComponent<Projectile>().pProj = true;
            go.GetComponent<Projectile>().parent = this.gameObject;
            //yield return new WaitForSeconds(attackSpeed*2);
        }
        attackCooldown = 0;
        isAttacking = false;
    }
    
    void Die()
    {
        animCycle = AnimationCycle.DIE;
        anim.SetBool("isDead",true);
        SetAnimBool(false, false, false, false, false);
        GameManager.playerCount--;
        GameManager.SetPlayerActive(this.gameObject, false);   
    }
    
    void CheckHP()
    {
        if (hp <= 0)
        {
            if (lives <= 0)
            {
                print(name + " has died");
                Die();
            }
            else
            {
                lives--;
                animCycle = AnimationCycle.DIE;
                Respawn();
            }

        }

    }
    void Respawn()
    {
        //set new transform
        transform.position = spawn.transform.position;
        //set health
        hp = startHp;
    }
    void SetAnimBool(bool idle, bool walkL, bool walkR, bool attack, bool jump)
    {
        anim.SetBool("isIdle", idle);
        anim.SetBool("isWalkingL", walkL);
        anim.SetBool("isWalkingR", walkR);
        anim.SetBool("isAttacking", attack);
        anim.SetBool("isJumping", jump);
    }


    public void TakeDamage(int atk)
    {
        currentVelocity.x = 0;
        int tDmg = atk - def;
        if (tDmg <= 0)
            tDmg = 1;
        hp -= tDmg;        
        CheckHP();
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        //for handling walking off platform so player can't jump in mid air 
        if (other.gameObject.CompareTag("Platform"))
        {
            grounded = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }

        for (int i = 0; i < other.contacts.Length; i++)
        {
            if (Vector2.Dot(other.contacts[i].normal, Vector2.up) > 0.9f)
            {
                anim.SetBool("isIdle", true);
                anim.SetBool("isJumping", false);
                grounded = true;
                isJumping = false;
                groundedTimer = 0;
            }
            if (other.gameObject.CompareTag("Platform") && (Vector2.Dot(other.contacts[i].normal, Vector2.left) > 0.75f || Vector2.Dot(other.contacts[i].normal, Vector2.right) > 0.75f))
            {
                currentVelocity.x = 0;
                currentVelocity.y = -1;
            }
        }
    }
}