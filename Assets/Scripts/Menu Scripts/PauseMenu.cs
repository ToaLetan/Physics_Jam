using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour 
{
    private const float THUMBSTICK_DEADZONE = 0.1f;
    private const float MIN_THUMBSTICK_POS = 0.5f; //The minimum thumbstick position where it can affect the menu.

    private InputManager inputManager = null;

    private GameObject[] menuSelections = new GameObject[4];

    private GameManager gameManager = null;

    private string ownerInputSource = "";

    private int currentSelectionIndex = 0;
    private int ownerInputIndex = -1;

    private bool isOnControlsScreen = false;
    private bool canChangeSelection = true; //Used to prevent controllers from moving through selections too fast.

    public string OwnerInputSource
    {
        get { return ownerInputSource; }
        set { ownerInputSource = value; }
    }

    public int OwnerInputIndex
    {
        get { return ownerInputIndex; }
        set { ownerInputIndex = value; }
    }

	// Use this for initialization
	void Start () 
	{
        inputManager = InputManager.Instance;

        //Subscribe to keyboard input events
        inputManager.Key_Pressed += ProcessInput;

        //Subscribe to controller input events
        inputManager.Button_Pressed += ProcessInput;
        inputManager.Left_Thumbstick_Axis += ProcessThumbsticks;

        menuSelections [0] = gameObject.transform.FindChild("Text_Resume").gameObject;
        menuSelections [1] = gameObject.transform.FindChild("Text_Restart").gameObject;
        menuSelections [2] = gameObject.transform.FindChild("Text_Controls").gameObject;
        menuSelections [3] = gameObject.transform.FindChild("Text_Quit").gameObject;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        currentSelectionIndex = 0;
        HighlightSelection();
	}
	
	// Update is called once per frame
	void Update () 
	{
        //TODO: Refactor to use controller events.
        /*if (ownerInputSource.Contains("Controller") == true)
        {
            ProcessInput(ownerInputIndex, null);
        }*/
	}

    private void ProcessInput(int playerNum, List<string> keysButtonsPressed)
    {
            if (keysButtonsPressed.Contains(inputManager.PlayerKeybindArray[ownerInputIndex].UpKey.ToString()) || keysButtonsPressed.Contains(inputManager.PlayerKeybindArray[ownerInputIndex].UpKey.ToString()))
            {
                IncrementSelection(-1);
                HighlightSelection();
            }

            if (keysButtonsPressed.Contains(inputManager.PlayerKeybindArray[ownerInputIndex].DownKey.ToString()) || keysButtonsPressed.Contains(inputManager.PlayerKeybindArray[ownerInputIndex].DownKey.ToString()))
            {
                IncrementSelection(1);
                HighlightSelection();
            }

            if (keysButtonsPressed.Contains(inputManager.PlayerKeybindArray[0].SelectKey.ToString() )  || keysButtonsPressed.Contains(inputManager.ControllerArray[playerNum].buttonA) )
                ProcessSelection();

            if(keysButtonsPressed.Contains(inputManager.ControllerArray[playerNum].buttonB) ) //Close the menu
            {
                inputManager.Key_Pressed -= ProcessInput;
                inputManager.Button_Pressed -= ProcessInput;
                inputManager.Left_Thumbstick_Axis -= ProcessThumbsticks;

                gameManager.HidePauseMenu();
            }
                
    }

    private void ProcessThumbsticks(int playerNum, Vector2 leftThumbstick)
    {
        if (playerNum == OwnerInputIndex)
        {
            if (leftThumbstick.y > MIN_THUMBSTICK_POS)
            {
                if (canChangeSelection == true)
                {
                    IncrementSelection(-1);
                    HighlightSelection();
                    canChangeSelection = false;
                }
            }
            if (leftThumbstick.y < -MIN_THUMBSTICK_POS)
            {
                if (canChangeSelection == true)
                {
                    IncrementSelection(1);
                    HighlightSelection();
                    canChangeSelection = false;
                }
            }

            //Allows menu to be used by flicking thumbsticks, prevents flickering.
            if (leftThumbstick.y < THUMBSTICK_DEADZONE && leftThumbstick.y > -THUMBSTICK_DEADZONE)
            {
                canChangeSelection = true;
            }
        }
    }

    private void IncrementSelection(int index)
    {
        if (currentSelectionIndex + index < 0)
            currentSelectionIndex = menuSelections.Length - 1;
        
        else if ((currentSelectionIndex + index) >= menuSelections.Length)
            currentSelectionIndex = 0;
        
        else
            currentSelectionIndex += index;
    }

    private void HighlightSelection()
    {
        if (menuSelections [currentSelectionIndex].GetComponent<SpriteRenderer>() != null)
        {
            menuSelections [currentSelectionIndex].GetComponent<SpriteRenderer>().color = Color.cyan;
        }

        //De-highlight any previous selections.
        for(int i = 0; i < menuSelections.Length; i++)
        {
            if(i != currentSelectionIndex)
                menuSelections [i].GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void ProcessSelection()
    {
        switch (menuSelections[currentSelectionIndex].name)
        {
            case "Text_Resume":
                if (gameManager.IsGamePaused == true)
                {
                    inputManager.Key_Pressed -= ProcessInput;
                    inputManager.Button_Pressed -= ProcessInput;
                    inputManager.Left_Thumbstick_Axis -= ProcessThumbsticks;
                    gameManager.HidePauseMenu();
                }
                break;
            case "Text_Restart":
                inputManager.Key_Pressed -= ProcessInput;
                inputManager.Button_Pressed -= ProcessInput;
                inputManager.Left_Thumbstick_Axis -= ProcessThumbsticks;
                gameManager.RestartGame();
                break;

            case "Text_Controls":
                if (isOnControlsScreen == false)
                {
                    inputManager.Key_Pressed -= ProcessInput;
                    inputManager.Button_Pressed -= ProcessInput;
                    inputManager.Left_Thumbstick_Axis -= ProcessThumbsticks;
                    ToggleControlsScreen(true);
                }
                break;
            case "Text_Quit":
                Application.Quit();
                break;
            default:
                if (gameManager.IsGamePaused == true)
                {
                    inputManager.Key_Pressed -= ProcessInput;
                    inputManager.Button_Pressed -= ProcessInput;
                    inputManager.Left_Thumbstick_Axis -= ProcessThumbsticks;
                    gameManager.HidePauseMenu();
                }
                break;
        }
    }

    public void ToggleControlsScreen(bool showControlsScreen)
    {
        isOnControlsScreen = showControlsScreen; //True if the controls screen is being shown, otherwise false.

        //Hide the Pause menu
        if (showControlsScreen == true)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                }
            }

            GameObject controlsScreen = GameObject.Instantiate(Resources.Load("Prefabs/GUI/ControlsScreen")) as GameObject;
            controlsScreen.transform.parent = gameObject.transform;
            controlsScreen.transform.localPosition = Vector3.zero;
            controlsScreen.transform.localScale = new Vector3(0.772f, 0.772f, 0); //Set the scale because it gets messed up when setting parent. HARDCODED AND GHETTO FOR NOW, FIX LATER?

            controlsScreen.GetComponent<ControlsScreen>().TieInput(ownerInputSource, ownerInputIndex);
        }
        else
        {
            //Remove the controls screen
            if (gameObject.transform.FindChild("ControlsScreen(Clone)") != null)
            {
                gameObject.transform.FindChild("ControlsScreen(Clone)").GetComponent<ControlsScreen>().RemoveControlsScreen();
            }

            //Show the pause menu
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }
            }
            HighlightSelection();

            //Resub to events
            inputManager.Key_Pressed += ProcessInput;
            inputManager.Button_Pressed += ProcessInput;
            inputManager.Left_Thumbstick_Axis += ProcessThumbsticks;
        }
    }
}