using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERENCES
    #region Tooltip
    [Tooltip("Populate with image component on the child WeaponImage gameobject")]
    #endregion Tooltip
    [SerializeField] private Image weaponImage;
    #region Tooltip
    [Tooltip("Populate with the Transform from the child AmmoHolder gameobject")]
    #endregion Tooltip
    [SerializeField] private Transform ammoHolderTransform;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child ReloadText gameobject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI reloadText;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child AmmoRemainingText gameobject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    #region Tooltip
    [Tooltip("Populate with the TextMeshPro-Text component on the child WeaponNameText gameobject")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI weaponNameText;
    #region Tooltip
    [Tooltip("Populate with the RectTransform of the child gameobject ReloadBar")]
    #endregion Tooltip
    [SerializeField] private Transform reloadBar;
    #region Tooltip
    [Tooltip("Populate with the Image component of the child gameobject BarImage")]
    #endregion Tooltip
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
       
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        //subscribe to events
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnWeaponReload;
        
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable()
    {
        //unsubscribe from events
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnWeaponReload;
        
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        //update active weapon status on the ui
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }


    //handle set active weapon event on the ui
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    
    
    //handle Weapon fired event on the UI
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

 
    //weapon fired update ui
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

 
    //handle weapon reload event on the ui
    private void ReloadWeaponEvent_OnWeaponReload(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }


    
    //handle weapon reloaded event on the UI
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }


    //weapon has been reloaded update ui if current weapon
    private void WeaponReloaded(Weapon weapon)
    {
        //if weapon reloaded is the current weapon
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }


    
    //sett the active weapon on the ui
    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        //if set weapon is still reloading then update reload bar
        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }


    
    //populate active weapon image
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

 
    //populate active weapon name
    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text =  weapon.weaponDetails.weaponName.ToUpper();
    }


    
    //update the ammo remaining text on the ui
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + " / " + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }


    
    //update ammo clip icons on the UI
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            //instantiate ammo icon prefab
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiAmmoIconSpacing * i,0f );

            ammoIconList.Add(ammoIcon);
        }
    }


    
    //clear ammo icons
    private void ClearAmmoLoadedIcons()
    {
        
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }


    //reload weapon update the reload bar on the ui
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity)
            return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        //reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }


    //animate reload weapon bar coroutine
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        //set the reload bar to red
        barImage.color = Color.red;

        //animate the weapon reload bar
        while (currentWeapon.isWeaponReloading)
        {
            //update reloadbar
            float barFill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            //update bar fill
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);

            yield return null;
        }
    }


    
    //initialise the weapon reload bar on the UI
    private void ResetWeaponReloadBar()
    {
        StopReloadWeaponCoroutine();
        
        barImage.color = Color.green;
        
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }


    
    //stop coroutine updating weapon reload progress bar
    private void StopReloadWeaponCoroutine()
    {
        //stop any active weapon reload bar on the ui
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }
    
    
    
    //update the blinking weapon reload text
    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
           
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }
    
    
    
    
    //start the coroutine to blink the reload weapon text
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }


    
    
    //stop the blinking reload text 
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }


    //stop the blinking reload text coroutine
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }

#endif
    #endregion Validation
}
