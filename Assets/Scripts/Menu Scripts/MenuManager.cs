using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct TiedController
{
    public int playerNum;
    public XboxController playerController;
}

public struct TiedKeybinds
{
    public int playerNum;
    public int keybindIndex;
}

public class MenuManager : MonoBehaviour 
{
    private const float THUMBSTICK_DEADZONE = 0.1f;
    private const float PANEL_MOVESPEED = 5.0f;

	private const int NUM_OF_COLOURS = 8;
    private const int MAX_NUM_OF_PLAYERS = 4;

	private List<GameObject> previewPlayers = new List<GameObject>();
    private List<GameObject> joinPrompts = new List<GameObject>();
    private List<TiedController> activeControllers = new List<TiedController>();
    private List<TiedKeybinds> activeKeybinds = new List<TiedKeybinds>();
    private List<int> queuedPlayersJoining = new List<int>();

	private Color[] colourArray = new Color[NUM_OF_COLOURS]; //Array of possible colours
    private Vector3[] panelPositionsArray = new Vector3[MAX_NUM_OF_PLAYERS]; //Array of desired locations for panels to move to.
    private int[] playerColourIndexArray = new int[MAX_NUM_OF_PLAYERS]; //Array of current colour indices (ex. red = 1, blue = 2, etc.)
    private bool[] canChangePlayerColourArray = new bool[MAX_NUM_OF_PLAYERS]; //Array of bools used to determine if the player can change colour, prevents holding keys to flicker through colours.

	private InputManager inputManager;

    private Vector3 startGamePrompt_Location = new Vector3(1.38f, -1.18f, 0); //Where the prompt to start the game will move to.

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

        //Set all currentColourIndexes to default at 1 and set all players to be able to change colour.
        for (int i = 0; i < previewPlayers.Count; i++)
        {
            playerColourIndexArray[i] = -1;
            canChangePlayerColourArray[i] = true;
        }

		//Set players to start as red.
        for (int i = 0; i < previewPlayers.Count; i++)
        {
            ApplyColourPreview(1 + i, i);
        }

        //Establish all desired panel positions for when they need to be moved on-screen.
        panelPositionsArray[0] = new Vector3(-0.50f, 1.05f, previewPlayers[0].transform.parent.transform.position.z);
        panelPositionsArray[1] = new Vector3(-0.93f, 0.61f, previewPlayers[1].transform.parent.transform.position.z);
        panelPositionsArray[2] = new Vector3(-1.37f, 0.16f, previewPlayers[2].transform.parent.transform.position.z);
        panelPositionsArray[3] = new Vector3(-1.81f, -0.29f, previewPlayers[3].transform.parent.transform.position.z);

		AnimatePlayers();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (Application.loadedLevel == 0)
        {
            inputManager.Update();

            if (activeControllers.Count > 0)
                UpdateControllerInput();
        }
	}

    private void UpdateControllerInput()
    {
        for (int i = 0; i < activeControllers.Count; i++)
        {
            //If moving left or right, apply the colour preview.
            if (activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.dPadHorizontal) > 0 ||
                activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.leftThumbstickHorizontal) > 0)
                ApplyColourPreview(1, activeControllers[i].playerNum);

            if (activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.dPadHorizontal) < 0 ||
                activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.leftThumbstickHorizontal) < 0)
                ApplyColourPreview(-1, activeControllers[i].playerNum);

            if (activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.dPadHorizontal) == 0 &&
                (activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.leftThumbstickHorizontal) < THUMBSTICK_DEADZONE &&
                activeControllers[i].playerController.GetThumbstickAxis(activeControllers[i].playerController.leftThumbstickHorizontal) > -THUMBSTICK_DEADZONE))
            {
                canChangePlayerColourArray[activeControllers[i].playerNum] = true;
            }
        }
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
		GameObject topmostObject = null;

		for (int i = 0; i < MAX_NUM_OF_PLAYERS; i++)
		{
			for (int j = 0; j < GameObject.FindGameObjectsWithTag("PlayerSelect").Length; j++) //Arrange the player previews by y-position.
			{
				//If the player's the topmost one and isn't already in the previewPlayers array, consider it the current topmost one.
				if (GameObject.FindGameObjectsWithTag("PlayerSelect")[j].transform.position.y >= highestValue && previewPlayers.Contains(GameObject.FindGameObjectsWithTag("PlayerSelect")[j]) == false)
				{
					highestValue = GameObject.FindGameObjectsWithTag("PlayerSelect")[j].transform.position.y;
                    topmostObject = GameObject.FindGameObjectsWithTag("PlayerSelect")[j];
				}
			}
            previewPlayers.Add(topmostObject);
			highestValue = -5;
            topmostObject = null;

            for (int k = 0; k < GameObject.FindGameObjectsWithTag("JoinPrompt").Length; k++) //Do the same thing with the join prompts
            {
                if (GameObject.FindGameObjectsWithTag("JoinPrompt")[k].transform.position.y >= highestValue && joinPrompts.Contains(GameObject.FindGameObjectsWithTag("JoinPrompt")[k]) == false)
                {
                    highestValue = GameObject.FindGameObjectsWithTag("JoinPrompt")[k].transform.position.y;
                    topmostObject = GameObject.FindGameObjectsWithTag("JoinPrompt")[k];
                }
            }
            joinPrompts.Add(topmostObject);
			highestValue = -5;
            topmostObject = null;
		}
	}

	private void ApplyColourPreview(int incrementation, int playerNum)
	{
		Color newColour = Color.cyan;

        if (canChangePlayerColourArray[playerNum] == true)
        {
            playerColourIndexArray[playerNum] += incrementation;

            //Prevent players from sharing colours
            int numPlayersSharingColour = 0;
            for (int i = 0; i < playerColourIndexArray.Length; i++)
            {
                if (i != playerNum && playerColourIndexArray[i] == playerColourIndexArray[playerNum])
                    numPlayersSharingColour += 1;
            }

            if (numPlayersSharingColour > 0)
                playerColourIndexArray[playerNum] += incrementation; //Do a second incrementation
                
            //Wrap around if the player goes all the way left/right
            if (playerColourIndexArray[playerNum] >= colourArray.Length)
                playerColourIndexArray[playerNum] = 0;
            if (playerColourIndexArray[playerNum] < 0)
                playerColourIndexArray[playerNum] = colourArray.Length - 1;

            newColour = colourArray[playerColourIndexArray[playerNum]]; //Set the player colour

            canChangePlayerColourArray[playerNum] = false; //Disable being able to change colour to slow the process (not causing seizures obviously being a plus.)

            GameInfoManager.Instance.PlayerColours[playerNum] = newColour; //Tie the player colour to that player so the GameInfoManager can pass it into the game.

            //Apply the colour to the player previews
            previewPlayers[playerNum].transform.FindChild("PreviewPlayer").GetChild(0).GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("ColourPreview").GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("Text_Player" + (playerNum + 1)).GetComponent<SpriteRenderer>().color = newColour;
        }
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
                {
                    //Add it to the list of controllers active in the menu.
                    if (currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1)
                    {
                        TiedKeybinds newKeybind = new TiedKeybinds();
                        newKeybind.playerNum = currentJoinedPlayerIndex + 1;
                        newKeybind.keybindIndex = i;
                        activeKeybinds.Add(newKeybind);
                    }
                    PlayerJoin(inputString);
                }
            }
		}

        for (int j = 0; j < previewPlayers.Count; j++)
        {
            if (GameInfoManager.Instance.PlayerInputSources[j].ToString() != "")
            {
                string inputSource = GameInfoManager.Instance.PlayerInputSources[j].ToString();
                int inputSourceIndex = 0;

                inputSourceIndex = int.Parse(inputSource.Substring(inputSource.IndexOf(" ") ));

                if (inputSource.Contains("Keybinds"))
                {
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()))
                        ApplyColourPreview(-1, j);
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                        ApplyColourPreview(1, j);
                }
            }
        }

        if (keysHeld.Contains(inputManager.PlayerKeybindArray[0].SelectKey.ToString())) //Start game once enter has been pressed. Make sure to unsub first.
        {
            if (currentJoinedPlayerIndex >= 1) //Prevent a game from starting until there's at least two players.
            {
                LoadGame();
            }
        }
	}

	private void CheckReleasedKeys(int playerNum, List<string> keysReleased)
	{
        for (int i = 0; i < previewPlayers.Count; i++)
        {
            if (GameInfoManager.Instance.PlayerInputSources[i].ToString() != "")
            {
                string inputSource = GameInfoManager.Instance.PlayerInputSources[i].ToString();
                int inputSourceIndex = 0;

                inputSourceIndex = int.Parse(inputSource.Substring(inputSource.IndexOf(" ")));

                if (inputSource.Contains("Keybinds"))
                {
                    if (keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) && keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                    {
                        for (int j = 0; j < activeKeybinds.Count; j++) //Check which player this activeKeybind belongs to and let that player be able to change colour.
                        {
                            if(activeKeybinds[j].keybindIndex == inputSourceIndex)
                                canChangePlayerColourArray[activeKeybinds[j].playerNum] = true;
                        }
                    }
                }
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

	private void PlayerJoin(string inputSource)
	{
		//When someone presses a Join Button, get the input source and tie it to the current player number.
		//Ex: if there's no current player 1, tie it to player 1.
        if(currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1)
            currentJoinedPlayerIndex++;

        //Register their info to the GameInfoManager
        GameInfoManager.Instance.PlayerInputSources[currentJoinedPlayerIndex] = inputSource;
        GameInfoManager.Instance.JoinedPlayers[currentJoinedPlayerIndex] = true;

        //Spawn a sweet little join animation
        GameObject joinAnim = GameObject.Instantiate(Resources.Load("Prefabs/AnimatedPrefabs/JoinAnimation"), panelPositionsArray[currentJoinedPlayerIndex], previewPlayers[currentJoinedPlayerIndex].transform.rotation) as GameObject;
        joinAnim.GetComponent<AnimationObject>().Animation_Complete += MovePanel;

        queuedPlayersJoining.Add(currentJoinedPlayerIndex);

        //Hide the prompt
        for (int i = 0; i < joinPrompts[currentJoinedPlayerIndex].transform.childCount; i++)
        {
            joinPrompts[currentJoinedPlayerIndex].transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }

        //Show the start game prompt if there's at least two people
        if (currentJoinedPlayerIndex == 1)
        {
            GameObject startGamePrompt = GameObject.FindGameObjectWithTag("StartGamePrompt");
            for (int j = 0; j < startGamePrompt.transform.childCount; j++)
            {
                //startGamePrompt.transform.GetChild(j).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                startGamePrompt.AddComponent<TweenComponent>();
                startGamePrompt.GetComponent<TweenComponent>().TweenPositionTo(startGamePrompt_Location, PANEL_MOVESPEED);
            }
        }
	}

    private void ButtonMenuInput(int playerNum, List<string> buttonsHeld)
    {
        if (buttonsHeld.Contains(inputManager.ControllerArray[playerNum].buttonA))
		{
            string inputString = inputManager.ControllerArray[playerNum].ToString() + " " + playerNum;
            //If there aren't 4 players and the input source hasn't already been bound to someone, join the new player.
            if (currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1 && GameInfoManager.Instance.PlayerInputSources.Contains(inputString) == false)
            {
                //Add it to the list of controllers active in the menu.
                if (currentJoinedPlayerIndex < MAX_NUM_OF_PLAYERS - 1)
                {
                    TiedController newActiveController = new TiedController();
                    newActiveController.playerController = inputManager.ControllerArray[playerNum];
                    newActiveController.playerNum = currentJoinedPlayerIndex + 1;

                    activeControllers.Add(newActiveController);
                }

                PlayerJoin(inputString);
            }
		}
        if (buttonsHeld.Contains(inputManager.ControllerArray[playerNum].startButton))
        {
            if (currentJoinedPlayerIndex >= 1) //Prevent a game from starting until there's at least two players.
            {
                LoadGame();
            }
        }
    }

    private void MovePanel()
    {
        if (queuedPlayersJoining.Count > 0)
        {
            GameObject panel = previewPlayers[queuedPlayersJoining[0]].transform.parent.gameObject;
            panel.AddComponent<TweenComponent>();
            panel.GetComponent<TweenComponent>().TweenPositionTo(panelPositionsArray[queuedPlayersJoining[0]], PANEL_MOVESPEED);
            queuedPlayersJoining.Remove(0);
        }
    }

    private void LoadGame()
    {
        inputManager.Key_Pressed -= MenuInput;
        inputManager.Key_Released -= CheckReleasedKeys;
        inputManager.Button_Pressed -= ButtonMenuInput;

        Application.LoadLevel("Main");
    }
}
