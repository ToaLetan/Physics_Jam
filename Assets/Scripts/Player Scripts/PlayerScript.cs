using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
    private const float MAXVELOCITY = 0.75f;
    private const float SELECTIONTIME = 0.5f;
    private const float RESPAWNTIME = 0.8f;
    private const float THROWVELOCITY = 250.0f;
    private const float BEAMOFFSET = -0.03f;
    private const float BEAMALPHA = 0.7f;
    private const float THUMBSTICK_DEADZONE = 0.1f;

    public delegate void PlayerEvent(int playerNum);
    public event PlayerEvent Player_Death;
    public event PlayerEvent Player_Lose;

    //Powerups
    public enum PlayerAction { Throw_Basic, Throw_Spread, Throw_Boomerang, Throw_Enlarge, Throw_Homing  }
    private PlayerAction currentAction = PlayerAction.Throw_Basic;

    //Active
    public Active.ActiveType currentActiveType = Active.ActiveType.Reflect; //SET TO PUBLIC FOR TESTING
    private Active currentActive = null; //Instance of active that's in use.

	public int PlayerNumber = 0;

    InputManager inputManager = InputManager.Instance;
    private GameManager gameManager = null;

    private GameObject selectorBeam = null;
    private GameObject fallSparkAnim = null;
    
    private Timer selectionTimer = new Timer(SELECTIONTIME);
    private Timer respawnTimer = new Timer(RESPAWNTIME);

    private Color playerColour = Color.white;

    private Vector3 startPosition = Vector3.zero;

    private string inputSource = "";
    private int inputSourceIndex = -1;

	private float currentVelocityX = 0;
	private float currentVelocityY = 0;

	private float turnSpeed = 250.0f;

	private float acceleration = 1.5f;
	private float deceleration = 4.0f;

	private int currentDirectionX = 0;
	private int currentDirectionY = 0;

    public int numLives = 5;

    private float width;
    private float height;

    private bool canPerformAction = true;
    private bool canMove = true; //Prevents instantly moving upon respawn.

    public GameObject PlayerBeam
    {
        get { return selectorBeam; }
    }

    public Color PlayerColour
    {
        get { return playerColour; }
    }

    public Vector3 StartPosition
    {
        get { return startPosition; }
    }

    public int NumLives
    {
        get { return numLives; }
        set { numLives = value; }
    }

    public bool CanMove
    {
        get { return canMove; }
        set { canMove = value; }
    }

	// Use this for initialization
	void Start () 
	{
        inputSource = GameInfoManager.Instance.PlayerInputSources[PlayerNumber];
        inputSourceIndex = int.Parse(inputSource.Substring(inputSource.IndexOf(" ") ) ); //Grabs the index number after the input source

        Debug.Log("PLAYER " + PlayerNumber + " TIED TO INPUT SOURCE: " + inputSource);

        inputManager.Key_Held += PlayerInput;
        inputManager.Key_Released += ApplyDeceleration;
        inputManager.Key_Pressed += MenuInput;
        inputManager.Button_Pressed += MenuInput;

        selectionTimer.OnTimerComplete += SetAction;
        respawnTimer.OnTimerComplete += EnableMove;

        //SetPlayerColour(); //Old, used for randomized colour initially.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        width = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max.x;
        height = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max.y;

        startPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
        selectionTimer.Update();
        respawnTimer.Update();

        UpdateActive();

        if (inputSource.Contains("Controller") == true)
        {
            PlayerInput(PlayerNumber, null);
            ApplyDeceleration(PlayerNumber, null);
            RotateBeamController();
        }
	}

    public void SetPlayerColour(Color newColour)
    {
        if (gameObject.transform.FindChild("PlayerGlowLayer") != null)
        {
            SpriteRenderer glowLayerRenderer = gameObject.transform.FindChild("PlayerGlowLayer").GetComponent<SpriteRenderer> ();
            playerColour = newColour;
            glowLayerRenderer.material.color = playerColour;
        }

        AttachBeam ();
    }

	public void PlayerInput(int playerNum, List<string> keysHeld)
	{
        if (gameManager.IsGamePaused == false)
        {
            Vector3 newPosition = gameObject.transform.position;

            if (canMove == true)
            {
                if(inputSource.Contains("Keybinds") == true)
                {
                    //================================================ MOVEMENT ================================================
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString()) ||
                        (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString())))
                    {
                        if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString()))
                        {

                            if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()))
                            {
                                currentDirectionY = 1;
                            }
                            else
                                currentDirectionY = -1;


                        }

                        if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                        {
                            if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()))
                                currentDirectionX = -1;
                            else
                                currentDirectionX = 1;
                        }
                        PlayerMove(currentDirectionX, currentDirectionY);
                    }
                        //=================================================================================================================

                        //================================================ BEAM ROTATION ================================================
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LTurnKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RTurnKey.ToString()))
                        {
                            if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LTurnKey.ToString()))
                                RotateBeamKeyboard(1);
                            else
                                RotateBeamKeyboard(-1);
                        }

                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].GraborThrowKey.ToString()))
                        {
                            if (canPerformAction == true)
                            {
                                if (selectorBeam.GetComponent<BeamScript>().IsHoldingObject == false)
                                    GrabObject();
                                else
                                    ThrowObject();
                            }

                            //REALLY GHETTO ACTIVE TEST
                            UseActive();
                        }
                        //=================================================================================================================
                }

                if (inputSource.Contains("Controller") == true)
                {
                    //================================================ MOVEMENT ================================================
                    if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) >= THUMBSTICK_DEADZONE ||
                        inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) <= -THUMBSTICK_DEADZONE )
                    {

                        if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) > 0)
                            currentDirectionY = 1;
                        if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) < 0)
                            currentDirectionY = -1;
                    }

                    if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) >= THUMBSTICK_DEADZONE ||
                        inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) <= -THUMBSTICK_DEADZONE)
                    {
                        if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) > THUMBSTICK_DEADZONE)
                            currentDirectionX = 1;
                        if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) < THUMBSTICK_DEADZONE)
                            currentDirectionX = -1;
                    }
                    //=================================================================================================================

                    if (inputManager.ControllerArray[inputSourceIndex].GetButtonDown(inputManager.ControllerArray[inputSourceIndex].rightBumper) )
                    {
                        if (canPerformAction == true)
                        {
                            if (selectorBeam.GetComponent<BeamScript>().IsHoldingObject == false)
                                GrabObject();
                            else
                                ThrowObject();
                        }
                    }

                    if (inputManager.ControllerArray[inputSourceIndex].GetButtonDown(inputManager.ControllerArray[inputSourceIndex].startButton.ToString())) //Bring up the pause menu
                    {
                        if (gameManager.IsGamePaused == false)
                            gameManager.ShowPauseMenu(inputSource, inputSourceIndex);
                    }

                    //Only call PlayerMove() if the thumbstick isn't completely idle.
                    if  ( ((inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) >= THUMBSTICK_DEADZONE ||
                            inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) <= -THUMBSTICK_DEADZONE)) ||
                        (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) >= THUMBSTICK_DEADZONE ||
                            inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) <= -THUMBSTICK_DEADZONE) )

                    PlayerMove(currentDirectionX, currentDirectionY);
                }
                
            } 
        }
	}

	public void ApplyDeceleration(int playerNum, List<string> keysReleased)
	{
        if (gameManager.IsGamePaused == false)
        {
            Vector3 newPosition = gameObject.transform.position;
		
            if (canMove == true)
            {
                if (inputSource.Contains("Keybinds") == true)
                {
                    if (keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) && keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString()))
                    {
                        if (currentVelocityY > 0)
                        {
                            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.up * currentDirectionY, height * 1.5f) == true)
                                currentVelocityY = 0;
                            else
                                currentVelocityY -= deceleration * Time.deltaTime;
                        }
                    }

                    if (keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) && keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                    {
                        if (currentVelocityX > 0)
                        {
                            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.right * currentDirectionX, width * 1.5f) == true)
                                currentVelocityX = 0;
                            else
                                currentVelocityX -= deceleration * Time.deltaTime;
                        }
                    }
                }

                if (inputSource.Contains("Controller") == true)
                {
                    if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) <= THUMBSTICK_DEADZONE &&
                        inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickVertical) >= -THUMBSTICK_DEADZONE)
                    {
                        if (currentVelocityY > 0)
                        {
                            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.up * currentDirectionY, height * 1.5f) == true)
                                currentVelocityY = 0;
                            else
                                currentVelocityY -= deceleration * Time.deltaTime;
                        }
                    }

                    if (inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) <= THUMBSTICK_DEADZONE &&
                        inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].leftThumbstickHorizontal) >= -THUMBSTICK_DEADZONE)
                    {
                        if (currentVelocityX > 0)
                        {
                            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.right * currentDirectionX, width * 1.5f) == true)
                                currentVelocityX = 0;
                            else
                                currentVelocityX -= deceleration * Time.deltaTime;
                        }
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
	}

    private void MenuInput(int playerNum, List<string> keysPressed)
    {
        if (keysPressed.Contains(InputManager.Instance.PlayerKeybindArray[inputSourceIndex].SelectKey.ToString()) )
        {
            if (gameManager.IsGamePaused == false)
                gameManager.ShowPauseMenu(inputSource, inputSourceIndex);
        }
    }

    private void PlayerMove(int directionX, int directionY)
    {
        Vector3 newPosition = gameObject.transform.position;

        if (currentVelocityX < MAXVELOCITY)  //As long as the player isn't at top speed, increase velocity.
        {
            //Check if the player is going to collide with an object.
            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.right * currentDirectionX, width * 1.5f) == true)
                currentVelocityX = 0;
            else
                currentVelocityX += acceleration * Time.deltaTime;
        }
        if (currentVelocityY < MAXVELOCITY) //Now do the same for the Y-axis
        {

            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.up * currentDirectionY, height * 1.5f) == true)
                currentVelocityY = 0;
            else
                currentVelocityY += acceleration * Time.deltaTime;
        }

        //Prevent the player from being dragged with an object long after escaping it.
        if (Mathf.Sqrt(Mathf.Pow(gameObject.GetComponent<Rigidbody2D>().velocity.x, 2) + Mathf.Pow(gameObject.GetComponent<Rigidbody2D>().velocity.y, 2)) < 0.4f)
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

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
                Death();
            if (collisionObj.gameObject.tag == "Pickup")
                GainPickup(collisionObj.gameObject.GetComponent<PickupScript>());
    }

	private void AttachBeam()
	{
		selectorBeam = GameObject.Instantiate (Resources.Load ("Prefabs/PlayerObjects/LineSegment")) as GameObject;

        if (gameObject.transform.FindChild("PlayerArm") != null)
        {
            //Get the position of the player arm and its length, attach the beam to the end of the arm where the hand is.
            Vector3 playerArmPos = gameObject.transform.FindChild("PlayerArm").position;
            float playerArmLength = gameObject.transform.FindChild("PlayerArm").GetComponent<SpriteRenderer>().sprite.bounds.max.x;

            playerArmPos.x += playerArmLength + BEAMOFFSET;

            selectorBeam.transform.position = playerArmPos;
            selectorBeam.transform.parent = gameObject.transform.FindChild("PlayerArm");
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

	private void RotateBeamKeyboard(int direction)
	{
        float newAngle = selectorBeam.transform.rotation.eulerAngles.z + turnSpeed * direction * Time.deltaTime;
		Quaternion newRotation = Quaternion.AngleAxis (newAngle, Vector3.forward);

        if (gameObject.transform.FindChild("PlayerArm") != null)
            gameObject.transform.FindChild("PlayerArm").rotation = newRotation;

		selectorBeam.transform.rotation = newRotation;
	}

    private void RotateBeamController()
    {
        Vector2 rightThumbstickOrientation = new Vector2(inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].rightThumbstickHorizontal),
            inputManager.ControllerArray[inputSourceIndex].GetThumbstickAxis(inputManager.ControllerArray[inputSourceIndex].rightThumbstickVertical));

        float newAngle = Mathf.Atan2(rightThumbstickOrientation.y, rightThumbstickOrientation.x) * Mathf.Rad2Deg * -1;

        Quaternion newRotation = Quaternion.AngleAxis(newAngle, Vector3.forward);

        if (gameObject.transform.FindChild("PlayerArm") != null)
            gameObject.transform.FindChild("PlayerArm").rotation = newRotation;

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
            playerBeam.CurrentObjectSelected.GetComponent<Rigidbody2D>().angularVelocity = 0;
			playerBeam.CurrentObjectSelected.GetComponent<BoxCollider2D>().enabled = false;

			//playerBeam.CurrentObjectSelected.transform.position = Vector3.zero;
			playerBeam.CurrentObjectSelected.transform.parent = selectorBeam.transform;

			//MAKE SURE TO SET THE OBJECT'S SCALE, AS IT INHERITS THE PARENT OBJECT'S SCALE FOR SOME STUPID REASON.
            playerBeam.CurrentObjectSelected.transform.rotation = playerBeam.transform.rotation;
			playerBeam.CurrentObjectSelected.transform.localScale = new Vector2(1/playerBeam.transform.localScale.x, 1/playerBeam.transform.localScale.y);

            selectionTimer.ResetTimer(true);
            canPerformAction = false;
		}
	}

	private void ThrowObject()
	{
        BeamScript playerBeam = selectorBeam.GetComponent<BeamScript> ();

        if (playerBeam.CurrentObjectHeld != null)
        {
            switch(currentAction)
            {
                case PlayerAction.Throw_Basic:
                    ActionTypes.Throw_Basic(playerBeam);
                    break;
                case PlayerAction.Throw_Spread:
                    ActionTypes.Throw_Spread(playerBeam);
                    break;
                case PlayerAction.Throw_Boomerang:
                    ActionTypes.Throw_Boomerang(playerBeam);
                    break;
                case PlayerAction.Throw_Enlarge:
                    ActionTypes.Throw_Enlarge(playerBeam);
                    break;
                case PlayerAction.Throw_Homing:
                    ActionTypes.Throw_Homing(playerBeam);
                    break;
                default:
                    ActionTypes.Throw_Basic(playerBeam);
                    break;
            }
            //If the player gained an action through a pickup, reset them back to the basic action.
            if(currentAction != PlayerAction.Throw_Basic)
                currentAction = PlayerAction.Throw_Basic;

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
        //Play death animation.
        gameObject.GetComponent<Animator>().Play("Player_Fall");

        //Hide the arm and selector beam.
        if (selectorBeam.GetComponent<SpriteRenderer>() != null)
            selectorBeam.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null && gameObject.transform.GetChild(i).name != "PlayerGlowLayer")
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

            if (gameObject.transform.GetChild(i).name == "PlayerGlowLayer") //Change the Glow Layer sprite to the falling one.
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Player/Player_GlowLayer_Fall");
        }

        if (gameObject.GetComponent<AnimationObject>() != null)
            gameObject.GetComponent<AnimationObject>().Animation_Complete += OnDeathAnimComplete;

        canMove = false;

    }

    private void Respawn(bool useSpawnTimer = true)
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null && gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 0))
        {
            //Show the player
            if (gameObject.GetComponent<SpriteRenderer>() != null)
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;

            //Show the beam
            if (selectorBeam.GetComponent<SpriteRenderer>() != null)
                selectorBeam.GetComponent<SpriteRenderer>().color = GameInfoManager.Instance.PlayerColours[PlayerNumber];

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    if (gameObject.transform.GetChild(i).name == "PlayerGlowLayer") //Set the Glow Layer to match the player colour and change it to the default version
                    {
                        gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Player/PlayerGlowLayerV1");
                        gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = GameInfoManager.Instance.PlayerColours[PlayerNumber];
                    }
                    else
                        gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }

        currentVelocityX = 0;
        currentVelocityY = 0;

        gameObject.transform.position = startPosition;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AnimatedPrefabs/SpawnAnimation"), gameObject.transform.position, gameObject.transform.rotation);

        canMove = false;

        if(useSpawnTimer == true)
            respawnTimer.StartTimer();

        //Disable physics on the player as an invulnerability period.
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void EnableMove()
    {
        canMove = true;
        respawnTimer.ResetTimer();

        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void GainPickup(PickupScript pickupInfo)
    {
        switch (pickupInfo.CurrentPickupType)
        {
            case PickupScript.PickupType.Spread:
                currentAction = PlayerAction.Throw_Spread;
                break;
            case PickupScript.PickupType.Boomerang:
                currentAction = PlayerAction.Throw_Boomerang;
                break;
            case PickupScript.PickupType.Enlarge:
                currentAction = PlayerAction.Throw_Enlarge;
                break;
            case PickupScript.PickupType.Homing:
                currentAction = PlayerAction.Throw_Homing;
                break;
        }
        //Spawn the pickup text
        GameObject pickupText = GameObject.Instantiate(Resources.Load("Prefabs/GUI/PickupAlert"), pickupInfo.gameObject.transform.position, 
                                                       pickupInfo.gameObject.transform.rotation) as GameObject;
        pickupText.GetComponent<PickupAlertScript>().PickupText(pickupInfo.CurrentPickupType.ToString() );

        Destroy(pickupInfo.gameObject);
    }

    private void OnDeathAnimComplete()
    {
        //Stop death animation
        if (gameObject.GetComponent<Animator>() != null)
            gameObject.GetComponent<Animator>().enabled = false;

        //Stop movement.
        currentVelocityX = 0;
        currentVelocityY = 0;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        //Spawn a Fall Spark animation object and set the colour to match the player
        fallSparkAnim = GameObject.Instantiate(Resources.Load("Prefabs/AnimatedPrefabs/FallSparkAnim"), gameObject.transform.position, 
                                                            gameObject.transform.rotation) as GameObject;
        if (fallSparkAnim.GetComponent<SpriteRenderer>() != null)
            fallSparkAnim.GetComponent<SpriteRenderer>().color = GameInfoManager.Instance.PlayerColours[PlayerNumber];

        if (fallSparkAnim.GetComponent<AnimationObject>() != null)
            fallSparkAnim.GetComponent<AnimationObject>().Animation_Complete += OnFallSparkAnimComplete;

        //Hide the player
        if (gameObject.GetComponent<SpriteRenderer>() != null)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        }
    }

    private void OnFallSparkAnimComplete()
    {
        //Unsub from event
        fallSparkAnim.GetComponent<AnimationObject>().Animation_Complete -= OnFallSparkAnimComplete;

        //Change back to idle animation
        if (gameObject.GetComponent<Animator>() != null)
        {
            //Re-enable the player's Animator because it gets disabled by AnimationObject
            gameObject.GetComponent<Animator>().enabled = true;

            gameObject.GetComponent<Animator>().Play("Player_Idle");
        }

        //Reset the player's position after the death animation has finished.
        //-1 life
        if (Player_Death != null)
            Player_Death(PlayerNumber);

        numLives -= 1;

        if (numLives <= 0)
        {
            if (Player_Lose != null)
                Player_Lose(PlayerNumber);

            //Respawn(false);
            currentVelocityX = 0;
            currentVelocityY = 0;
            canMove = false;
        }
        else
            Respawn();
    }

    private void UpdateActive()
    {
        if (currentActive != null)
        {
            currentActive.Update();
        }
    }

    private void UseActive() //Uses the player's Active
    {
        if (currentActive == null)
        {
            switch (currentActiveType)
            {
                case Active.ActiveType.GravityField:
                    currentActive = ActivesTypes.GravityField(this);
                    break;
                case Active.ActiveType.Reflect:
                    currentActive = ActivesTypes.Reflect(this);
                    break;
                case Active.ActiveType.Slipstream:
                    currentActive = ActivesTypes.Slipstream(this);
                    break;
                case Active.ActiveType.Soak:
                    currentActive = ActivesTypes.Soak(this);
                    break;
                case Active.ActiveType.Overclock:
                    currentActive = ActivesTypes.Overclock(this);
                    break;
                case Active.ActiveType.Slowstream:
                    currentActive = ActivesTypes.Slowstream(this);
                    break;
            }

            if (currentActive != null)
            {
                currentActive.Duration.StartTimer();
                currentActive.Duration.OnTimerComplete += OnActiveDurationEnded;
            }  
        }
    }

    private void OnActiveDurationEnded()
    {
        //Start the Cooldown Timer
        currentActive.Cooldown.StartTimer();
        currentActive.Cooldown.OnTimerComplete += OnActiveCooldownEnded;
        currentActive.Duration.OnTimerComplete -= OnActiveDurationEnded;

        //Show the Cooldown overlay on the HUD

        Debug.Log("ACTIVE OVER");
    }

    private void OnActiveCooldownEnded()
    {
        //Reset the Active
        currentActive.Cooldown.OnTimerComplete -= OnActiveCooldownEnded;

        currentActive = null;

        Debug.Log("COOLDOWN OVER");
    }
}
