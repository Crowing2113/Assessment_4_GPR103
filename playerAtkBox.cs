using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAtkBox : MonoBehaviour
{
    playerController parent;//reference to the player parent

    private void Start()
    {
        parent = (playerController)GetComponentInParent<playerController>();//gettign the parent's playerController class


    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            print(name + " is attaking " + other.name);
            other.GetComponent<enemyController>().TakeDamage(parent.atk);
        }
    }
}
