using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public float speed;
    public int dmg;
    public float decayTime = 5;
    public string pAnim; //name of the animation for the projectile
    Rigidbody2D rb;
    Animator anim;
    public bool pProj = false;
    public GameObject parent;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        decayTime -= Time.deltaTime;
        if (decayTime <= 0)
            Destroy(this.gameObject);
        anim.Play(pAnim);
        Vector2 cVel = rb.velocity;
        cVel.x = speed;
        rb.velocity = cVel;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject go = other.gameObject;
        //if the tag of the collided object is the same as the parent (I.e if the parent is an enemt and it hits and enemy)
        //then ignore the collision
        if (parent != null && go.CompareTag(parent.tag))
        {
            Physics2D.IgnoreCollision(other, parent.GetComponent<Collider2D>(), true);
            return;
        }
        if (go.CompareTag("Enemy"))
        {
            if (pProj)
            {
                go.GetComponent<enemyController>().TakeDamage(dmg);
                Destroy(this.gameObject);
            }
        }
        if (go.CompareTag("Player"))
        {
            if (!pProj)
            {
                go.GetComponent<playerController>().TakeDamage(dmg);
                Destroy(this.gameObject);
            }
        }
        else
        {
            Physics2D.IgnoreCollision(other, this.GetComponent<Collider2D>(), true);
        }
    }
}
