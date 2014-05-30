using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
	public int PlayerNumber = 0;

	InputManager inputManager = InputManager.Instance;

	private const float MAXVELOCITY = 0.75f;

	public float currentVelocityX = 0;
	public float currentVelocityY = 0;

	public float turnSpeed = 50.0f;

	private GameObject selectorBeam = null;

	private float acceleration = 1.5f;
	private float deceleration = 4.0f;

	private int currentDirectionX = 0;
	private int currentDirectionY = 0;

	// Use this for initialization
	void Start () 
	{
		inputManager.Key_Pressed += PlayerInput;
		inputManager.Key_Released += ApplyDeceleration;

		AttachBeam ();
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	private void PlayerInput(int playerNum, List<KeyCode> keysHeld)
	{
		Vector3 newPosition = gameObject.transform.position;

		if (playerNum == PlayerNumber) 
		{
			if(keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].UpKey) || keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].DownKey) )
			{
				//================================================ MOVEMENT ================================================
				//Clean this mess up later, make a MovePlayer() function
				if(keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].UpKey) )
				{
					currentDirectionY = 1;
				}
				else
					currentDirectionY = -1;
				
				if (currentVelocityY < MAXVELOCITY)
					currentVelocityY += acceleration * Time.deltaTime;
			}

			if(keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].LeftKey) || keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].RightKey) )
			{
				if(keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].LeftKey) )
					currentDirectionX = -1;
				else
					currentDirectionX = 1;

				if (currentVelocityX < MAXVELOCITY)
					currentVelocityX += acceleration * Time.deltaTime;
			}
			//=================================================================================================================

			//================================================ BEAM ROTATION ================================================
			if(keysHeld.Contains(inputManager.PlayerKeybindArray[playerNum].LTurnKey) || keysHeld.Contains(inputManager.PlayerKeybindArray[playerNum].RTurnKey) )
			{
				if(keysHeld.Contains(inputManager.PlayerKeybindArray[playerNum].LTurnKey) )
					RotateBeam(1);
				else
					RotateBeam(-1);
			}

			if(keysHeld.Contains(inputManager.PlayerKeybindArray[playerNum].GraborThrowKey) )
			{
				GrabObject();
			}
			//=================================================================================================================
		}

		newPosition.x += currentVelocityX * currentDirectionX * Time.deltaTime;
		newPosition.y += currentVelocityY * currentDirectionY * Time.deltaTime;
		gameObject.transform.position = newPosition;
	}

	private void ApplyDeceleration(int playerNum, List<KeyCode> keysReleased)
	{
		Vector3 newPosition = gameObject.transform.position;
		
		if (playerNum == PlayerNumber) 
		{
			if(keysReleased.Contains(inputManager.PlayerKeybindArray [playerNum].UpKey) && keysReleased.Contains(inputManager.PlayerKeybindArray [playerNum].DownKey) )
			{
				if (currentVelocityY > 0)
					currentVelocityY -= deceleration * Time.deltaTime;
			}

			if(keysReleased.Contains(inputManager.PlayerKeybindArray [playerNum].LeftKey) && keysReleased.Contains(inputManager.PlayerKeybindArray [playerNum].RightKey) )
			{
				if (currentVelocityX > 0)
					currentVelocityX -= deceleration * Time.deltaTime;
			}
		}

		if (currentVelocityX < 0)
			currentVelocityX = 0;
		if (currentVelocityY < 0)
			currentVelocityY = 0;

		newPosition.x += currentVelocityX * currentDirectionX * Time.deltaTime;
		newPosition.y += currentVelocityY * currentDirectionY * Time.deltaTime;
		gameObject.transform.position = newPosition;
	}

	void OnCollisionEnter2D(Collision2D collisionObj)
	{
		//Replace with a list of collision objects that the player is unable to pass.
		switch (collisionObj.gameObject.name) 
		{
		case "Player":
		case "Tile_Rock":
		case "Tile_RockCorner":
			currentVelocityX = 0;
			currentVelocityY = 0;
			break;
		}
	}

	void AttachBeam()
	{
		selectorBeam = GameObject.Instantiate (Resources.Load ("Prefabs/LineSegment")) as GameObject;
		selectorBeam.transform.position = gameObject.transform.position;
		selectorBeam.transform.localScale = new Vector2 (20, 1);
		selectorBeam.transform.parent = gameObject.transform;

		//Set beam colour
		SpriteRenderer beamRenderer = selectorBeam.transform.GetComponent<SpriteRenderer> ();
		beamRenderer.material.color = new Color (Random.value, Random.value, Random.value, 0.7f);
	}

	void RotateBeam(int direction)
	{
		float newAngle = selectorBeam.transform.rotation.eulerAngles.z + turnSpeed * direction * Time.deltaTime;
		Quaternion newRotation = Quaternion.AngleAxis (newAngle, Vector3.forward);

		selectorBeam.transform.rotation = newRotation;
	}

	void GrabObject()
	{
		BeamScript playerBeam = selectorBeam.GetComponent<BeamScript> ();
		if (playerBeam.CurrentObjectSelected != null) 
		{
			playerBeam.CurrentObjectSelected.GetComponent<Rigidbody2D>().gravityScale = 0;
			playerBeam.CurrentObjectSelected.GetComponent<Rigidbody2D>().angularVelocity = 0;
			playerBeam.CurrentObjectSelected.GetComponent<BoxCollider2D>().enabled = false;

			//playerBeam.CurrentObjectSelected.transform.position = Vector3.zero;
			playerBeam.CurrentObjectSelected.transform.parent = selectorBeam.transform;

			//MAKE SURE TO SET THE OBJECT'S SCALE, AS IT INHERITS THE PARENT OBJECT'S SCALE FOR SOME STUPID REASON.
			playerBeam.CurrentObjectSelected.transform.localScale = new Vector2(1/playerBeam.transform.localScale.x, 1/playerBeam.transform.localScale.y);
			playerBeam.CurrentObjectSelected.transform.rotation = playerBeam.transform.rotation;
		}
	}

	void ThrowObject()
	{

	}
}
