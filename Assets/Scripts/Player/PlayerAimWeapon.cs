using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAimWeapon : MonoBehaviour
{
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }
    
    private Transform aimTransform;
    private Transform aimGunEndPointTransform;
    private PlayerController playerController;
    private Vector3 mousePosition;

    public GameObject bullet;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;
    

    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        playerController = gameObject.GetComponent<PlayerController>();
        aimGunEndPointTransform = aimTransform.Find("GunEndPosition");

    }

    private void Update()
    {
        HandleAiming();
        HandleShooting();


        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer>timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
        if (Input.GetMouseButton(0) && canFire)
        {
            canFire = false;
            Instantiate(bullet, aimGunEndPointTransform.position, Quaternion.identity);
        }


    }

    private void HandleAiming()
    {
        mousePosition = playerController.mouseWorldPosition;

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        Vector3 aimLocalScale = Vector3.one;
        if (angle>90||angle<-90)
        {
            aimLocalScale.y = -1f;
        }
        else
        {
            aimLocalScale.y = +1f;
        }

        aimTransform.localScale = aimLocalScale;


    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //shoot animation
            mousePosition = playerController.mouseWorldPosition;

            OnShoot?.Invoke(this,new OnShootEventArgs
            {
                gunEndPointPosition = aimGunEndPointTransform.position,
                shootPosition = mousePosition,
                
            });
        }
    }
    
}
