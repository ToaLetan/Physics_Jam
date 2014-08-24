using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public delegate void GameEvent();
    public GameEvent GamePaused;
    public GameEvent GameResumed;

    public List<GameObject> PlayerList = new List<GameObject>();
    public List<GameObject> PlayerSpawnPoints = new List<GameObject>();

    private GameObject pauseMenu = null;

    private bool isGamePaused = false;
    private bool isGameOver = false;

    private XboxController newController;

    public bool IsGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }

	// Use this for initialization
	void Start () 
	{
        InitializeGame();

        newController = new XboxController(1);
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
            if (GameInfoManager.Instance.JoinedPlayers[i] == true)
            {
                PlayerSpawnPoints.Add(GameObject.FindGameObjectsWithTag("PlayerSpawn")[i]);
                GameObject newPlayer = GameObject.Instantiate(Resources.Load("Prefabs/PlayerObjects/Player")) as GameObject;
                newPlayer.transform.position = PlayerSpawnPoints[i].transform.position;
                newPlayer.GetComponent<PlayerScript>().PlayerNumber = i;
            }
        }

        //Get all players, add them to the exposed PlayerList and subscribe to their Player_Death events.
        for (int j = 0; j < GameObject.FindGameObjectsWithTag("Player").Length; j++)
        {
            PlayerList.Add(GameObject.FindGameObjectsWithTag("Player")[j]);
            GameObject.FindGameObjectsWithTag("Player")[j].GetComponent<PlayerScript>().Player_Death += OnPlayerDeath;
            GameObject.FindGameObjectsWithTag("Player")[j].GetComponent<PlayerScript>().Player_Lose += OnPlayerLose;
        
            InputManager.Instance.Key_Pressed += HandleInput;
        
            GameObject.FindGameObjectsWithTag("Player")[j].GetComponent<PlayerScript>().SetPlayerColour(GameInfoManager.Instance.PlayerColours[j]);
        
            UIManager.Instance.ResetUI();
        }
    }

    private void OnPlayerDeath(int playerNum)
    {
        //Remove 1 life from the player.
        UIManager.Instance.RemoveLife(playerNum);
    }

    private void OnPlayerLose(int playerNum)
    {
        //Show the winner, and the prompt for what to do next.
        //THIS IS REALLY CHEAP, IMPROVE IT WHENEVER.
        if(playerNum == 0)
            UIManager.Instance.ShowEnding(1);
        else
            UIManager.Instance.ShowEnding(0);

        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].GetComponent<PlayerScript>().CanMove = false;
        }

        isGameOver = true;
    }

    private void HandleInput(int playerNum, List<string> keysPressed)
    {
        /*if(keysHeld.Contains(InputManager.Instance.PlayerKeybindArray [0].SelectKey) ) //GO back to the main menu after unsubbing.
        {
            //All players must unsub from InputManager first
            for(int i = 0; i < PlayerList.Count; i++)
            {
                InputManager.Instance.Key_Pressed -= PlayerList[i].GetComponent<PlayerScript>().PlayerInput;
                InputManager.Instance.Key_Released -= PlayerList[i].GetComponent<PlayerScript>().ApplyDeceleration;
            }

            InputManager.Instance.Key_Pressed -= HandleInput;
            Application.LoadLevel("Menu");
        }*/

        if (keysPressed.Contains(InputManager.Instance.PlayerKeybindArray [0].SelectKey.ToString() ) ) //Bring up the pause menu
        {
            if(isGamePaused == false)
                ShowPauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        if (GamePaused != null)
            GamePaused();

        //Instantiate the pause menu if it doesn't exist, attach it to the camera and center it.
        if (pauseMenu == null)
        {
            pauseMenu = GameObject.Instantiate(Resources.Load("Prefabs/GUI/PauseMenu") ) as GameObject;
            pauseMenu.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
            pauseMenu.transform.localPosition = new Vector3(0, 0, 1);
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
        Application.LoadLevel("Menu");
    }
}
