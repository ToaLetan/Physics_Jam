﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public delegate void GameEvent();
    public GameEvent GamePaused;
    public GameEvent GameResumed;

    public List<GameObject> PlayerList = new List<GameObject>();
    public List<GameObject> PlayerSpawnPoints = new List<GameObject>();

    private List<int> numPlayerLives = new List<int>();

    private GameObject pauseMenu = null;

    private int numOfDefeatedPlayers = 0;

    private bool isGamePaused = false;
    private bool isGameOver = false;

    public bool IsGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
        set { isGameOver = value; }
    }

	// Use this for initialization
	void Start () 
	{
        InitializeGame();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update any management systems
		InputManager.Instance.Update();
		SlowMoManager.Instance.Update();
        UIManager.Instance.Update();
	}

    private void InitializeGame()
    {
        //Associate players to their respective spawn points.
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("PlayerSpawn").Length; i++)
        {
            PlayerSpawnPoints.Add(GameObject.FindGameObjectsWithTag("PlayerSpawn")[i]);

            PlayerSpawnPoints.Sort(SortSpawnPointsByIndex);
        }

        for(int j = 0; j < PlayerSpawnPoints.Count; j++)
        {
            if (GameInfoManager.Instance.JoinedPlayers[j] == true)
            {
                GameObject newPlayer = GameObject.Instantiate(Resources.Load("Prefabs/PlayerObjects/Player")) as GameObject;

                //Position the player on the corresponding spawn point.

                newPlayer.transform.position = PlayerSpawnPoints[j].transform.position;
                newPlayer.GetComponent<PlayerScript>().PlayerNumber = j;

                numPlayerLives.Add(newPlayer.GetComponent<PlayerScript>().NumLives);
            }
        }

        //Get all players, add them to the exposed PlayerList and subscribe to their Player_Death events.
        GameObject[] existingPlayers = GameObject.FindGameObjectsWithTag("Player");

        for (int j = 0; j < existingPlayers.Length; j++)
        {
            PlayerList.Add(existingPlayers[j]);

            if (existingPlayers[j].GetComponent<PlayerScript>() != null)
            {
                existingPlayers[j].GetComponent<PlayerScript>().Player_Death += OnPlayerDeath;
                existingPlayers[j].GetComponent<PlayerScript>().Player_Lose += OnPlayerLose;
                existingPlayers[j].GetComponent<PlayerScript>().SetPlayerColour(GameInfoManager.Instance.PlayerColours[j]);
                existingPlayers[j].GetComponent<PlayerScript>().SetPlayerAbility(GameInfoManager.Instance.PlayerActives[j]);
            }
        }
        InputManager.Instance.Key_Pressed += HandleInput;
        UIManager.Instance.ResetUI();
    }

    private static int SortSpawnPointsByIndex(GameObject obj1, GameObject obj2)
    {
        return obj1.GetComponent<PlayerSpawnPointScript>().owningPlayerTag.CompareTo(obj2.GetComponent<PlayerSpawnPointScript>().owningPlayerTag);
    }

    private void OnPlayerDeath(int playerNum)
    {
        //Remove 1 life from the player.
        UIManager.Instance.RemoveLife(playerNum);
        numPlayerLives[playerNum] -= 1;
    }

    private void OnPlayerLose(int playerNum)
    {
        int mostLives = 0;
        int playerNumWithMostLives = 0;

        numOfDefeatedPlayers += 1;

        for (int i = 0; i < PlayerList.Count; i++)
        {
            if (numPlayerLives[i] > mostLives)
            {
                mostLives = numPlayerLives[i];
                playerNumWithMostLives = i;
            }
        }

        //If they were the second last player, the game is over.
        //Show the winner (remaining player) and prompt what to do next.
        if (numOfDefeatedPlayers == PlayerList.Count - 1)
        {
            for (int j = 0; j < PlayerList.Count; j++)
            {
                PlayerList[j].GetComponent<PlayerScript>().CanMove = false;
            }
            UIManager.Instance.ShowEnding(playerNumWithMostLives);
            isGameOver = true;
        }
    }

    private void HandleInput(int playerNum, List<string> keysPressed)
    {
        /*if (keysPressed.Contains(InputManager.Instance.PlayerKeybindArray [0].SelectKey.ToString() ) ) //Bring up the pause menu
        {
            if(isGamePaused == false)
                ShowPauseMenu();
        }*/
    }

    public void ShowPauseMenu( string inputSource, int inputSourceIndex)
    {
        if (GamePaused != null)
            GamePaused();

        //Instantiate the pause menu if it doesn't exist, attach it to the camera and center it.
        if (pauseMenu == null)
        {
            pauseMenu = GameObject.Instantiate(Resources.Load("Prefabs/GUI/PauseMenu") ) as GameObject;
            pauseMenu.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
            pauseMenu.transform.localPosition = new Vector3(0, 0, 1);
            pauseMenu.GetComponent<PauseMenu>().OwnerInputSource = inputSource;
            pauseMenu.GetComponent<PauseMenu>().OwnerInputIndex = inputSourceIndex;
        }

        isGamePaused = true;
    }

    public void HidePauseMenu()
    {
        if (GameResumed != null)
            GameResumed();

        if (pauseMenu != null)
           GameObject.Destroy(pauseMenu);

        isGamePaused = false;
    }

    public void RestartGame() //Go back to the Main Menu, make sure to unsubscribe to events, prevent all null errors, etc.
    {
        GameInfoManager.Instance.Reset();
        Application.LoadLevel("Splash");
    }
}
