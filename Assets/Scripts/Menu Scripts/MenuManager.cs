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
    private const float MIN_THUMBSTICK_POS = 0.5f; //The minimum thumbstick position where it can affect the menu.
    private const float PANEL_MOVESPEED = 5.0f;

	private const int NUM_OF_COLOURS = 8;
    private const int MAX_NUM_OF_PLAYERS = 4;
    private const int NUM_OF_ABILITIES = 6;

    private enum PlayerJoinStatus { NotJoined = 0, ColourSelect, AbilitySelect, Done };

	private List<GameObject> previewPlayers = new List<GameObject>();
    private List<GameObject> joinPrompts = new List<GameObject>();
    private List<GameObject> abilitySelections = new List<GameObject>();
    private List<TiedController> activeControllers = new List<TiedController>();
    private List<TiedKeybinds> activeKeybinds = new List<TiedKeybinds>();
    public List<int> queuedPlayersJoining = new List<int>();

    private PlayerJoinStatus[] playerStatuses = new PlayerJoinStatus[MAX_NUM_OF_PLAYERS]; //Array of bools used to determine if the player is selecting an Ability. False if haven't picked colour first.
	private Color[] colourArray = new Color[NUM_OF_COLOURS]; //Array of possible colours
    private Vector3[] panelPositionsArray = new Vector3[MAX_NUM_OF_PLAYERS]; //Array of desired locations for panels to move to.
    private int[] playerColourIndexArray = new int[MAX_NUM_OF_PLAYERS]; //Array of current colour indices (ex. red = 1, blue = 2, etc.)
    private int[] playerAbilityIndexArray = new int[MAX_NUM_OF_PLAYERS];
    private bool[] canChangePlayerColourArray = new bool[MAX_NUM_OF_PLAYERS]; //Array of bools used to determine if the player can change colour, prevents holding keys to flicker through colours.

    private float panelDestinationX = -0.42f;

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
            playerColourIndexArray[i] = -1; //Set to -1 to default as white.
            playerAbilityIndexArray[i] = 1; //Goes from 1-6 for each of the Abilities.
            canChangePlayerColourArray[i] = true;
            playerStatuses[i] = PlayerJoinStatus.NotJoined;

            //Establish all desired panel positions for when they need to be moved on-screen.
            panelPositionsArray[i] = new Vector3(panelDestinationX, previewPlayers[i].transform.parent.transform.position.y, previewPlayers[i].transform.parent.transform.position.z);

            //Hide all Ability selections
            ShowHideAbilitySelection(i, false);
        }

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
            if (activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.dPadHorizontal) > MIN_THUMBSTICK_POS ||
                activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.leftThumbstickHorizontal) > MIN_THUMBSTICK_POS)
            {
                if (playerStatuses[activeControllers[i].playerNum] == PlayerJoinStatus.ColourSelect)
                    ApplyColourPreview(1, activeControllers[i].playerNum);
                else if (playerStatuses[activeControllers[i].playerNum] == PlayerJoinStatus.AbilitySelect)
                    SelectAbility(1, activeControllers[i].playerNum);
            }

            if (activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.dPadHorizontal) < -MIN_THUMBSTICK_POS ||
                activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.leftThumbstickHorizontal) < -MIN_THUMBSTICK_POS)
            {
                if (playerStatuses[activeControllers[i].playerNum] == PlayerJoinStatus.ColourSelect)
                    ApplyColourPreview(-1, activeControllers[i].playerNum);
                else if (playerStatuses[activeControllers[i].playerNum] == PlayerJoinStatus.AbilitySelect)
                    SelectAbility(-1, activeControllers[i].playerNum);
            }

            if (activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.dPadHorizontal) == 0 &&
                (activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.leftThumbstickHorizontal) < THUMBSTICK_DEADZONE &&
                activeControllers[i].playerController.GetThumbstickTriggerAxis(activeControllers[i].playerController.leftThumbstickHorizontal) > -THUMBSTICK_DEADZONE))
            {
                canChangePlayerColourArray[activeControllers[i].playerNum] = true;
            }
        }
    }

	private void PopulateColours()
	{
		//THIS IS ALL REALLY TEMPORARY
		colourArray[0] = Color.red;
		colourArray[1] = Color.blue;
		colourArray[2] = Color.green;
		colourArray[3] = Color.yellow;
		colourArray[4] = new Color(1, 0, 1); //Pink
		colourArray[5] = new Color(0.65f, 0.03f, 0.98f); //Purple
		colourArray[6] = new Color(1, 0.51f, 0.11f); //Orange
		colourArray[7] = Color.cyan;
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
            abilitySelections.Add(topmostObject.transform.parent.FindChild("CooldownSelect").gameObject);
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
            //Collect all colours currently being used by other players
            List<int> occupiedColours = new List<int>();

            for (int i = 0; i < playerColourIndexArray.Length; i++)
            {
                if (playerColourIndexArray[i] > -1)
                    occupiedColours.Add(playerColourIndexArray[i]);
            }

            IncrementPlayerColour(incrementation, playerNum); //Change the colour.

            for (int j = 0; j < occupiedColours.Count; j++)
            {
                if (occupiedColours.Contains(playerColourIndexArray[playerNum]))
                {
                    IncrementPlayerColour(incrementation, playerNum); //Attempt another change.
                }
            }

            newColour = colourArray[playerColourIndexArray[playerNum]]; //Set the player colour

            canChangePlayerColourArray[playerNum] = false; //Disable being able to change colour to slow the process (not causing seizures obviously being a plus.)

            GameInfoManager.Instance.PlayerColours[playerNum] = newColour; //Tie the player colour to that player so the GameInfoManager can pass it into the game.

            //Apply the colour to the player previews and their ability cooldown icon.
            previewPlayers[playerNum].transform.FindChild("PreviewPlayer").GetChild(0).GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("ColourPreview").GetComponent<SpriteRenderer>().color = newColour;
            previewPlayers[playerNum].transform.FindChild("Text_Player" + (playerNum + 1)).GetComponent<SpriteRenderer>().color = newColour;
            abilitySelections[playerNum].transform.FindChild("Cooldown_Base").GetComponent<SpriteRenderer>().color = newColour;
        }
	}

    private void IncrementPlayerColour(int incrementation, int playerNum)
    {
        playerColourIndexArray[playerNum] += incrementation;

        //Wrap around if the player goes all the way left/right
        if (playerColourIndexArray[playerNum] >= colourArray.Length)
            playerColourIndexArray[playerNum] = 0;
        if (playerColourIndexArray[playerNum] < 0)
            playerColourIndexArray[playerNum] = colourArray.Length - 1;
    }

    private void SelectAbility(int incrementation, int playerNum)
    {
        if (canChangePlayerColourArray[playerNum] == true) //Sharing this to prevent ability switching from going too fast.
        {
            playerAbilityIndexArray[playerNum] += incrementation;

            if (playerAbilityIndexArray[playerNum] > NUM_OF_ABILITIES)
                playerAbilityIndexArray[playerNum] = 1;
            if (playerAbilityIndexArray[playerNum] < 1)
                playerAbilityIndexArray[playerNum] = NUM_OF_ABILITIES;

            string cooldownName = "";

            switch(playerAbilityIndexArray[playerNum]) //Set the player ability based on the index from 1-6.
            {
                case 1:
                    GameInfoManager.Instance.PlayerActives[playerNum] = Active.ActiveType.GravField;
                    cooldownName = "GravField";
                    break;
                case 2:
                    GameInfoManager.Instance.PlayerActives[playerNum] = Active.ActiveType.Reflect;
                    cooldownName = "Reflect";
                    break;
                case 3:
                    GameInfoManager.Instance.PlayerActives[playerNum] = Active.ActiveType.SlipGel;
                    cooldownName = "SlipGel";
                    break;
                case 4:
                    GameInfoManager.Instance.PlayerActives[playerNum] = Active.ActiveType.SlowGel;
                    cooldownName = "SlowGel";
                    break;
                case 5:
                    GameInfoManager.Instance.PlayerActives[playerNum] = Active.ActiveType.Overclock;
                    cooldownName = "Overclock";
                    break;
                case 6:
                    GameInfoManager.Instance.PlayerActives[playerNum] = Active.ActiveType.Soak;
                    cooldownName = "Soak";
                    break;
                default:
                    break;
            }

            //Update the Ability info display for icons, name, and description.
            abilitySelections[playerNum].transform.FindChild("Cooldown_Base").GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Cooldowns/Cooldown_" + cooldownName);
            abilitySelections[playerNum].transform.FindChild("AbilityName").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Menus/Text_" + cooldownName);
            abilitySelections[playerNum].transform.FindChild("AbilityDescription").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Menus/Description_" + cooldownName);

            canChangePlayerColourArray[playerNum] = false;
        }
    }

	private void MenuInput(int playerNum, List<string> keysHeld)
	{
        //Keyboard input for determining new players
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

        //Keyboard input for existing players
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
                    {
                        if (playerStatuses[j] == PlayerJoinStatus.ColourSelect)
                            ApplyColourPreview(-1, j);
                        else if (playerStatuses[j] == PlayerJoinStatus.AbilitySelect)
                            SelectAbility(-1, j);
                    }
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                    {
                        if (playerStatuses[j] == PlayerJoinStatus.ColourSelect)
                            ApplyColourPreview(1, j);
                        else if (playerStatuses[j] == PlayerJoinStatus.AbilitySelect)
                            SelectAbility(1, j);
                    }
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].GraborThrowKey.ToString()))
                    {
                        if (playerStatuses[j] == PlayerJoinStatus.ColourSelect) //If the player is still selecting a colour, apply the colour and move on to Ability selection.
                        {
                            //Used to prevent the player status from immediately changing.
                            if (previewPlayers[j].transform.parent.gameObject.transform.position.x >= panelDestinationX - 0.05f 
                                && previewPlayers[j].transform.parent.gameObject.transform.position.x <= panelDestinationX + 0.05f) 
                            {
                                playerStatuses[j] = PlayerJoinStatus.AbilitySelect;
                                UpdateSidePrompt(j);

                                //Hide the colour selection and show ability selection
                                ShowHideColourSelection(j, false);
                                ShowHideAbilitySelection(j, true);
                            }
                        }
                    }
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

        //Default them to a colour.
        ApplyColourPreview(1 + currentJoinedPlayerIndex, currentJoinedPlayerIndex);

        //Queue them up for handling panel movement.
        queuedPlayersJoining.Add(currentJoinedPlayerIndex);

        //Spawn a sweet little join animation
        GameObject joinAnim = GameObject.Instantiate(Resources.Load("Prefabs/AnimatedPrefabs/JoinAnimation"), panelPositionsArray[currentJoinedPlayerIndex], previewPlayers[currentJoinedPlayerIndex].transform.rotation) as GameObject;
        joinAnim.GetComponent<AnimationObject>().Animation_Complete += MovePanel;

        //Hide the prompt
        /*for (int i = 0; i < joinPrompts[currentJoinedPlayerIndex].transform.childCount; i++)
        {
            joinPrompts[currentJoinedPlayerIndex].transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }*/

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

        //Set the player status and update their prompt.
        playerStatuses[currentJoinedPlayerIndex] = PlayerJoinStatus.ColourSelect;

        UpdateSidePrompt(currentJoinedPlayerIndex);
	}

    private void ButtonMenuInput(int playerNum, List<string> buttonsHeld)
    {
        //Check any unregistered controllers (ones that aren't tied to a player)
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
        else if (buttonsHeld.Contains(inputManager.ControllerArray[playerNum].startButton))
        {
            if (currentJoinedPlayerIndex >= 1) //Prevent a game from starting until there's at least two players.
            {
                LoadGame();
            }
        }

        //Check registered controllers
        for (int i = 0; i < activeControllers.Count; i++)
        {
            if (activeControllers[i].playerController.GetButtonHeld(activeControllers[i].playerController.buttonA) )
            {
                int controllerPlayerNum = activeControllers[i].playerNum;

                if (playerStatuses[controllerPlayerNum] == PlayerJoinStatus.ColourSelect) //If the player is still selecting a colour, apply the colour and move on to Ability selection.
                {
                    //Used to prevent colour selection from immediately being skipped.
                    if (previewPlayers[controllerPlayerNum].transform.parent.gameObject.transform.position.x >= panelDestinationX - 0.05f
                                && previewPlayers[controllerPlayerNum].transform.parent.gameObject.transform.position.x <= panelDestinationX + 0.05f)
                    {
                        playerStatuses[controllerPlayerNum] = PlayerJoinStatus.AbilitySelect;
                        UpdateSidePrompt(controllerPlayerNum);

                        ShowHideAbilitySelection(controllerPlayerNum, true);
                        ShowHideColourSelection(controllerPlayerNum, false);
                    }
                }
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

            queuedPlayersJoining.RemoveAt(0);
        }
    }

    private void LoadGame()
    {
        inputManager.Key_Pressed -= MenuInput;
        inputManager.Key_Released -= CheckReleasedKeys;
        inputManager.Button_Pressed -= ButtonMenuInput;

        Application.LoadLevel("Main");
    }

    private void ShowHideAbilitySelection(int previewIndex, bool showAbility = false)
    {
        for (int i = 0; i < abilitySelections[previewIndex].transform.childCount; i++)
        {
            if (abilitySelections[previewIndex].transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
            {
                abilitySelections[previewIndex].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = showAbility;

                if (abilitySelections[previewIndex].transform.GetChild(i).name == "Cooldown_Base")
                    abilitySelections[previewIndex].transform.GetChild(i).GetChild(0).GetComponent<SpriteRenderer>().enabled = showAbility;
            }
        }
    }

    private void ShowHideColourSelection(int previewIndex, bool showColour = false)
    {
        for (int i = 0; i < previewPlayers[previewIndex].transform.childCount; i++)
        {
            if (previewPlayers[previewIndex].transform.GetChild(i).gameObject.name != "PreviewPlayer")
                previewPlayers[previewIndex].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = showColour;
        } 
    }

    private void UpdateSidePrompt(int playerNum)
    {
        Debug.Log(playerNum);

        Vector3 newPromptPos = Vector3.zero;

        Sprite textSprite = joinPrompts[playerNum].transform.FindChild("Join_Text").GetComponent<SpriteRenderer>().sprite;

        switch(playerStatuses[playerNum])
        {
            case PlayerJoinStatus.NotJoined:
                textSprite = Resources.Load<Sprite>("Sprites/UI/Menus/Join_Text");
                break;
            case PlayerJoinStatus.ColourSelect:
                textSprite = Resources.Load<Sprite>("Sprites/UI/Menus/Text_SelectColour");
                break;
            case PlayerJoinStatus.AbilitySelect:
                textSprite = Resources.Load<Sprite>("Sprites/UI/Menus/Text_SelectAbility");
                break;
            case PlayerJoinStatus.Done:
                textSprite = Resources.Load<Sprite>("Sprites/UI/Menus/Text_Ready");
                break;
        }

        if (GameInfoManager.Instance.PlayerInputSources[playerNum].Contains("Keybinds 0") ) //Uses the F key, hide other prompts and center the F prompt
        {
            joinPrompts[playerNum].transform.FindChild("KEYBOARD_SEMICOLON").GetComponent<SpriteRenderer>().enabled = false;
            joinPrompts[playerNum].transform.FindChild("XBONE_A").GetComponent<SpriteRenderer>().enabled = false;

            newPromptPos = joinPrompts[playerNum].transform.FindChild("KEYBOARD_F").position;
            newPromptPos.x = joinPrompts[playerNum].transform.FindChild("Join_Text").position.x;

            joinPrompts[playerNum].transform.FindChild("KEYBOARD_F").position = newPromptPos;

            RemovePromptFromOthers(playerNum, "KEYBOARD_F"); //Hide the F Key from all other prompts, since there's only one.
        }
        else if (GameInfoManager.Instance.PlayerInputSources[playerNum].Contains("Keybinds 1")) //Uses the ; key, hide other prompts and center the ; prompt
        {
            joinPrompts[playerNum].transform.FindChild("KEYBOARD_F").GetComponent<SpriteRenderer>().enabled = false;
            joinPrompts[playerNum].transform.FindChild("XBONE_A").GetComponent<SpriteRenderer>().enabled = false;

            newPromptPos = joinPrompts[playerNum].transform.FindChild("KEYBOARD_SEMICOLON").position;
            newPromptPos.x = joinPrompts[playerNum].transform.FindChild("Join_Text").position.x;

            joinPrompts[playerNum].transform.FindChild("KEYBOARD_SEMICOLON").position = newPromptPos;

            RemovePromptFromOthers(playerNum, "KEYBOARD_SEMICOLON"); //Hide the Semicolon Key from all other prompts, since there's only one.
        }
        else if (GameInfoManager.Instance.PlayerInputSources[playerNum].Contains("Controller")) //Uses an Xbox controller, hide other prompts and center the A button prompt
        {
            joinPrompts[playerNum].transform.FindChild("KEYBOARD_F").GetComponent<SpriteRenderer>().enabled = false;
            joinPrompts[playerNum].transform.FindChild("KEYBOARD_SEMICOLON").GetComponent<SpriteRenderer>().enabled = false;

            newPromptPos = joinPrompts[playerNum].transform.FindChild("XBONE_A").position;
            newPromptPos.x = joinPrompts[playerNum].transform.FindChild("Join_Text").position.x;

            joinPrompts[playerNum].transform.FindChild("XBONE_A").position = newPromptPos;
        }

        joinPrompts[playerNum].transform.FindChild("Join_Text").GetComponent<SpriteRenderer>().sprite = textSprite;
    }

    private void RemovePromptFromOthers(int claimedUser, string promptNameToHide) //Used to hide keyboard prompts claimed by other users.
    {
        for (int i = 0; i < joinPrompts.Count; i++)
        {
            if (i != claimedUser)
            {
                joinPrompts[i].transform.FindChild(promptNameToHide).GetComponent<SpriteRenderer>().enabled = false;

                //Reposition the other prompts based on how many are left.
                int numPromptsVisible = 0;

                //First, count up all of the visible prompts.
                for (int j = 0; j < joinPrompts[i].transform.childCount; j++)
                {
                    if (joinPrompts[i].transform.GetChild(j).name != "Join_Text")
                    {
                        if (joinPrompts[i].transform.GetChild(j).GetComponent<SpriteRenderer>() != null && joinPrompts[i].transform.GetChild(j).GetComponent<SpriteRenderer>().enabled == true)
                            numPromptsVisible++;
                    }
                }

                if (numPromptsVisible == 2) //Center both prompts.
                {
                    for (int k = 0; k < joinPrompts[i].transform.childCount; k++)
                    {
                        if (joinPrompts[i].transform.GetChild(k).name != "Join_Text")
                        {
                            Vector3 newPos = joinPrompts[i].transform.GetChild(k).position;
                            newPos.x -= joinPrompts[i].transform.GetChild(k).GetComponent<SpriteRenderer>().bounds.extents.x * 1.5f;

                            joinPrompts[i].transform.GetChild(k).position = newPos;
                        }
                    }
                }
                else if (numPromptsVisible == 1) //Center the only prompt.
                {
                    for (int l = 0; l < joinPrompts[i].transform.childCount; l++)
                    {
                        if (joinPrompts[i].transform.GetChild(l).name != "Join_Text")
                        {
                            Vector3 newPos = joinPrompts[i].transform.GetChild(l).position;
                            newPos.x = joinPrompts[i].transform.FindChild("Join_Text").position.x;

                            joinPrompts[i].transform.GetChild(l).position = newPos;
                        }
                    }
                }
            }
        }
    }

}
