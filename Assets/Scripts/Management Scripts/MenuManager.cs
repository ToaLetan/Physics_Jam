using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour 
{
    private const int NUMOFCOLOURS = 8;

    private List<GameObject> previewPlayers = new List<GameObject>();

    private Color[] colourArray = new Color[NUMOFCOLOURS];

    private int currentColourPlayer1 = -1;
    private int currentColourPlayer2 = -1;

    private bool canChangeColourPlayer1 = true;
    private bool canChangeColourPlayer2 = true;

    private InputManager inputManager;

	// Use this for initialization
	void Start () 
    {
        inputManager = InputManager.Instance;
        inputManager.Key_Pressed += MenuInput;
        inputManager.Key_Released += CheckReleasedKeys;

        PopulateColours();

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("PlayerSelect").Length; i++)
        {
            previewPlayers.Add(GameObject.FindGameObjectsWithTag("PlayerSelect")[i]);
        }

        //Set players to start as red.
        ApplyColourPreview(1, 0);
        ApplyColourPreview(1, 1);

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

    private void ApplyColourPreview(int incrementation, int playerNum)
    {
        //THIS IS REALLY GHETTO FOR NOW, IMPROVE IT LATER

        Color newColour = Color.white;

        if (playerNum == 0 && canChangeColourPlayer1 == true)
        {
            currentColourPlayer1 += incrementation;

            if(currentColourPlayer1 >= colourArray.Length)
                currentColourPlayer1 = 0;
            if(currentColourPlayer1 < 0)
                currentColourPlayer1 = colourArray.Length - 1;

            newColour = colourArray [currentColourPlayer1];

            GameInfoManager.Instance.ColourPlayer1 = newColour;

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



            GameInfoManager.Instance.ColourPlayer2 = newColour;

            canChangeColourPlayer2 = false;

            //Apply the colour to the player
            previewPlayers[playerNum].transform.FindChild("PreviewPlayer").GetChild(0).GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("ColourPreview").GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("Text_Player" + (playerNum+1) ).GetComponent<SpriteRenderer>().color = newColour;
        }
    }

    private void MenuInput(int playerNum, List<KeyCode> keysHeld)
    {
        for (int i = 0; i < previewPlayers.Count; i++)
        {
            if(keysHeld.Contains(inputManager.PlayerKeybindArray [i].LeftKey) )
            {
                ApplyColourPreview(-1, i);
            }
            if(keysHeld.Contains(inputManager.PlayerKeybindArray [i].RightKey) )
            {
                ApplyColourPreview(1, i);
            }
        }

        if(keysHeld.Contains(inputManager.PlayerKeybindArray [0].SelectKey) ) //Start game once enter has been pressed. Make sure to unsub first.
        {
            inputManager.Key_Pressed -= MenuInput;
            Application.LoadLevel("Main");
        }
    }

    private void CheckReleasedKeys(int playerNum, List<KeyCode> keysReleased)
    {
        for (int i = 0; i < previewPlayers.Count; i++)
        {
            if(keysReleased.Contains(inputManager.PlayerKeybindArray [i].LeftKey) && keysReleased.Contains(inputManager.PlayerKeybindArray [i].RightKey) )
            {
                if(i == 0)
                    canChangeColourPlayer1 = true;
                else
                    canChangeColourPlayer2 = true;
            }
        }
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
}
