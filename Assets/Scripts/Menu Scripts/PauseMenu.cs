using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour 
{
    private InputManager inputManager = null;

    private GameObject[] menuSelections = new GameObject[4];

    private GameManager gameManager = null;

    private int currentSelectionIndex = 0;
    private int ownerPlayerNum = -1;

    public int OwnerPlayerNum
    {
        get { return ownerPlayerNum; }
        set { ownerPlayerNum = value; }
    }

	// Use this for initialization
	void Start () 
	{
        inputManager = InputManager.Instance;
        inputManager.Key_Pressed += ProcessSelection;

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
	
	}

    private void ProcessSelection(int playerNum, List<string> keysPressed)
    {
        if (keysPressed.Contains(inputManager.PlayerKeybindArray[ownerPlayerNum].UpKey.ToString()) || keysPressed.Contains(inputManager.PlayerKeybindArray[ownerPlayerNum].UpKey.ToString()))
        {
            if(currentSelectionIndex - 1 < 0)
                currentSelectionIndex = menuSelections.Length - 1;
            else
                currentSelectionIndex--;
            
            HighlightSelection();
        }

        if (keysPressed.Contains(inputManager.PlayerKeybindArray[ownerPlayerNum].DownKey.ToString()) || keysPressed.Contains(inputManager.PlayerKeybindArray[ownerPlayerNum].DownKey.ToString()))
        {
            if((currentSelectionIndex + 1) >= menuSelections.Length)
                currentSelectionIndex = 0;
            else
                currentSelectionIndex++;

            HighlightSelection();
        }

        if (keysPressed.Contains(inputManager.PlayerKeybindArray[0].SelectKey.ToString()))
        {
            switch(menuSelections[currentSelectionIndex].name)
            {
                case "Text_Resume":
                    if(gameManager.IsGamePaused == true)
                    {
                        inputManager.Key_Pressed -= ProcessSelection;
                        gameManager.HidePauseMenu();
                    }
                    break;
                case "Text_Restart":
                    inputManager.Key_Pressed -= ProcessSelection;
                    gameManager.RestartGame();
                    break;

                case "Text_Controls":
                    inputManager.Key_Pressed -= ProcessSelection;
                    Debug.Log("FUNCTION CALL TO SHOW CONTROLS PAGE");
                    break;
                case "Text_Quit":
                        Application.Quit();
                    break;
                default:
                    if(gameManager.IsGamePaused == true)
                    {
                        inputManager.Key_Pressed -= ProcessSelection;
                        gameManager.HidePauseMenu();
                    }
                    break;
            }
        }
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
}
