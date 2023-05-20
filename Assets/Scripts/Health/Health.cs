using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]
    #endregion
    [SerializeField] private HealthBar healthBar;

    #region Referances
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);
    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;
    [HideInInspector] public GameState gameState;
    #endregion

    private void Awake()
    {
     
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        //trigger a health event for UI update
        CallHealthEvent(0);

        //attempt to load enemy player components
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        bool enableEnemyBossHealthBar;

        if (gameState == GameState.bossStage || gameState == GameState.engagingBoss)
        {
            enableEnemyBossHealthBar = true;
            //Debug.Log(enableEnemyBossHealthBar+"before all +++++++++");
        }
        else
        {
            enableEnemyBossHealthBar = false;
            //Debug.Log(enableEnemyBossHealthBar+"before all +++++++++");
        }

        //get player and enemy hit immunity details
        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.enemyDetails.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
        
        //enable the health bar if required
        if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null && enableEnemyBossHealthBar)
        {
            //Debug.Log(enableEnemyBossHealthBar +"+++++++++++++++++++++++++++++++++++");
            healthBar.EnableHealthBar();
            
        }
        else if (healthBar != null || !enableEnemyBossHealthBar)
        {
            //Debug.Log(enableEnemyBossHealthBar +"+++++++++++++++++++++++++++++++++++");
            healthBar.DisableHealthBar();
        }
    }

    
    
    
    //public method called when damage is taken
    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false;

        if (player != null)
            isRolling = player.playerControl.isPlayerRolling;

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();
            
            //set health bar as the percentage of health remaining
            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }
        }
    }

    
    
    //indicate a hit and give some post hit immunity
    private void PostHitImmunity()
    {
        
        if (gameObject.activeSelf == false)
            return;

       
        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
                StopCoroutine(immunityCoroutine);

            //flash red and give period of immunity
            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }

    }


    
    //coroutine to indicate a hit and give some post hit immunity
    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return WaitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return WaitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;

        }

        isDamageable = true;

    }

    private void CallHealthEvent(int damageAmount)
    {
        //trigger health event
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }

    
    
    //set starting health 
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }
    
    
    //get the starting health
    public int GetStartingHealth()
    {
        return startingHealth;
    }

}