using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MusicManager : SingletonMonobehaviour<MusicManager>
{
    private AudioSource musicAudioSource = null;
    private AudioClip currentAudioClip = null;
    private Coroutine fadeOutMusicCoroutine;
    private Coroutine fadeInMusicCoroutine;
    public int musicVolume = 10;

    protected override void Awake()
    {
        base.Awake();

        //load components
        musicAudioSource = GetComponent<AudioSource>();

        //start with music off
        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);

    }

    private void Start()
    {
        
        //check if volume save exist and get it afterwards
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetInt("musicVolume");
        }
        
        SetMusicVolume(musicVolume);
    }
    
    
    private void OnDisable()
    {
        //save music volume
        PlayerPrefs.SetInt("musicVolume", musicVolume);
    }
    
    
    

    public void PlayMusic(MusicTrackSO musicTrack, float fadeOutTime = Settings.musicFadeOutTime, float fadeInTime = Settings.musicFadeInTime)
    {
        //play music track
        StartCoroutine(PlayMusicRoutine(musicTrack, fadeOutTime, fadeInTime));
    }


    
    //play music for room routine
    private IEnumerator PlayMusicRoutine(MusicTrackSO musicTrack, float fadeOutTime, float fadeInTime)
    {
        //if fade out routine already running then stop it
        if (fadeOutMusicCoroutine != null)
        {
            StopCoroutine(fadeOutMusicCoroutine);
        }

        //if fade in routine already running then stop it
        if (fadeInMusicCoroutine != null)
        {
            StopCoroutine(fadeInMusicCoroutine);
        }

        //if the music track has changed then play new music track
        if (musicTrack.musicClip != currentAudioClip)
        {
            currentAudioClip = musicTrack.musicClip;

            yield return fadeOutMusicCoroutine = StartCoroutine(FadeOutMusic(fadeOutTime));

            yield return fadeInMusicCoroutine = StartCoroutine(FadeInMusic(musicTrack, fadeInTime));
        }

        yield return null;
    }


    //fade out music routine
    private IEnumerator FadeOutMusic(float fadeOutTime)
    {
        GameResources.Instance.musicLowSnapshot.TransitionTo(fadeOutTime);

        yield return new WaitForSeconds(fadeOutTime);
    }


    //fade in music routine
    private IEnumerator FadeInMusic(MusicTrackSO musicTrack, float fadeInTime)
    {
        //set clip & play
        musicAudioSource.clip = musicTrack.musicClip;
        musicAudioSource.volume = musicTrack.musicVolume;
        musicAudioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(fadeInTime);

        yield return new WaitForSeconds(fadeInTime);
    }

    
    
    //Increase music volume
    public void IncreaseMusicVolume()
    {
        int maxMusicVolume = 20;

        if (musicVolume >= maxMusicVolume) return;

        musicVolume += 1;

        SetMusicVolume(musicVolume);
    }


    //Decrease music volume
    public void DecreaseMusicVolume()
    {
        if (musicVolume == 0) return;

        musicVolume -= 1;

        SetMusicVolume(musicVolume);
    }
    

    //set music volume
    public void SetMusicVolume(int musicVolume)
    {
        float muteDecibels = -80f;

        if (musicVolume == 0)
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", muteDecibels);
        }
        else
        {
            GameResources.Instance.musicMasterMixerGroup.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(musicVolume));
        }
    }
}