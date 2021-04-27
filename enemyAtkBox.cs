using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAtkBox : MonoBehaviour
{
    enemyController parent; //reference to the enemy parent

    private void Start()
    {
        parent = (enemyController)GetComponentInParent<enemyController>();


    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.GetComponent<playerController>().TakeDamage(parent.atk);
        }
    }
}
