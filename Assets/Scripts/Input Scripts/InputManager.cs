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

	//Physics manipulation and ability keys
	public KeyCode GraborThrowKey;
    public KeyCode AbilityKey;

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
    public delegate void AxisEvent(int playerNum, Vector2 axisValues);
    public delegate void TriggerEvent(int playerNum, float triggerValue);

    public event ButtonHeldEvent Button_Held;
    public event ButtonPressedEvent Button_Pressed;
    public event ButtonReleasedEvent Button_Released;

    public event AxisEvent Left_Thumbstick_Axis;
    public event AxisEvent Right_Thumbstick_Axis;
    public event AxisEvent DPad_Axis;

    public event TriggerEvent Left_Trigger_Axis;
    public event TriggerEvent Right_Trigger_Axis;

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
        PlayerKeybindArray[0].AbilityKey = KeyCode.R;

		PlayerKeybindArray [1].UpKey = KeyCode.I;
		PlayerKeybindArray [1].DownKey = KeyCode.K;
		PlayerKeybindArray [1].LeftKey = KeyCode.J;
		PlayerKeybindArray [1].RightKey = KeyCode.L;
		PlayerKeybindArray [1].LTurnKey = KeyCode.U;
		PlayerKeybindArray [1].RTurnKey = KeyCode.O;
		PlayerKeybindArray [1].GraborThrowKey = KeyCode.Semicolon;
        PlayerKeybindArray[1].AbilityKey = KeyCode.P;

		PlayerKeybindArray [0].SelectKey = KeyCode.Return; //Only exists once to prevent game from "double-pausing"
		PlayerKeybindArray [0].ExitKey = KeyCode.Escape;

		for (int i = 0; i < ControllerArray.Length; i++)
		{
			ControllerArray[i] = new XboxController(i);
		}
	}
	
	// Update is called once per frame
	public void Update () 
	{
		UpdateButtonInput();
        UpdateAxisInput();
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
            if (Input.GetKey(PlayerKeybindArray[i].AbilityKey))
                allHeldKeys.Add(PlayerKeybindArray[i].AbilityKey.ToString());

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
            if (Input.GetKeyDown(PlayerKeybindArray[i].AbilityKey))
                allPressedKeys.Add(PlayerKeybindArray[i].AbilityKey.ToString());
			
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
            if (!Input.GetKey(PlayerKeybindArray[i].AbilityKey))
                allReleasedKeys.Add(PlayerKeybindArray[i].AbilityKey.ToString());
			
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
                //Check all buttons being held along with thumbsticks.
                List<string> allHeldButtons = new List<string>();
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonA))
                    allHeldButtons.Add(ControllerArray[j].buttonA);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonB))
                    allHeldButtons.Add(ControllerArray[j].buttonB);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonX))
                    allHeldButtons.Add(ControllerArray[j].buttonX);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].buttonY))
                    allHeldButtons.Add(ControllerArray[j].buttonY);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].startButton))
                    allHeldButtons.Add(ControllerArray[j].startButton);
                if (ControllerArray[j].GetButtonHeld(ControllerArray[j].backButton))
                    allHeldButtons.Add(ControllerArray[j].backButton);

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
                if (ControllerArray[j].GetButtonDown(ControllerArray[j].startButton))
                    allPressedButtons.Add(ControllerArray[j].startButton);
                if (ControllerArray[j].GetButtonDown(ControllerArray[j].backButton))
                    allPressedButtons.Add(ControllerArray[j].backButton);

                if (allPressedButtons.Count > 0)
                {
                    if (Button_Pressed != null)
                        Button_Pressed(j, allPressedButtons);

                    allPressedButtons.Clear();
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
                if (!ControllerArray[j].GetButtonHeld(ControllerArray[j].startButton))
                    allReleasedButtons.Add(ControllerArray[j].startButton);
                if (!ControllerArray[j].GetButtonHeld(ControllerArray[j].backButton))
                    allReleasedButtons.Add(ControllerArray[j].backButton);


                if (allReleasedButtons.Count > 0)
                {
                    if (Button_Released != null)
                        Button_Released(j, allReleasedButtons);

                    allReleasedButtons.Clear();
                }
            }
	}

    public void UpdateAxisInput()
    {
        for (int i = 0; i < ControllerArray.Length; i++)
        {
            if (Left_Thumbstick_Axis != null)
                Left_Thumbstick_Axis(ControllerArray[i].Controller_ID, new Vector2(ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].leftThumbstickHorizontal),
                                                                                    ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].leftThumbstickVertical)));

            if (Right_Thumbstick_Axis != null)
                Right_Thumbstick_Axis(ControllerArray[i].Controller_ID, new Vector2(ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].rightThumbstickHorizontal),
                                                                                    ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].rightThumbstickVertical) ) );

            if (DPad_Axis != null)
                DPad_Axis(ControllerArray[i].Controller_ID, new Vector2(ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].dPadHorizontal),
                                                                                    ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].dPadVertical) ) );

            if (Left_Trigger_Axis != null)
            {
                if (ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].leftRightTriggers) >= 0)
                    Left_Trigger_Axis(ControllerArray[i].Controller_ID, ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].leftRightTriggers) );
            }

            if (Right_Trigger_Axis != null)
            {
                if (ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].leftRightTriggers) <= 0)
                    Right_Trigger_Axis(ControllerArray[i].Controller_ID, ControllerArray[i].GetThumbstickTriggerAxis(ControllerArray[i].leftRightTriggers));
            }
        }
    }

}
