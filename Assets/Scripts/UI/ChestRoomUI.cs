using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRoomUI : MonoBehaviour
{
    private PlayerDetailsSO playerDetails;
    private RectTransform rectTransform;
    
    [Range(0, 2)] [SerializeField] private int buffWeaponIndex;
    
    
    void Awake()
    {
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        //playerDetails.startingWeaponList[1].weaponCurrentAmmo.ammoDamage+=100;
        rectTransform = GetComponent<RectTransform>();

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            rectTransform.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerDetails.startingWeaponList[buffWeaponIndex].weaponCurrentAmmo.ammoDamage += 5;
                Destroy(this.transform.parent.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
