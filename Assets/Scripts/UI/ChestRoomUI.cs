using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestRoomUI : MonoBehaviour
{
    private PlayerDetailsSO playerDetails;
    private RectTransform rectTransform;
    private bool _canApply;
    
    
    [Header("0->Pistol/1->AK47,2->Shotgun")]
    [Range(0, 2)] [SerializeField] private int buffWeaponIndex;
    [Space(10)]
    [Header("0->AttackRate/1->AttackDamage/2->RangeBuff")]
    [Space(10)]
    [Range(0, 2)] [SerializeField] private int buffTypeIndex;

    [Header("Buff amounts")] [Space(10)] 
    [Range(0,10)]
    [SerializeField] private float amountOfAttackRateBuff;
    [Range(0,10)]
    [SerializeField] private int amountOfAttackDamageBuff;
    [Range(0, 10)] 
    [SerializeField] private int amountOfRangeBuff;
    
    
    void Awake()
    {
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        //playerDetails.startingWeaponList[1].weaponCurrentAmmo.ammoDamage+=100;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && _canApply)
        {
            Debug.Log("pressed e in trigger");
            ApplyBuff(buffTypeIndex,buffWeaponIndex,amountOfAttackRateBuff,amountOfAttackDamageBuff,amountOfRangeBuff);
            Destroy(this.transform.parent.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            //rectTransform.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            rectTransform.transform.localScale = Vector3.Lerp(rectTransform.transform.localScale,
                new Vector3(1.2f, 1.2f, 1f), 1 * Time.deltaTime);
            _canApply = true;
            Debug.Log(_canApply);

            /*
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("pressed e in trigger");
                ApplyBuff(buffTypeIndex,buffWeaponIndex,amountOfAttackRateBuff,amountOfAttackDamageBuff,amountOfRangeBuff);
                Destroy(this.transform.parent.gameObject);
            }
            */

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            rectTransform.transform.localScale = Vector3.Lerp(rectTransform.transform.localScale,
                new Vector3(1f, 1f, 1f), 1 * Time.deltaTime);
            _canApply = false;
            Debug.Log(_canApply);
        }
    }

    private void ApplyBuff(int buffType, int weaponType, float buffRateAmount, int buffDamageAmount, int rangeAmount)
    {
        
        switch (buffType)
        {
            case 0:
                playerDetails.startingWeaponList[weaponType].weaponFireRate -= buffRateAmount;
                Debug.Log("case 0 applied");
                break;
            case 1:
                playerDetails.startingWeaponList[weaponType].weaponCurrentAmmo.ammoDamage += buffDamageAmount;
                Debug.Log("case 1 applied");
                break;
            case 2:
                playerDetails.startingWeaponList[weaponType].weaponCurrentAmmo.ammoRange += rangeAmount;
                Debug.Log("case 2 applied");
                break;
        }
    }
}
