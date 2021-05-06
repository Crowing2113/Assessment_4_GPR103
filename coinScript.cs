using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinScript : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    int scoreVal;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        anim.Play("coin_spin");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.score += scoreVal;
            Destroy(this.gameObject);
        }
    }
}
