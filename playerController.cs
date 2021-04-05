using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed;
    public float jumpHeight;

    private bool grounded = true;

    public bool p1 = true;
    public bool p2 = false;
    Rigidbody2D rb;//for handling rigidbody
    SpriteRenderer sr; // for flipping the sprite when going left or right
    PlayerBaseClass playerClass;

    KeyCode moveLeft,moveRight,jump;
    // Start is called before the first frame update
    void Start()
    {
        if (p1)
        {
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            jump = KeyCode.W;
        } else if (p2)
        {
            moveLeft = KeyCode.J;
            moveRight = KeyCode.L;
            jump = KeyCode.I;
        }
        playerClass = GetComponent<PlayerBaseClass>();
        speed = playerClass.GetSpeed();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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
            if (grounded)
            {
                print("Jumping");
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Vector2 cp = other.contacts[0].point;//get the contact point of the collision
        Vector2 center = other.collider.bounds.center;//get the center of the collider
        
        float pdc = other.gameObject.transform.position.magnitude / 2;//points distance from center
        if (other.gameObject.tag == "Platform")
        {
            if (cp.y > center.y )
            {
                print("Hit top");
                grounded = true;
            }
        }
    }
}