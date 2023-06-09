using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoomTrapFire : MonoBehaviour
{
    
    [Header("Direction|0-up|1-right|2-down|3-left")] [Range(0, 3)] [SerializeField]
    private int directionIndex;

    [Space(10)] 
    
    [SerializeField] private float shootInterval;
    [SerializeField] private int projectileSpeed;
    [SerializeField] private GameObject projectilePrefab;
    
    private Vector2 projectileDirection;
    
    void Start()
    {
        StartCoroutine(ProjectileCoroutine());
    }

    private IEnumerator ProjectileCoroutine()
    {
        while (true)
        {
            CalculateFireDirection(directionIndex);
            Fire();
            
            yield return new WaitForSeconds(shootInterval);

        }
    }
    
    
    

    private void CalculateFireDirection(int index)
    {
        switch (index)
        {
            case 0:
                projectileDirection = Vector2.up;
                break;
            case 1:
                projectileDirection = Vector2.right;
                break;
            case 2:
                projectileDirection = Vector2.down;
                break;
            case 3:
                projectileDirection = Vector2.left;
                break;
        }
    }

    private void Fire()
    {
        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb!=null)
        {
            rb.velocity = projectileDirection * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab doesnt have Rigidbody2D, check RoomTrapFire");
        }
    }
}
