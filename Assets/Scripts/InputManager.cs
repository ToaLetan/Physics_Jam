using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class InputManager
{
	public delegate void ControllerButtonEvent(ButtonState buttonState);

	private static InputManager instance = null;

	public GamePadState[] GamePadStateArray = new GamePadState[4];
	public GamePadState[] PrevGamePadStateArray = new GamePadState[4];

	public static InputManager Instance
	{
		get
		{
			if(instance == null)
				instance = new InputManager();
			return instance;
		}
	}

	// Use this for initialization
	private InputManager()
	{
		for (int i = 0; i < GamePadStateArray.Length; i++) 
		{
			GamePadStateArray[i] = GamePad.GetState((PlayerIndex) i);
			Debug.Log( "Controller " + (i+1) + " connected: " + GamePadStateArray[i].IsConnected);
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{
		UpdateControllerStates();
	}

	private void UpdateControllerStates()
	{
		//Check if each controller is connected, set the previous state to the current state and update the current state.
		for (int i = 0; i < GamePadStateArray.Length; i++) 
		{
			if(GamePad.GetState((PlayerIndex) i).IsConnected)
			{
				PrevGamePadStateArray[i] = GamePadStateArray[i];
				GamePadStateArray[i] = GamePad.GetState((PlayerIndex) i);
			}
			//If the controller is disconnected, maybe pause the game and prompt a reconnect?
		}
	}

	private void UpdateButtonInput()
	{

	}
}
