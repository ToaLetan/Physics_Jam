using UnityEngine;
using System.Collections;

public struct Keybinds
{
	public KeyCode UpKey;
	public KeyCode DownKey;
	public KeyCode LeftKey;
	public KeyCode RightKey;
	public KeyCode LTurnKey;
	public KeyCode RTurnKey;
}

public class InputManager
{
	public delegate void KeyHeldEvent(int playerNum, KeyCode key);
	public delegate void KeyReleasedEvent(int playerNum, KeyCode key);

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

		PlayerKeybindArray [1].UpKey = KeyCode.I;
		PlayerKeybindArray [1].DownKey = KeyCode.K;
		PlayerKeybindArray [1].LeftKey = KeyCode.J;
		PlayerKeybindArray [1].RightKey = KeyCode.L;
		PlayerKeybindArray [1].LTurnKey = KeyCode.U;
		PlayerKeybindArray [1].RTurnKey = KeyCode.O;
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
			if(Input.GetKey(PlayerKeybindArray [i].UpKey))
			{
				if(Key_Pressed != null)
					Key_Pressed(i, PlayerKeybindArray [i].UpKey);
			}
			if(Input.GetKey(PlayerKeybindArray [i].DownKey))
			{
				if(Key_Pressed != null)
					Key_Pressed(i, PlayerKeybindArray [i].DownKey);
			}
			if(Input.GetKey(PlayerKeybindArray [i].LeftKey))
			{
				if(Key_Pressed != null)
					Key_Pressed(i, PlayerKeybindArray [i].LeftKey);
			}
			if(Input.GetKey(PlayerKeybindArray [i].RightKey))
			{
				if(Key_Pressed != null)
					Key_Pressed(i, PlayerKeybindArray [i].RightKey);
			}
			if(Input.GetKey(PlayerKeybindArray [i].LTurnKey))
			{
				if(Key_Pressed != null)
					Key_Pressed(i, PlayerKeybindArray [i].LTurnKey);
			}
			if(Input.GetKey(PlayerKeybindArray [i].RTurnKey))
			{
				if(Key_Pressed != null)
					Key_Pressed(i, PlayerKeybindArray [i].RTurnKey);
			}

			//Check all keys being released.
			if(!Input.GetKey(PlayerKeybindArray [i].UpKey))
			{
				if(Key_Released != null)
					Key_Released(i, PlayerKeybindArray [i].UpKey);
			}
			if(!Input.GetKey(PlayerKeybindArray [i].DownKey))
			{
				if(Key_Released != null)
					Key_Released(i, PlayerKeybindArray [i].DownKey);
			}
			if(!Input.GetKey(PlayerKeybindArray [i].LeftKey))
			{
				if(Key_Released != null)
					Key_Released(i, PlayerKeybindArray [i].LeftKey);
			}
			if(!Input.GetKey(PlayerKeybindArray [i].RightKey))
			{
				if(Key_Released != null)
					Key_Released(i, PlayerKeybindArray [i].RightKey);
			}
			if(!Input.GetKey(PlayerKeybindArray [i].LTurnKey))
			{
				if(Key_Released != null)
					Key_Released(i, PlayerKeybindArray [i].LTurnKey);
			}
			if(!Input.GetKey(PlayerKeybindArray [i].RTurnKey))
			{
				if(Key_Released != null)
					Key_Released(i, PlayerKeybindArray [i].RTurnKey);
			}

		}
	}
}
