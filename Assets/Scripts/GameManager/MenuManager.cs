using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    /*
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    */

    public void MenuToGame()
    {
        Cursor.visible = false;
        SceneManager.LoadScene(2);
    }

    public void MenuToOptions()
    {
        
    }

    public void MenuToControls()
    {
        SceneManager.LoadScene(1);
    }

    public void ControlsToMenu()
    {
        SceneManager.LoadScene(0);
    }
    

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
        
    }
}
