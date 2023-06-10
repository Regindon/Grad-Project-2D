using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChooseNameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    void Start()
    {
        GameResources.Instance.currentPlayer.playerName = playerNameInput.text;
    }

    
    //this method is called from the field changed in inspector
    public void UpdatePlayerName()
    {
        playerNameInput.text = playerNameInput.text.ToUpper();

        GameResources.Instance.currentPlayer.playerName = playerNameInput.text;
    }
}
