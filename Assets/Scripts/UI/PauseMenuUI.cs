using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    
    #region Tooltip
    [Tooltip("Populate with the music volume level")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI musicLevelText;
    #region Tooltip
    [Tooltip("Populate with the sounds volume level")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI soundsLevelText;
    
    
    private void Start()
    {
        // Initially hide the pause menu
        gameObject.SetActive(false);
    }

    //Initialize the UI text
    private IEnumerator InitializeUI()
    {
        //Wait a frame so previous music and sound levels have been set
        yield return null;

        //Initialise UI text
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    
    private void OnEnable()
    {
        Time.timeScale = 0f;
        
        //Initialise UI text
        StartCoroutine(InitializeUI());
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
    

    //increase music volume - linked to from music volume increase button in UI
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }


    //decrease music volume - linked to from music volume decrease button in UI
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }


    //Increase sounds volume - linked to from sounds volume increase button in UI
    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }


    //decrease sounds volume - linked to from sounds volume decrease button in U
    public void DecreaseSoundsVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }


    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLevelText), musicLevelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsLevelText), soundsLevelText);
    }

#endif
    #endregion Validation
    
    
}