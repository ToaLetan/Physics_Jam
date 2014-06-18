using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public List<GameObject> PlayerList = new List<GameObject>();

	// Use this for initialization
	void Start () 
	{
        //Get all players, add them to the exposed PlayerList and subscribe to their Player_Death events.
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            PlayerList.Add(GameObject.FindGameObjectsWithTag("Player") [i]);
            GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerScript>().Player_Death += OnPlayerDeath;
            GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerScript>().Player_Lose += OnPlayerLose;

            //InputManager.Instance.Key_Pressed += HandleInput;


            //EXTREMELY TEMPORARY
            if(i == 0)
                GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerScript>().SetPlayerColour(GameInfoManager.Instance.ColourPlayer1);
            else
                GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerScript>().SetPlayerColour(GameInfoManager.Instance.ColourPlayer2);
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update any management systems
		InputManager.Instance.Update();
		SlowMoManager.Instance.Update();
        UIManager.Instance.Update();
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
    }

    private void HandleInput(int playerNum, List<KeyCode> keysHeld)
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
    }
}
