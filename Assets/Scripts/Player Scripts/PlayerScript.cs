using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
    private const float MAXVELOCITY = 0.75f;
    private const float SELECTIONTIME = 0.5f;
    private const float THROWVELOCITY = 250.0f;
    private const float BEAMOFFSET = -0.03f;
    private const float BEAMALPHA = 0.7f;

    public delegate void PlayerEvent(int playerNum);
    public event PlayerEvent Player_Death;

	public int PlayerNumber = 0;

    InputManager inputManager = InputManager.Instance;

    private GameObject selectorBeam = null;
    
    private Timer selectionTimer = new Timer(SELECTIONTIME);

    private Color playerColour = Color.white;

    private Vector3 startPosition = Vector3.zero;

	private float currentVelocityX = 0;
	private float currentVelocityY = 0;

	private float turnSpeed = 100.0f;

	private float acceleration = 1.5f;
	private float deceleration = 4.0f;

	private int currentDirectionX = 0;
	private int currentDirectionY = 0;

    private float width;
    private float height;

    private bool canPerformAction = true;

    public Color PlayerColour
    {
        get { return playerColour; }
    }

    public Vector3 StartPosition
    {
        get { return startPosition; }
    }

	// Use this for initialization
	void Start () 
	{
		inputManager.Key_Pressed += PlayerInput;
		inputManager.Key_Released += ApplyDeceleration;

        selectionTimer.OnTimerComplete += SetAction;

        SetPlayerColour();
		AttachBeam ();

        width = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max.x;
        height = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max.y;

        startPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
        selectionTimer.Update();
	}

    private void SetPlayerColour()
    {
        if (gameObject.transform.FindChild("PlayerGlowLayerV1") != null)
        {
            SpriteRenderer glowLayerRenderer = gameObject.transform.FindChild("PlayerGlowLayerV1").GetComponent<SpriteRenderer> ();
            playerColour = new Color (Random.value, Random.value, Random.value, 1.0f);
            glowLayerRenderer.material.color = playerColour;
        }
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
				
				if (currentVelocityY < MAXVELOCITY) //As long as the player isn't at top speed, increase velocity.
                {
                    //Check if the player is going to collide with an object.
                    if(SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.up * currentDirectionY, height * 1.5f) == true)
                        currentVelocityY = 0;
                    else
					    currentVelocityY += acceleration * Time.deltaTime;
                }
			}

			if(keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].LeftKey) || keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].RightKey) )
			{
				if(keysHeld.Contains(inputManager.PlayerKeybindArray [playerNum].LeftKey) )
					currentDirectionX = -1;
				else
					currentDirectionX = 1;

				if (currentVelocityX < MAXVELOCITY)
                {
                    if(SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.right * currentDirectionX, width * 1.5f) == true)
                        currentVelocityX = 0;
                    else
					    currentVelocityX += acceleration * Time.deltaTime;
                }
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
                if(canPerformAction == true)
                {
                    if(selectorBeam.GetComponent<BeamScript> ().IsHoldingObject == false )
                        GrabObject();
                    else
                        ThrowObject();
                }
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
                {
                    if(SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.up * currentDirectionY, height * 1.5f) == true)
                        currentVelocityY = 0;
                    else
					    currentVelocityY -= deceleration * Time.deltaTime;
                }
			}

			if(keysReleased.Contains(inputManager.PlayerKeybindArray [playerNum].LeftKey) && keysReleased.Contains(inputManager.PlayerKeybindArray [playerNum].RightKey) )
			{
				if (currentVelocityX > 0)
                {
                    if(SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.right * currentDirectionX, width * 1.5f) == true)
                        currentVelocityX = 0;
                    else
					    currentVelocityX -= deceleration * Time.deltaTime;
                }
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

	private void OnCollisionEnter2D(Collision2D collisionObj)
	{
		//Replace with a list of collision objects that the player is unable to pass.
		switch (collisionObj.gameObject.name) 
		{
		case "Player":
		case "Tile_Rock":
		case "Tile_RockCorner":                       
            if(currentVelocityX > 0)
			    currentVelocityX = 0;
            if(currentVelocityY > 0)
			    currentVelocityY = 0;
			break;
		}
	}

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "KillBox")
        {
            Death();
        }
    }

	private void AttachBeam()
	{
		selectorBeam = GameObject.Instantiate (Resources.Load ("Prefabs/PlayerObjects/LineSegment")) as GameObject;

        if (gameObject.transform.FindChild("PlayerArmV1") != null)
        {
            //Get the position of the player arm and its length, attach the beam to the end of the arm where the hand is.
            Vector3 playerArmPos = gameObject.transform.FindChild("PlayerArmV1").position;
            float playerArmLength = gameObject.transform.FindChild("PlayerArmV1").GetComponent<SpriteRenderer>().sprite.bounds.max.x;

            playerArmPos.x += playerArmLength + BEAMOFFSET;

            selectorBeam.transform.position = playerArmPos;
            selectorBeam.transform.parent = gameObject.transform.FindChild("PlayerArmV1");
        } 
        else
        {
            selectorBeam.transform.position = gameObject.transform.position;
            selectorBeam.transform.parent = gameObject.transform;
        }

		selectorBeam.transform.localScale = new Vector2 (20, 1);
		

		//Set beam colour
		SpriteRenderer beamRenderer = selectorBeam.transform.GetComponent<SpriteRenderer> ();
        Color beamColour = playerColour;
        beamColour.a = BEAMALPHA;
        beamRenderer.material.color = beamColour;
	}

	private void RotateBeam(int direction)
	{
		float newAngle = selectorBeam.transform.rotation.eulerAngles.z + turnSpeed * direction * Time.deltaTime;
		Quaternion newRotation = Quaternion.AngleAxis (newAngle, Vector3.forward);

        if (gameObject.transform.FindChild("PlayerArmV1") != null)
            gameObject.transform.FindChild("PlayerArmV1").rotation = newRotation;

		selectorBeam.transform.rotation = newRotation;
	}

	private void GrabObject()
	{
		BeamScript playerBeam = selectorBeam.GetComponent<BeamScript> ();
		if (playerBeam.CurrentObjectSelected != null) 
		{
            playerBeam.GrabObject();

			playerBeam.CurrentObjectSelected.GetComponent<Rigidbody2D>().gravityScale = 0;
			playerBeam.CurrentObjectSelected.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			playerBeam.CurrentObjectSelected.GetComponent<BoxCollider2D>().enabled = false;

			//playerBeam.CurrentObjectSelected.transform.position = Vector3.zero;
			playerBeam.CurrentObjectSelected.transform.parent = selectorBeam.transform;

			//MAKE SURE TO SET THE OBJECT'S SCALE, AS IT INHERITS THE PARENT OBJECT'S SCALE FOR SOME STUPID REASON.
			playerBeam.CurrentObjectSelected.transform.localScale = new Vector2(1/playerBeam.transform.localScale.x, 1/playerBeam.transform.localScale.y);
			playerBeam.CurrentObjectSelected.transform.rotation = playerBeam.transform.rotation;

            selectionTimer.ResetTimer(true);
            canPerformAction = false;
		}
	}

	private void ThrowObject()
	{
        BeamScript playerBeam = selectorBeam.GetComponent<BeamScript> ();

        if (playerBeam.CurrentObjectHeld != null)
        {
            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().AddForce(playerBeam.CurrentObjectHeld.transform.right * THROWVELOCITY);

            playerBeam.CurrentObjectHeld.GetComponent<BoxCollider2D>().enabled = true;

            playerBeam.CurrentObjectHeld.transform.parent = null;
            playerBeam.ReleaseObject();

            selectionTimer.ResetTimer(true);
            canPerformAction = false;
        }
	}

    private void SetAction()
    {
        canPerformAction = true;
    }

    private void Death()
    {
        if(Player_Death != null)
            Player_Death(PlayerNumber);
        
        //Change the player's pose, hide the arm and selector beam.
        AnimationPlayer.PlayAnimation(gameObject, "Player_Fall");
        AnimationPlayer.ChangeSprite(gameObject.transform.FindChild("PlayerGlowLayerV1").gameObject, "Sprites/Player/Player_GlowLayer_Fall");
        selectorBeam.transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        gameObject.transform.FindChild("PlayerArmV1").transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        //Reset the player's position after the death animation has finished.
        gameObject.transform.position = startPosition;

        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SpawnAnimation"), gameObject.transform.position, gameObject.transform.rotation);
    }
}
