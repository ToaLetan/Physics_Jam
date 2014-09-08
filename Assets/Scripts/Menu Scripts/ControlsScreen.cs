using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlsScreen : MonoBehaviour 
{
    private InputManager inputManager = InputManager.Instance;

	// Use this for initialization
	void Start () 
    {
        inputManager.Key_Pressed += ProcessInput;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void ProcessInput(int playerNum, List<string> keysPressed)
    {
        if (keysPressed.Contains(inputManager.PlayerKeybindArray[0].SelectKey.ToString()))
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
}
