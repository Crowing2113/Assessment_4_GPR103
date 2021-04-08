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
    public int lives = 3;
    SpriteRenderer sr; // for flipping the sprite when going left or right
    Rigidbody2D rb;//for handling rigidbody
    private KeyCode moveLeft, moveRight, jump;
    public bool grounded = true;
    // Start is called before the first frame update

    void Start()
    {
        //sets up controls depending if player 1 or 2 character
        if (p1)
        {
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            jump = KeyCode.W;
        }
        else if (p2)
        {
            moveLeft = KeyCode.J;
            moveRight = KeyCode.L;
            jump = KeyCode.I;
        }
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        if (lives <= 0)
        {
            //do something
        }
        if (Input.GetKey(moveLeft))//moving left
        {
            if (sr.flipX == true)
                sr.flipX = false;
            this.transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(moveRight))//moving right
        {
            if (sr.flipX == false)
                sr.flipX = true;
            this.transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(jump))//can also jump while moving
        {
            if (grounded)//checking that we're on a platform so we can jump
            {
                print("Jumping");
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Vector2 cp = other.ClosestPoint(transform.position);//get the contact point of the collision
        Vector2 center = other.bounds.center;//get the center of the collider

        //Checking that the player is on a platform for jumping
        if (other.gameObject.CompareTag("Platform"))
        {
            if (cp.y > center.y)
            {
                print("Hit top");
                grounded = true;
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