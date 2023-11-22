using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillingProp : MonoBehaviour
{

    public Transform player;
    private CharacterController2D cc2d;

    void Awake()
    {
        this.cc2d = player.GetComponent<CharacterController2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name == "Karna")
        {
            this.cc2d.TakeDamage();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Karna")
        {
            this.cc2d.TakeDamage();
        }
    }
}
