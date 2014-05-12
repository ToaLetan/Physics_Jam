using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public int PlayerNumber = 0;

	InputManager inputManager = InputManager.Instance;

	// Use this for initialization
	void Start () 
	{
		inputManager.Key_Pressed += PlayerInput;
		inputManager.Key_Released += ApplyDeceleration;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	private void PlayerInput(int playerNum, KeyCode key)
	{
		Vector3 newPosition = gameObject.transform.position;

		if (playerNum == PlayerNumber) 
		{
			if (key == inputManager.PlayerKeybindArray [playerNum].UpKey)
					newPosition.y += 0.5f * Time.deltaTime;
			if (key == inputManager.PlayerKeybindArray [playerNum].DownKey)
					newPosition.y -= 0.5f * Time.deltaTime;
			if (key == inputManager.PlayerKeybindArray [playerNum].LeftKey)
					newPosition.x -= 0.5f * Time.deltaTime;
			if (key == inputManager.PlayerKeybindArray [playerNum].RightKey)
					newPosition.x += 0.5f * Time.deltaTime;
		}

		gameObject.transform.position = newPosition;
	}

	private void ApplyDeceleration(int PlayerNum, KeyCode key)
	{

	}
}
