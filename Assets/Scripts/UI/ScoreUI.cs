using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreTextTMP;

    private void Awake()
    {
        scoreTextTMP = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        //subscribe to the score changed event
        StaticEventHandler.OnScoreChanged += StaticEventHandler_OnScoreChanged;
    }

    private void OnDisable()
    {
        //unsubscribe from the score changed event
        StaticEventHandler.OnScoreChanged -= StaticEventHandler_OnScoreChanged;
    }

    //handle score changed event
    private void StaticEventHandler_OnScoreChanged(ScoreChangedArgs scoreChangedArgs)
    {
        //update UI
        scoreTextTMP.text = "SCORE: " + scoreChangedArgs.score.ToString("###,###0");
    }

}