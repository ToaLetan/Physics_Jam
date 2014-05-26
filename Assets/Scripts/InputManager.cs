using UnityEngine;
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
}

public class InputManager
{
	public delegate void KeyHeldEvent(int playerNum, List<KeyCode> heldKeys);
	public delegate void KeyReleasedEvent(int playerNum, List<KeyCode> releasedKeys);

	public event KeyHeldEvent Key_Pressed;
	public event KeyReleasedEvent Key_Released;

	public Keybinds[] PlayerKeybindArray = new Keybinds[2];
	
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
	}
	
	// Update is called once per frame
	public void Update () 
	{
		UpdateButtonInput ();
	}


	private void UpdateButtonInput()
	{
		for (int i = 0; i < PlayerKeybindArray.Length; i++) 
		{
			//Check all keys being held.
			List<KeyCode> allHeldKeys = new List<KeyCode>();

			if(Input.GetKey(PlayerKeybindArray [i].UpKey))
				allHeldKeys.Add(PlayerKeybindArray [i].UpKey);
			if(Input.GetKey(PlayerKeybindArray [i].DownKey))
				allHeldKeys.Add(PlayerKeybindArray [i].DownKey);
			if(Input.GetKey(PlayerKeybindArray [i].LeftKey))
				allHeldKeys.Add(PlayerKeybindArray [i].LeftKey);
			if(Input.GetKey(PlayerKeybindArray [i].RightKey))
				allHeldKeys.Add(PlayerKeybindArray [i].RightKey);
			if(Input.GetKey(PlayerKeybindArray [i].LTurnKey))
				allHeldKeys.Add(PlayerKeybindArray [i].LTurnKey);
			if(Input.GetKey(PlayerKeybindArray [i].RTurnKey))
				allHeldKeys.Add(PlayerKeybindArray [i].RTurnKey);

			if(allHeldKeys.Count > 0)
			{
				if(Key_Pressed != null)
					Key_Pressed(i, allHeldKeys);
			}

			//Check all keys being released.
			List<KeyCode> allReleasedKeys = new List<KeyCode>();
			
			if(!Input.GetKey(PlayerKeybindArray [i].UpKey))
				allReleasedKeys.Add(PlayerKeybindArray [i].UpKey);
			if(!Input.GetKey(PlayerKeybindArray [i].DownKey))
				allReleasedKeys.Add(PlayerKeybindArray [i].DownKey);
			if(!Input.GetKey(PlayerKeybindArray [i].LeftKey))
				allReleasedKeys.Add(PlayerKeybindArray [i].LeftKey);
			if(!Input.GetKey(PlayerKeybindArray [i].RightKey))
				allReleasedKeys.Add(PlayerKeybindArray [i].RightKey);
			if(!Input.GetKey(PlayerKeybindArray [i].LTurnKey))
				allReleasedKeys.Add(PlayerKeybindArray [i].LTurnKey);
			if(!Input.GetKey(PlayerKeybindArray [i].RTurnKey))
				allReleasedKeys.Add(PlayerKeybindArray [i].RTurnKey);
			
			if(allReleasedKeys.Count > 0)
			{
				if(Key_Released != null)
					Key_Released(i, allReleasedKeys);
			}

		}
	}
}
