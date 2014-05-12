using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	public int PlayerNumber = 0;

	InputManager inputManager = InputManager.Instance;

	private const float MAXVELOCITY = 1.5f;

	private float currentVelocityX = 0;
	private float currentVelocityY = 0;

	private float acceleration = 2.5f;
	private float deceleration = 2.5f;
	

	// Use this for initialization
	void Start () 
	{
		inputManager.Key_Pressed += PlayerInput;
		//inputManager.Key_Released += ApplyDeceleration;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	private void PlayerInput(int playerNum, KeyCode key)
	{
		Vector3 newPosition = gameObject.transform.position;
		int horizontalDirectionModifier = 0;
		int verticalDirectionModifier = 0;

		if (playerNum == PlayerNumber) 
		{
			if (key == inputManager.PlayerKeybindArray [playerNum].UpKey || key == inputManager.PlayerKeybindArray [playerNum].DownKey)
			{
				if(key == inputManager.PlayerKeybindArray [playerNum].UpKey)
					verticalDirectionModifier = 1;
				else
					verticalDirectionModifier = -1;

				if (currentVelocityY < MAXVELOCITY)
					currentVelocityY += acceleration * Time.deltaTime;
			}
			if (key == inputManager.PlayerKeybindArray [playerNum].LeftKey || key == inputManager.PlayerKeybindArray [playerNum].RightKey)
			{
				if(key == inputManager.PlayerKeybindArray [playerNum].LeftKey)
					horizontalDirectionModifier = -1;
				else
					horizontalDirectionModifier = 1;

				if (currentVelocityX < MAXVELOCITY)
					currentVelocityX += acceleration * Time.deltaTime;
			}
		}

		newPosition.x += currentVelocityX * horizontalDirectionModifier * Time.deltaTime;
		newPosition.y += currentVelocityY * verticalDirectionModifier * Time.deltaTime;
		gameObject.transform.position = newPosition;
	}

	private void ApplyDeceleration(int playerNum, KeyCode key)
	{
		Vector3 newPosition = gameObject.transform.position;
		int horizontalDirectionModifier = 0;
		int verticalDirectionModifier = 0;
		
		if (playerNum == PlayerNumber) 
		{
			if (key == inputManager.PlayerKeybindArray [playerNum].UpKey || key == inputManager.PlayerKeybindArray [playerNum].DownKey)
			{
				if(key == inputManager.PlayerKeybindArray [playerNum].UpKey)
					verticalDirectionModifier = 1;
				else
					verticalDirectionModifier = -1;
				
				if (currentVelocityY > 0)
					currentVelocityY -= deceleration * Time.deltaTime;
			}
			if (key == inputManager.PlayerKeybindArray [playerNum].LeftKey || key == inputManager.PlayerKeybindArray [playerNum].RightKey)
			{
				if(key == inputManager.PlayerKeybindArray [playerNum].LeftKey)
					horizontalDirectionModifier = -1;
				else
					horizontalDirectionModifier = 1;
				
				if (currentVelocityX > 0)
					currentVelocityX -= deceleration * Time.deltaTime;
			}
		}
		
		newPosition.x += currentVelocityX * horizontalDirectionModifier * Time.deltaTime;
		newPosition.y += currentVelocityY * verticalDirectionModifier * Time.deltaTime;
		gameObject.transform.position = newPosition;
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		//Replace with a list of collision objects that the player is unable to pass.
		switch (coll.gameObject.name) 
		{
		case "Tile_Rock":
		case "Tile_RockCorner":
			currentVelocityX = 0;
			currentVelocityY = 0;
			break;
		}
	}
}
