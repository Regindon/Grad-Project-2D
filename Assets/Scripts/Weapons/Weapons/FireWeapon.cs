using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float firePreChargeTimer = 0f;
    private float fireRateCoolDownTimer = 0f;
    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private void Awake()
    {
       
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
    }

    private void OnEnable()
    {
        //subscribe to fire weapon event
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        //unsubscribe from fire weapon event
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        //decrease cooldown timer
        fireRateCoolDownTimer -= Time.deltaTime;
    }

    
    //handle fire weapon event
    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEventArgs);
    }

   
    //fire weapon
    private void WeaponFire(FireWeaponEventArgs fireWeaponEventArgs)
    {
        //handle weapon precharge timer
        WeaponPreCharge(fireWeaponEventArgs);

        //weapon fire
        if (fireWeaponEventArgs.fire)
        {
            //test if weapon is ready to fire
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);

                ResetCoolDownTimer();

                ResetPrechargeTimer();
            }
        }
    }

    
    //handle weapon precharge.
    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            firePreChargeTimer -= Time.deltaTime;
        }
        else
        {
            ResetPrechargeTimer();
        }
    }

    
    //returns true if the weapon is ready to fire else returns false.
    private bool IsWeaponReadyToFire()
    {
        //if there is no ammo and weapon doesnt have infinite ammo then return false
        if (activeWeapon.GetCurrentWeapon().weaponRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo)
            return false;

        //if the weapon is reloading then return false
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading)
            return false;

        //if the weapon isn't precharged or is cooling down then return false
        if (firePreChargeTimer > 0f || fireRateCoolDownTimer > 0f)
            return false;

        //if no ammo in the clip and the weapon doesn't have infinite clip capacity then return false
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity && activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0)
        {
            //trigger a reload weapon event.
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);

            return false;
        }

        //weapon is ready to fire return true
        return true;
    }

    
    //set up ammo using an ammo gameobject and component from the object pool
    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            //fire ammo routine
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        }
    }

    
    //coroutine to spawn multiple ammo per shot if specified in the ammo details
    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;
        
        int ammoPerShot = Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax + 1);
        
        float ammoSpawnInterval;

        if (ammoPerShot > 1)
        {
            ammoSpawnInterval = Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }
        
        while (ammoCounter < ammoPerShot)
        {
            ammoCounter++;

            //get ammo prefab from array
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[Random.Range(0, currentAmmo.ammoPrefabArray.Length)];

            //get random speed value
            float ammoSpeed = Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);

            //get gameobject with IFireable component
            IFireable ammo = (IFireable)PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity);

            //initialise ammo
            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            //wait for ammo per shot timegap
            yield return new WaitForSeconds(ammoSpawnInterval);
        }

        //reduce ammo clip count if not infinite clip capacity
        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainingAmmo--;
        }

        //call weapon fired event
        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.GetCurrentWeapon());
        
        //weapon fired sound effect
        WeaponSoundEffect();
        
        //display weapon shoot effect
        WeaponShootEffect(aimAngle);
        
        
    }

    
    //reset cooldown timer
    private void ResetCoolDownTimer()
    {
        //reset cooldown timer
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    
    //reset precharge timers
    private void ResetPrechargeTimer()
    {
        //reset precharge timer
        firePreChargeTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponPrechargeTime;
    }
    
    
    //play weapon shooting sound effect
    private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
    
    
    private void WeaponShootEffect(float aimAngle)
    {
        //process if there is a shoot effect and prefab
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect != null && 
            activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab != null)
        {
            //get weapon shoot effect gameobject from the pool with particle system component
            WeaponShootEffect weaponShootEffect = (WeaponShootEffect)PoolManager.Instance.ReuseComponent
                (activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect.weaponShootEffectPrefab, 
                    activeWeapon.GetShootEffectPosition(), Quaternion.identity);

            //set shoot effect
            weaponShootEffect.SetShootEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponShootEffect, aimAngle);

            //set gameobject active the particle system is set to automatically disable the
            // gameobject once finished
            weaponShootEffect.gameObject.SetActive(true);
        }
    }
    
}