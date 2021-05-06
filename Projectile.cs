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
        anim.Play(pAnim);
        Vector2 cVel = rb.velocity;
        cVel.x = speed;
        rb.velocity = cVel;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject go = other.gameObject;
        if (go.Equals(parent))
        {
            return;
        }
        if (go.CompareTag("Enemy")){
            if (pProj)
            {
                go.GetComponent<enemyController>().TakeDamage(dmg);
            }
            else
            {
                Physics2D.IgnoreCollision(other.collider, other.otherCollider,true);
            }
        }
        if (go.CompareTag("Player"))
        {
            if (!pProj)
                go.GetComponent<playerController>().TakeDamage(dmg);
        }
        Destroy(this.gameObject);
    }
}
