using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SplashScreenManager : MonoBehaviour 
{
    //InputManager inputManager = null;


	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Application.loadedLevel == 0) //Only update on the splash screen.
        {
            if (Input.anyKeyDown) //NOTE: This works with both keyboard and controllers!
            {
                StartGame();
            }
        }
	}

    private void ProcessInput(int playerNum, List<string> pressedKeys)
    {
        
    }

    private void StartGame()
    {
        Application.LoadLevel("Menu");
    }
}
