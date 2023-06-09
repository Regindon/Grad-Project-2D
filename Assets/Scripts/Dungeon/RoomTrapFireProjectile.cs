using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrapFireProjectile : MonoBehaviour
{

    [SerializeField] private int projectileDamage;
    [SerializeField] private float immunityTime;
    private bool isColliding = false;
    private Collider2D gameObjectCollider;

    private void Awake()
    {
        gameObjectCollider = GetComponent<Collider2D>();
        gameObjectCollider.enabled = false;
    }

    private void Start()
    {

        StartCoroutine(ImmunityFirstSecond(immunityTime));

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isColliding) return;
        DealDamage(other);
        Destroy(gameObject);
    }


    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();

        if (health != null)
        {
            isColliding = true;
            health.TakeDamage(projectileDamage);
            
        }

    }

    private IEnumerator ImmunityFirstSecond(float immunTime)
    {
        yield return new WaitForSeconds(immunTime);
        gameObjectCollider.enabled = true;
        Debug.Log("immunity disabled");

    }
}
