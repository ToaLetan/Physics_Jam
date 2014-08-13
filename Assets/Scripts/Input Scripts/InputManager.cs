﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Keybinds
{
	//Movement Keys
	public KeyCode UpKey;
	public KeyCode DownKey;
	public KeyCode LeftKey;
	public KeyCode RightKey;

	//Beam keys
	public KeyCode LTurnKey;
	public KeyCode RTurnKey;

	//Physics manipulation keys
	public KeyCode GraborThrowKey;

	//Menu Keys
	public KeyCode SelectKey;
	public KeyCode ExitKey;
}

public class InputManager
{
    //Events for keyboard input
	public delegate void KeyHeldEvent(int playerNum, List<string> heldKeys);
	public delegate void KeyPressedEvent(int playerNum, List<string> pressedKeys);
	public delegate void KeyReleasedEvent(int playerNum, List<string> releasedKeys);

	public event KeyHeldEvent Key_Held;
	public event KeyPressedEvent Key_Pressed;
	public event KeyReleasedEvent Key_Released;

    //Events for controller input
    public delegate void ButtonHeldEvent(int playerNum, List<string> heldButtons);
    public delegate void ButtonPressedEvent(int playerNum, List<string> pressedButtons);
    public delegate void ButtonReleasedEvent(int playerNum, List<string> releasedButtons);
    public delegate void ThumbstickEvent(int playerNum, Vector2 thumbstickVector);

    public event ButtonHeldEvent Button_Held;
    public event ButtonPressedEvent Button_Pressed;
    public event ButtonReleasedEvent Button_Released;

	public Keybinds[] PlayerKeybindArray = new Keybinds[2]; //Keybinds for up to two keyboard-based players.
	public XboxController[] ControllerArray = new XboxController[4]; //Supporting up to 4 connected controllers.
	
	private static InputManager instance = null;

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
		//Hard-setting Keybinds for players 1 and 2 here for now. Eventually implement options.
		PlayerKeybindArray [0].UpKey = KeyCode.W;
		PlayerKeybindArray [0].DownKey = KeyCode.S;
		PlayerKeybindArray [0].LeftKey = KeyCode.A;
		PlayerKeybindArray [0].RightKey = KeyCode.D;
		PlayerKeybindArray [0].LTurnKey = KeyCode.Q;
		PlayerKeybindArray [0].RTurnKey = KeyCode.E;
		PlayerKeybindArray [0].GraborThrowKey = KeyCode.F;

		PlayerKeybindArray [1].UpKey = KeyCode.I;
		PlayerKeybindArray [1].DownKey = KeyCode.K;
		PlayerKeybindArray [1].LeftKey = KeyCode.J;
		PlayerKeybindArray [1].RightKey = KeyCode.L;
		PlayerKeybindArray [1].LTurnKey = KeyCode.U;
		PlayerKeybindArray [1].RTurnKey = KeyCode.O;
		PlayerKeybindArray [1].GraborThrowKey = KeyCode.Semicolon;

		PlayerKeybindArray [0].SelectKey = KeyCode.Return;
		PlayerKeybindArray [0].ExitKey = KeyCode.Escape;

		for (int i = 0; i < ControllerArray.Length; i++)
		{
			ControllerArray[i] = new XboxController(i);
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{
		UpdateButtonInput ();
	}


	private void UpdateButtonInput()
	{
        //================================= KEYBOARD INPUT FOR KEYS HELD/RELEASED ================================= 
		for (int i = 0; i < PlayerKeybindArray.Length; i++) 
		{
			//Check all keys being held.
			List<string> allHeldKeys = new List<string>();

			if(Input.GetKey(PlayerKeybindArray [i].UpKey))
				allHeldKeys.Add(PlayerKeybindArray [i].UpKey.ToString() );
			if(Input.GetKey(PlayerKeybindArray [i].DownKey))
                allHeldKeys.Add(PlayerKeybindArray[i].DownKey.ToString() );
			if(Input.GetKey(PlayerKeybindArray [i].LeftKey))
                allHeldKeys.Add(PlayerKeybindArray[i].LeftKey.ToString() );
			if(Input.GetKey(PlayerKeybindArray [i].RightKey))
                allHeldKeys.Add(PlayerKeybindArray[i].RightKey.ToString() );
			if(Input.GetKey(PlayerKeybindArray [i].LTurnKey))
                allHeldKeys.Add(PlayerKeybindArray[i].LTurnKey.ToString() );
			if(Input.GetKey(PlayerKeybindArray [i].RTurnKey))
                allHeldKeys.Add(PlayerKeybindArray[i].RTurnKey.ToString() );
			if(Input.GetKey(PlayerKeybindArray [i].GraborThrowKey))
                allHeldKeys.Add(PlayerKeybindArray[i].GraborThrowKey.ToString() );

			if(Input.GetKey(PlayerKeybindArray [i].SelectKey))
                allHeldKeys.Add(PlayerKeybindArray[i].SelectKey.ToString() );

			if(allHeldKeys.Count > 0)
			{
				if(Key_Held != null)
					Key_Held(i, allHeldKeys);
			}

			//Check all keys being pressed.
            List<string> allPressedKeys = new List<string>();

			if(Input.GetKeyDown(PlayerKeybindArray [i].UpKey))
				allPressedKeys.Add(PlayerKeybindArray [i].UpKey.ToString() );
			if(Input.GetKeyDown(PlayerKeybindArray [i].DownKey))
                allPressedKeys.Add(PlayerKeybindArray[i].DownKey.ToString() );
			if(Input.GetKeyDown(PlayerKeybindArray [i].LeftKey))
                allPressedKeys.Add(PlayerKeybindArray[i].LeftKey.ToString() );
			if(Input.GetKeyDown(PlayerKeybindArray [i].RightKey))
                allPressedKeys.Add(PlayerKeybindArray[i].RightKey.ToString() );
			if(Input.GetKeyDown(PlayerKeybindArray [i].LTurnKey))
                allPressedKeys.Add(PlayerKeybindArray[i].LTurnKey.ToString() );
			if(Input.GetKeyDown(PlayerKeybindArray [i].RTurnKey))
                allPressedKeys.Add(PlayerKeybindArray[i].RTurnKey.ToString() );
			if(Input.GetKeyDown(PlayerKeybindArray [i].GraborThrowKey))
                allPressedKeys.Add(PlayerKeybindArray[i].GraborThrowKey.ToString() );
			
			if(Input.GetKeyDown(PlayerKeybindArray [i].SelectKey))
                allPressedKeys.Add(PlayerKeybindArray[i].SelectKey.ToString() );

			if(allPressedKeys.Count > 0)
			{
				if(Key_Pressed != null)
					Key_Pressed(i, allPressedKeys);
			}

			//Check all keys being released.
			List<string> allReleasedKeys = new List<string>();
			
			if(!Input.GetKey(PlayerKeybindArray [i].UpKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].UpKey.ToString() );
			if(!Input.GetKey(PlayerKeybindArray [i].DownKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].DownKey.ToString() );
			if(!Input.GetKey(PlayerKeybindArray [i].LeftKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].LeftKey.ToString() );
			if(!Input.GetKey(PlayerKeybindArray [i].RightKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].RightKey.ToString() );
			if(!Input.GetKey(PlayerKeybindArray [i].LTurnKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].LTurnKey.ToString() );
			if(!Input.GetKey(PlayerKeybindArray [i].RTurnKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].RTurnKey.ToString() );
			if(!Input.GetKey(PlayerKeybindArray [i].GraborThrowKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].GraborThrowKey.ToString() );
			
			if(allReleasedKeys.Count > 0)
			{
				if(Key_Released != null)
					Key_Released(i, allReleasedKeys);
			}
		}
        //=========================================================================================================== 

        //================================= CONTROLLER INPUT FOR BUTTONS HELD/RELEASED ==============================
        for (int j = 0; j < ControllerArray.Length; j++)
        {
                //Check all buttons being held.
                List<string> allHeldButtons = new List<string>();
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonA))
                    allHeldButtons.Add(ControllerArray[j].buttonA);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonB))
                    allHeldButtons.Add(ControllerArray[j].buttonB);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonX))
                    allHeldButtons.Add(ControllerArray[j].buttonX);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonY))
                    allHeldButtons.Add(ControllerArray[j].buttonY);

                if (allHeldButtons.Count > 0)
                {
                    if (Button_Held != null)
                        Button_Held(j, allHeldButtons);

                    allHeldButtons.Clear();
                }

                
                //Check all buttons being pressed.
                List<string> allPressedButtons = new List<string>();
                if (ControllerArray[j].GetButtonDown(ControllerArray[j].buttonA))
                    allPressedButtons.Add(ControllerArray[j].buttonA);
                if (ControllerArray[j].GetButtonDown(ControllerArray[j].buttonB))
                    allPressedButtons.Add(ControllerArray[j].buttonB);
                if (ControllerArray[j].GetButtonDown(ControllerArray[j].buttonX))
                    allPressedButtons.Add(ControllerArray[j].buttonX);
                if (ControllerArray[j].GetButtonDown(ControllerArray[j].buttonY))
                    allPressedButtons.Add(ControllerArray[j].buttonY);

                if (allPressedButtons.Count > 0)
                {
                    if (Button_Pressed != null)
                        Button_Pressed(j, allPressedButtons);

                    allHeldButtons.Clear();
                }

                //Check all buttons being released.
                List<string> allReleasedButtons = new List<string>();
                if (!ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonA))
                    allReleasedButtons.Add(ControllerArray[j].buttonA);
                if (!ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonB))
                    allReleasedButtons.Add(ControllerArray[j].buttonB);
                if (!ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonX))
                    allReleasedButtons.Add(ControllerArray[j].buttonX);
                if (!ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonY))
                    allReleasedButtons.Add(ControllerArray[j].buttonY);

                if (allReleasedButtons.Count > 0)
                {
                    if (Button_Released != null)
                        Button_Released(j, allReleasedButtons);

                    allReleasedButtons.Clear();
                }
            }
	}
}
