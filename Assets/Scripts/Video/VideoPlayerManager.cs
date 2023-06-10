using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    private VideoPlayer vp;
    private bool canChange;
    private int currentSceneIndex;
    

    private void Start()
    {
        vp = GetComponent<VideoPlayer>();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (vp.isPlaying)
        {
            canChange = true;
        }

        if (canChange && !vp.isPlaying)
        {
            if(currentSceneIndex ==2) SceneManager.LoadScene("MainGameScene");
            else
            {
                SceneManager.LoadScene("MainMenuV2");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            
            if(currentSceneIndex ==2) SceneManager.LoadScene("MainGameScene");
            else
            {
                SceneManager.LoadScene("MainMenuV2");
            }
            
        }
    }

    
    
}
