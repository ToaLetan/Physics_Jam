using UnityEngine;
using System.Collections;

public class InputManager
{
	private static InputManager instance = null;

	public XboxController[] ControllerArray = new XboxController[4];

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
		ControllerArray [0] = new XboxController (0);
	}
	
	// Update is called once per frame
	public void Update () 
	{
		if(ControllerArray[0].GetButtonDown(ControllerArray[0].buttonA) )
		{
			Debug.Log("A PRESSED");
		}
	}
}
