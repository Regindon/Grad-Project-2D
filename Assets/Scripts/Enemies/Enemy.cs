using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
#endregion REQUIRE COMPONENTS

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{

    #region Referances
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    private HealthEvent healthEvent;
    private Health health;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    private FireWeapon fireWeapon;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    private MaterializeEffect materializeEffect;
    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    #endregion
    
    
    private void Awake()
    {
        
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        materializeEffect = GetComponent<MaterializeEffect>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        //subscribe to event
        healthEvent.OnHealthChanged += HealthEvent_OnHealthLost;
    }

    private void OnDisable()
    {
        //unsub to events
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthLost;
        //Or i can also put blood sprite effect here too
    }

    
    
    //health lost event
    private void HealthEvent_OnHealthLost(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            EnemyDestroyed();
        }
    }

    
    
    //enemy destroyed
    private void EnemyDestroyed()
    {
        
        StartCoroutine(DeMaterializeEnemy());
        Debug.Log("Enemydestroyed even ?");
        //DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        //CREATING SPRITE FOR BLOOD EFFECT?
        //demateralize enemy
        
        //destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }


    
    //initializing enemy
    public void EnemyInitialization(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevel);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();
        
        StartCoroutine(MaterializeEnemy());
    }

    
    
    
    //spreading frame rates of ai movement to not get lag spikes
    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }


    
    //setting starting health for enemy
    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevel)
    {
        
        foreach (EnemyHealthDetails enemyHealthDetails in enemyDetails.enemyHealthDetailsArray)
        {
            if (enemyHealthDetails.dungeonLevel == dungeonLevel)
            {
                health.SetStartingHealth(enemyHealthDetails.enemyHealthAmount);
                return;
            }
        }
        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    
    
    //setting enemy starting weapon
    private void SetEnemyStartingWeapon()
    {
        
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon() { weaponDetails = enemyDetails.enemyWeapon, weaponReloadTimer = 0f, weaponClipRemainingAmmo 
                = enemyDetails.enemyWeapon.weaponClipAmmoCapacity, weaponRemainingAmmo = enemyDetails.enemyWeapon.weaponAmmoCapacity, isWeaponReloading = false };

            
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        }
    }

    //setting enemy animator speed
    private void SetEnemyAnimationSpeed()
    {
        
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    
    //materializing enemy on spawn
    private IEnumerator MaterializeEnemy()
    {
        
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, 
            enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));

        
        EnemyEnable(true);

    }
    
    private IEnumerator DeMaterializeEnemy()
    {
        
        EnemyEnable(false);
        
        StartCoroutine(materializeEffect.DeMaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor, 
            enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));
        
        
        
        
        

        yield return new WaitForSeconds(enemyDetails.enemyMaterializeTime-.2f);
        
        DestroyedEvent destroyedEvent = GetComponent<DestroyedEvent>();
        //CREATING SPRITE FOR BLOOD EFFECT?
        //demateralize enemy
        
        destroyedEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }

    private void EnemyEnable(bool isEnabled)
    {
        
        circleCollider2D.enabled = isEnabled;
        polygonCollider2D.enabled = isEnabled;

       
        enemyMovementAI.enabled = isEnabled;

        
        
        fireWeapon.enabled = isEnabled;

    }
}