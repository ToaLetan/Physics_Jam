using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlsScreen : MonoBehaviour 
{
    private InputManager inputManager = InputManager.Instance;

    private int currentPageIndex = 0;

	// Use this for initialization
	void Start () 
    {
        inputManager.Key_Pressed += ProcessInput;
        ChangeSelection(0);
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ProcessInput(int playerNum, List<string> keysPressed)
    {
        if (keysPressed.Contains(inputManager.PlayerKeybindArray[playerNum].LeftKey.ToString()))
        {
            if (currentPageIndex == 1)
                ChangeSelection(0);
        }
        if (keysPressed.Contains(inputManager.PlayerKeybindArray[playerNum].RightKey.ToString()))
        {
            if (currentPageIndex == 0)
                ChangeSelection(1);
        }

        if (keysPressed.Contains(inputManager.PlayerKeybindArray[0].SelectKey.ToString())) //Go back to the Pause Menu.
        {
            if (gameObject.transform.parent.GetComponent<PauseMenu>() != null)
            {
                gameObject.transform.parent.GetComponent<PauseMenu>().ToggleControlsScreen(false);
            }
        }
    }

    public void RemoveControlsScreen()
    {
        inputManager.Key_Pressed -= ProcessInput;
        Destroy(gameObject);
    }

    private void ChangeSelection(int pageIndex)
    {
        currentPageIndex = pageIndex;

        switch(pageIndex)
        {
            case 0:
                HighlightTitle("Keyboard");
                break;
            case 1:
                HighlightTitle("Controller");
                break;
        }
    }

    private void HighlightTitle(string title)
    {
        string selectionTitle = "Selection_" + title;
        string controlsTitle = "Controls_" + title;

        string previousTitle = "";

        if (title == "Keyboard")
            previousTitle = "Controller";
        if (title == "Controller")
            previousTitle = "Keyboard";

        string previousSelectionTitle = "Selection_" + previousTitle;
        string previousControlsTitle = "Controls_" + previousTitle;

        //Set the highlight on the top bar
        if (gameObject.transform.FindChild(selectionTitle) != null && gameObject.transform.FindChild(previousSelectionTitle).GetComponent<SpriteRenderer>() != null)
            gameObject.transform.FindChild(selectionTitle).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Menus/" + selectionTitle + "_Highlighted");

        //Set the control scheme to display
        if (gameObject.transform.FindChild("Controls_Keyboard") != null && gameObject.transform.FindChild("Controls_Keyboard").GetComponent<SpriteRenderer>() != null)
        {
            gameObject.transform.FindChild("Controls_Keyboard").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Menus/" + controlsTitle);
        }  

        //Un-highlight the previously highlighted bar section
        if (gameObject.transform.FindChild(previousSelectionTitle) != null && gameObject.transform.FindChild(previousSelectionTitle).GetComponent<SpriteRenderer>() != null)
            gameObject.transform.FindChild(previousSelectionTitle).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Menus/" + previousSelectionTitle);
    }
}
