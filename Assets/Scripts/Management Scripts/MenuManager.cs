using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour 
{
	private const int NUM_OF_COLOURS = 8;
	private const int MAX_NUM_OF_PLAYERS = 4;

	private List<GameObject> previewPlayers = new List<GameObject>();

	private Color[] colourArray = new Color[NUM_OF_COLOURS];

	//Kinda messy, maybe set up a class for this?
	private int currentColourPlayer1 = -1;
	private int currentColourPlayer2 = -1;
	private int currentColourPlayer3 = -1;
	private int currentColourPlayer4 = -1;

	private bool canChangeColourPlayer1 = true;
	private bool canChangeColourPlayer2 = true;
	private bool canChangeColourPlayer3 = true;
	private bool canChangeColourPlayer4 = true;

	private InputManager inputManager;

    private int currentJoinedPlayerIndex = -1; //Player number of whoever joined recently, set to -1 because arrays start at 0.

	// Use this for initialization
	void Start () 
	{
		inputManager = InputManager.Instance;
		inputManager.Key_Pressed += MenuInput;
		inputManager.Key_Released += CheckReleasedKeys;

        inputManager.Button_Pressed += ButtonMenuInput;

		PopulateColours();
		PopulatePreviewPlayers();
		

		//Set players to start as red.
		//ApplyColourPreview(1, 0);
		//ApplyColourPreview(1, 1);

		AnimatePlayers();
	}
	
	// Update is called once per frame
	void Update () 
	{
		inputManager.Update();
	}

	private void PopulateColours()
	{
		//THIS IS ALL REALLY TEMPORARY
		colourArray [0] = Color.red;
		colourArray [1] = Color.blue;
		colourArray [2] = Color.green;
		colourArray [3] = Color.yellow;
		colourArray [4] = new Color(1, 0, 1); //Pink
		colourArray [5] = new Color(0.4f, 0, 0.5f); //Purple
		colourArray [6] = new Color(1, 0.3f, 0); //Orange
		colourArray [7] = Color.cyan;
	}

	private void PopulatePreviewPlayers() //Add the preview player bars in order from top to bottom.
	{
		float highestValue = -5;
		GameObject topmostPlayer = null;

		for (int i = 0; i < MAX_NUM_OF_PLAYERS; i++)
		{
			for (int j = 0; j < GameObject.FindGameObjectsWithTag("PlayerSelect").Length; j++)
			{
				//If the player's the topmost one and isn't already in the previewPlayers array, consider it the current topmost one.
				if (GameObject.FindGameObjectsWithTag("PlayerSelect")[j].transform.position.y >= highestValue && previewPlayers.Contains(GameObject.FindGameObjectsWithTag("PlayerSelect")[j]) == false)
				{
					highestValue = GameObject.FindGameObjectsWithTag("PlayerSelect")[j].transform.position.y;
					topmostPlayer = GameObject.FindGameObjectsWithTag("PlayerSelect")[j];
				}
			}
			previewPlayers.Add(topmostPlayer);
			highestValue = -5;
			
		}
	}

	private void ApplyColourPreview(int incrementation, int playerNum)
	{
		//THIS IS REALLY GHETTO FOR NOW, IMPROVE IT LATER

		Color newColour = Color.cyan;

		for (int i = 0; i < previewPlayers.Count; i++)
		{
            previewPlayers[playerNum].transform.FindChild("PreviewPlayer").GetChild(0).GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("ColourPreview").GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("Text_Player" + (playerNum + 1)).GetComponent<SpriteRenderer>().color = newColour;
		}

		/*
		if (playerNum == 0 && canChangeColourPlayer1 == true)
		{
			currentColourPlayer1 += incrementation;

			if(currentColourPlayer1 >= colourArray.Length)
				currentColourPlayer1 = 0;
			if(currentColourPlayer1 < 0)
				currentColourPlayer1 = colourArray.Length - 1;

			newColour = colourArray [currentColourPlayer1];

			GameInfoManager.Instance.PlayerColours[0] = newColour;

			canChangeColourPlayer1 = false;

			//Apply the colour to the player
			previewPlayers[playerNum].transform.FindChild("PreviewPlayer").GetChild(0).GetComponent<SpriteRenderer>().color = newColour;
			previewPlayers[playerNum].transform.FindChild("ColourPreview").GetComponent<SpriteRenderer>().color = newColour;
			previewPlayers[playerNum].transform.FindChild("Text_Player" + (playerNum+1) ).GetComponent<SpriteRenderer>().color = newColour;
		} 
		if (playerNum == 1 && canChangeColourPlayer2 == true)
		{
			currentColourPlayer2 += incrementation;

			if(currentColourPlayer2 >= colourArray.Length)
				currentColourPlayer2 = 0;
			if(currentColourPlayer2 < 0)
				currentColourPlayer2 = colourArray.Length - 1;

			newColour = colourArray [currentColourPlayer2];



			GameInfoManager.Instance.PlayerColours[1] = newColour;

			canChangeColourPlayer2 = false;

			//Apply the colour to the player
			previewPlayers[playerNum].transform.FindChild("PreviewPlayer").GetChild(0).GetComponent<SpriteRenderer>().color = newColour;
			previewPlayers[playerNum].transform.FindChild("ColourPreview").GetComponent<SpriteRenderer>().color = newColour;
			previewPlayers[playerNum].transform.FindChild("Text_Player" + (playerNum+1) ).GetComponent<SpriteRenderer>().color = newColour;
		}*/
	}

	private void MenuInput(int playerNum, List<string> keysHeld)
	{
        for (int i = 0; i < inputManager.PlayerKeybindArray.Length; i++)
		{
            if (keysHeld.Contains(inputManager.PlayerKeybindArray[i].GraborThrowKey.ToString() ) )
            {
                string inputString = inputManager.PlayerKeybindArray[i].ToString() + " " + playerNum;
                //If there aren't 4 players and the input source hasn't already been bound to someone, join the new player.
                if (currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1 && GameInfoManager.Instance.PlayerInputSources.Contains(inputString) == false)
                    PlayerJoin(inputString);
            }

            if (keysHeld.Contains(inputManager.PlayerKeybindArray[i].LeftKey.ToString() ) )
			{
				//ApplyColourPreview(-1, i);
			}
            if (keysHeld.Contains(inputManager.PlayerKeybindArray[i].RightKey.ToString() ) )
			{
				//ApplyColourPreview(1, i);
			}
		}

        if (keysHeld.Contains(inputManager.PlayerKeybindArray[0].SelectKey.ToString() ) ) //Start game once enter has been pressed. Make sure to unsub first.
		{
            if (currentJoinedPlayerIndex >= 1) //Prevent a game from starting until there's at least two players.
            {
                inputManager.Key_Pressed -= MenuInput;
                Application.LoadLevel("Main");
            }
		}
	}

	private void CheckReleasedKeys(int playerNum, List<string> keysReleased)
	{
		/*
		for (int i = 0; i < previewPlayers.Count; i++)
		{
			if(keysReleased.Contains(inputManager.PlayerKeybindArray [i].LeftKey) && keysReleased.Contains(inputManager.PlayerKeybindArray [i].RightKey) )
			{
				if(i == 0)
					canChangeColourPlayer1 = true;
				else
					canChangeColourPlayer2 = true;
			}
		}*/
	}

	private void AnimatePlayers()
	{
		for(int i = 0; i < previewPlayers.Count; i++)
		{
			if(previewPlayers[i].GetComponent<Animator>() != null)
			{
				AnimationPlayer.PlayAnimation(previewPlayers[i], "Player_Fall");
			}
		}
	}

	private void PlayerJoin(string inputSource)
	{
		//When someone presses a Join Button, get the input source and tie it to the current player number.
		//Ex: if there's no current player 1, tie it to player 1.
        if(currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1)
            currentJoinedPlayerIndex++;

        GameInfoManager.Instance.PlayerInputSources[currentJoinedPlayerIndex] = inputSource;

        Debug.Log(inputSource);

        //Move the player panels on-screen all fancy-like.
        Vector3 panelPosition = Vector3.zero;

        switch(currentJoinedPlayerIndex)
        {
            case 0:
                panelPosition = new Vector3(-0.50f, 1.05f, previewPlayers[currentJoinedPlayerIndex].transform.parent.transform.position.z);
                break;
            case 1:
                panelPosition = new Vector3(-0.93f, 0.61f, previewPlayers[currentJoinedPlayerIndex].transform.parent.transform.position.z);
                break;
            case 2:
                panelPosition = new Vector3(-1.37f, 0.16f, previewPlayers[currentJoinedPlayerIndex].transform.parent.transform.position.z);
                break;
            case 3:
                panelPosition = new Vector3(-1.81f, -0.29f, previewPlayers[currentJoinedPlayerIndex].transform.parent.transform.position.z);
                break;
        }

        previewPlayers[currentJoinedPlayerIndex].transform.parent.transform.position = panelPosition;
	}

    private void ButtonMenuInput(int playerNum, List<string> buttonsHeld)
    {
            if (buttonsHeld.Contains(inputManager.ControllerArray[playerNum].buttonA))
			{
                string inputString = inputManager.ControllerArray[playerNum].ToString() + " " + playerNum;
                //If there aren't 4 players and the input source hasn't already been bound to someone, join the new player.
                if (currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1 && GameInfoManager.Instance.PlayerInputSources.Contains(inputString) == false)
                    PlayerJoin(inputString);
			}
    }
}
