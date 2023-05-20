using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header GAMEOBJECT REFERENCES

    #region Tooltip
    [Tooltip("Populate with the MessageText textmeshpro component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI messageTextTMP;

    #region Tooltip
    [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;
    
    
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("Populate with the starting dungeon level for testing , first level = 0")]

    #endregion Tooltip
    [SerializeField] private int currentDungeonLevelListIndex = 0;

    #region Referances
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;
    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    private long gameScore;
    private InstantiatedRoom bossRoom;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;
        
        InstantiatePlayer();
    }

    
    private void InstantiatePlayer()
    {
        //instantiate player
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        //initialize Player
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);

    }


    private void OnEnable()
    {
        //subscribing to events
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
        
        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        //unsubscribing to events
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
        
        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;

    }
    
    //handle room changed event
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }
    
    
    //handle room enemies defeated event
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    
    //handle points scored event
    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        //increase score
        gameScore += pointsScoredArgs.points;

        //call score changed event
        StaticEventHandler.CallScoreChangedEvent(gameScore);
    }
    
    
    //handle player destroyed event
    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }
    

    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
        
        //set score to zero
        gameScore = 0;
        
        //set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
        
    }

    
    private void Update()
    {
        HandleGameState();

        
        if (Input.GetKeyDown(KeyCode.N))
        {
            gameState = GameState.gameStarted;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        
        //Debug.Log(gameState);
        
    }

    
    private void HandleGameState()
    {
        
        switch (gameState)
        {
            case GameState.gameStarted:

                //play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;
                
                //trigger room enemies defeated since we start in the entrance where there are no enemies
                RoomEnemiesDefeated();

                break;

            //handle the level being completed
            case GameState.levelCompleted:

                //display level completed text
                StartCoroutine(LevelCompleted());

                break;
            
            //handle the game being won only trigger this once test the previous game state to do this
            case GameState.gameWon:

                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());

                break;

            //handle the game being lost only trigger this once test the previous game state to do this
            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines(); //prevent messages if you clear the level just as you get killed
                    StartCoroutine(GameLost());
                }

                break;

            
            case GameState.restartGame:

                RestartGame();

                break;

        }

    }

    
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;

        
    }
    
    
    
    //room enemies defated test if all dungeon rooms have been cleared of enemies if so load
    // next dungeon game level
    private void RoomEnemiesDefeated()
    {
        //initialise dungeon as being cleared then test each room
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        //loop through all dungeon rooms to see if cleared
        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            //skiop boss room
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            //check if other rooms have been cleared of enemies
            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        //setting game state
        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            //are there more dungeon levels then
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        
        //else if dungeon level cleared apart from boss room
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
        }

    }


    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("couldnt build dungeon from specified rooms and node graphs");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);
        
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
        
        
        StartCoroutine(DisplayDungeonLevelText());
        
        //demo code
        //RoomEnemiesDefeated();


    }
    
    
    //display the dungeon level text
    private IEnumerator DisplayDungeonLevelText()
    {
        //set screen to black
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        //fade In
        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));

    }

    
    //display the message text for displaySeconds if displaySeconds =0 then the message is displayed until the return key is pressed
    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        //display the message for the given time
        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        else
            //else display the message until the return button is pressed
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;
        
        messageTextTMP.SetText("");
    }
    
    
    
    private IEnumerator BossStage()
    {
        
        bossRoom.gameObject.SetActive(true);
        bossRoom.UnlockDoors(0f);
        
        yield return new WaitForSeconds(2f);
        
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE YOU'VE SURVIVED SO FAR\n\nNOW FIND AND DEFEAT THE BOSS", Color.white, 5f));
        
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

    }
    
    
    private IEnumerator LevelCompleted()
    {
        
        gameState = GameState.playingLevel;
        
        yield return new WaitForSeconds(2f);
        
        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS ENTER/RETURN\n\nTO DESCEND FURTHER INTO THE DUNGEON", Color.white, 5f));
        
        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
        
        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // to avoid enter being detected twice
        
        // Increase index to next level
        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }
    
    public IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }

    }

    
    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;
        
        GetPlayer().playerControl.DisablePlayer();
        
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));
        
        yield return StartCoroutine(DisplayMessageRoutine("WELL DONE YOU HAVE DEFEATED THE DUNGEON", Color.white, 3f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0"), Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS ENTER/RETURN TO RESTART THE GAME", Color.white, 0f));

        //set game state to restart game
        gameState = GameState.restartGame;
    }

    

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;
        
        GetPlayer().playerControl.DisablePlayer();
        
        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));
        
        //disable enemies
        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }
        
        yield return StartCoroutine(DisplayMessageRoutine("BAD LUCK YOU HAVE SUCCUMBED TO THE DUNGEON", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("YOU SCORED " + gameScore.ToString("###,###0"), Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("PRESS ENTER/RETURN TO RESTART THE GAME", Color.white, 0f));

        // Set game state to restart game
        gameState = GameState.restartGame;
    }

    
    //restart the game
    private void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }
    
    
    public Player GetPlayer()
    {
        return player;
    }
    
    
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    
    //get the current dungeon level
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation

}

