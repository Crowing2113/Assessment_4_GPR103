using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed;
    public float jumpHeight;

    public bool p1 = true;
    public bool p2 = false;
    public int hp;//hit points, how many times to get hit before die
    public int atk, def;
    public int lives = 3;
    public string[] playerAnim;

    private Vector3 scale;
    private Vector2 currentVelocity;
    SpriteRenderer sr; // for flipping the sprite when going left or right
    Rigidbody2D rb;//rigidbody reference
    Animator anim;
    private KeyCode moveLeft, moveRight, jump, attackKey;
    AnimationCycle animCycle;

    bool isAttacking = false;
    [SerializeField]
    bool grounded = true;
    [SerializeField]
    float groundedTimer = 0f;
    // Start is called before the first frame update

    enum AnimationCycle
    {
        IDLE = 0,
        WALK,
        JUMP,
        ATTACK
    }

    void Start()
    {
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
        }
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        sr = this.gameObject.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        currentVelocity = rb.velocity;
        scale = transform.localScale;

        groundedTimer += Time.deltaTime;
        if (lives <= 0)
        {
            //do something
        }
        if (Input.GetKey(moveLeft))//moving left
        {
            animCycle = AnimationCycle.WALK;
            currentVelocity.x = -speed;
            scale.x = -1;
        }
        else if (Input.GetKey(moveRight))//moving right
        {
            animCycle = AnimationCycle.WALK;
            currentVelocity.x = speed;
            scale.x = 1;
        }
        else
        {

            animCycle = AnimationCycle.IDLE;
            currentVelocity.x = 0;
        }
        if (Input.GetKey(jump))//can also jump while moving
        {
            if (grounded && groundedTimer > 0.5f)//checking that we're on a platform so we can jump
            {
                animCycle = AnimationCycle.JUMP;
                print("Jumping");
                grounded = false;
                currentVelocity.y = jumpHeight;
            }
        }
        if (Input.GetKey(attackKey) && !isAttacking)
        {
            StartCoroutine(Attack());
            Attack();

        }
        anim.Play(playerAnim[(int)animCycle]);
        rb.velocity = currentVelocity;
        transform.localScale = scale;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animCycle = AnimationCycle.ATTACK;
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        
        //Checking that the player is on a platform for jumping
        if (other.gameObject.CompareTag("Platform"))
        {
            Vector2 cp = other.ClosestPoint(transform.position);//get the contact point of the collision
            Vector2 center = other.bounds.center;//get the center of the collider
            if (cp.y > center.y)
            {
                print("Hit top");
                grounded = true;
                groundedTimer = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Vector2 cp = other.ClosestPoint(transform.position);//get the contact point of the collision
        Vector2 center = other.bounds.center;//get the center of the collider

        if (other.gameObject.CompareTag("Platform"))
        {
            if (cp.y > center.y)
            {
                print("leaving top");
                grounded = false;
            }
        }
    }



    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
    }
}