using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private void Start()
    {
        
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);
    }
    
    
    
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("ControlsMenuV2");
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenuV2");
    }

    public void OpenHighScores()
    {
        SceneManager.LoadScene("HighScoreScene");
    }

    public void OpenStartCinematic()
    {
        SceneManager.LoadScene("StartCinematicScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}