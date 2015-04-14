using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour 
{
    private const float DEFAULT_MAX_VELOCITY = 0.75f;
    private const float DEFAULT_ACCELERATION = 1.5f;
    private const float DEFAULT_DECELERATION = 4.0f;
    private const float SELECTIONTIME = 0.5f;
    private const float RESPAWNTIME = 0.8f;
    private const float THROWVELOCITY = 250.0f;
    private const float BEAMOFFSET = -0.03f;
    private const float BEAMALPHA = 0.7f;
    private const float THUMBSTICK_TRIGGER_DEADZONE = 0.1f;

    public delegate void PlayerEvent(int playerNum);
    public event PlayerEvent Player_Death;
    public event PlayerEvent Player_Lose;
    public event PlayerEvent Player_Cooldown_Start;
    public event PlayerEvent Player_Cooldown_Complete;

    //Powerups
    public enum PlayerAction { Throw_Basic, Throw_Spread, Throw_Boomerang, Throw_Enlarge, Throw_Homing  }
    private PlayerAction currentAction = PlayerAction.Throw_Basic;

    //Active
    public Active.ActiveType currentActiveType = Active.ActiveType.Overclock;
    private Active currentActive = null; //Instance of active that's in use.

    //Animations
    public enum PlayerAnim { Idle, Walk }
    private PlayerAnim currentAnim = PlayerAnim.Idle;

    //Directions
    public enum Direction { Up, Down, Left, Right }
    private Direction currentDirection = Direction.Down;

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

    private float maxVelocity = DEFAULT_MAX_VELOCITY;
	private float acceleration = DEFAULT_ACCELERATION;
    private float deceleration = DEFAULT_DECELERATION;

    private float width;
    private float height;

    private float armInitialPosX = 0.0f;

	private int currentDirectionX = 0;
	private int currentDirectionY = 0;

    public int numLives = 5;

    private bool canPerformAction = true;
    private bool canMove = true; //Prevents instantly moving upon respawn.

    public GameObject PlayerBeam
    {
        get { return selectorBeam; }
    }

    public Active PlayerActive
    {
        get { return currentActive; }
    }

    public Color PlayerColour
    {
        get { return playerColour; }
    }

    public Vector3 StartPosition
    {
        get { return startPosition; }
    }

    public float MaxVelocity
    {
        get { return maxVelocity; }
        set { maxVelocity = value; }
    }

    public float Acceleration
    {
        get { return acceleration; }
        set { acceleration = value; }
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

        if (inputSource.Contains("Keybinds"))
        {
            //Keyboard Input
            inputManager.Key_Held += PlayerInput;
            inputManager.Key_Released += ApplyDeceleration;
            inputManager.Key_Pressed += MenuInput;
        }

        if (inputSource.Contains("Controller"))
        {
            //Controller input
            inputManager.Button_Pressed += MenuInput;
            inputManager.Right_Thumbstick_Axis += RotateBeamController;
            inputManager.Left_Thumbstick_Axis += PlayerControllerInput;
            inputManager.Left_Thumbstick_Axis += ApplyDecelerationController;
            inputManager.Left_Trigger_Axis += LeftTriggerInput;
            inputManager.Right_Trigger_Axis += RightTriggerInput;
        }
        

        selectionTimer.OnTimerComplete += SetAction;
        respawnTimer.OnTimerComplete += EnableMove;

        //SetPlayerColour(); //Old, used for randomized colour initially.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        width = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max.x;
        height = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.max.y;

        startPosition = gameObject.transform.position;
        armInitialPosX = gameObject.transform.FindChild("PlayerArm").transform.localPosition.x;

        //Start with playing the idle animation based on currentDirection and currentAnimation.
        UpdateDirectionalAnim();
	}
	
	// Update is called once per frame
	void Update () 
	{
        selectionTimer.Update();
        respawnTimer.Update();

        UpdateActive();
	}

    public void SetPlayerColour(Color newColour) //Tied to GameManager to set the Colour based on what's established in the GameInfoManager
    {
        if (gameObject.transform.FindChild("PlayerGlowLayer") != null)
        {
            SpriteRenderer glowLayerRenderer = gameObject.transform.FindChild("PlayerGlowLayer").GetComponent<SpriteRenderer> ();
            playerColour = newColour;
            glowLayerRenderer.material.color = playerColour;
        }

        AttachBeam ();
    }

    public void SetPlayerAbility(Active.ActiveType ability) //Tied to GameManager to set the Ability based on what's established in the GameInfoManager
    {
        currentActiveType = ability;
    }

	public void PlayerInput(int playerNum, List<string> keysHeld)
	{
        if (gameManager.IsGamePaused == false)
        {
            Vector3 newPosition = gameObject.transform.position;

            if (canMove == true)
            {
                    //================================================ MOVEMENT ================================================
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString()) ||
                        (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString())))
                    {
                        if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString()))
                        {

                            if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) )
                            {
                                currentDirectionY = 1;
                                currentDirection = Direction.Up;
                            }
                            else if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString()) ) //Can no longer do a simple else because of the animation.
                            {
                                currentDirectionY = -1;
                                currentDirection = Direction.Down;
                            }  
                        }

                        if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) || keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                        {
                            if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) )
                            {
                                currentDirectionX = -1;
                                currentDirection = Direction.Left;
                            }
                            else if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()) ) //Can no longer do a simple else because of the animation.
                            {
                                currentDirectionX = 1;
                                currentDirection = Direction.Right;
                            }   
                        }
                        //Move the player and play an animation based on the current direction
                        PlayerMove(currentDirectionX, currentDirectionY);

                        if (currentAnim != PlayerAnim.Walk)
                        {
                            currentAnim = PlayerAnim.Walk;
                            UpdateDirectionalAnim();
                        }
                    }
                        //=================================================================================================================

                        //================================================ BEAM FUNCTIONALITY AND ABILITY ================================================
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
                        }
                    if (keysHeld.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].AbilityKey.ToString()))
                    {
                        UseActive();
                    }
                        //=================================================================================================================       
            } 
        }
	}

    public void PlayerControllerInput(int playerNum, Vector2 thumbstickVector)
    {
        if (playerNum == inputSourceIndex)
        {
            if (gameManager.IsGamePaused == false)
            {
                Vector3 newPosition = gameObject.transform.position;

                if (canMove == true)
                {
                        //================================================ MOVEMENT ================================================
                    if (thumbstickVector.y >= THUMBSTICK_TRIGGER_DEADZONE || thumbstickVector.y <= -THUMBSTICK_TRIGGER_DEADZONE)
                    {
                        if (thumbstickVector.y > 0)
                        {
                            currentDirectionY = 1;
                            currentDirection = Direction.Up;
                        }

                        if (thumbstickVector.y < 0)
                        {
                            currentDirectionY = -1;
                            currentDirection = Direction.Down;
                        }
                    }
                    else
                        currentDirectionY = 0;

                    if (thumbstickVector.x >= THUMBSTICK_TRIGGER_DEADZONE || thumbstickVector.x <= -THUMBSTICK_TRIGGER_DEADZONE)
                    {
                        if (thumbstickVector.x > THUMBSTICK_TRIGGER_DEADZONE)
                        {
                            currentDirectionX = 1;
                            currentDirection = Direction.Right;
                        }
                            
                        if (thumbstickVector.x < THUMBSTICK_TRIGGER_DEADZONE)
                        {
                            currentDirectionX = -1;
                            currentDirection = Direction.Left;
                        }  
                    }
                    else
                        currentDirectionX = 0;

                    //Only call PlayerMove() if the thumbstick isn't completely idle.
                    if (((thumbstickVector.y >= THUMBSTICK_TRIGGER_DEADZONE || thumbstickVector.y <= -THUMBSTICK_TRIGGER_DEADZONE)) ||
                        (thumbstickVector.x >= THUMBSTICK_TRIGGER_DEADZONE || thumbstickVector.x <= -THUMBSTICK_TRIGGER_DEADZONE))
                    {
                        PlayerMove(currentDirectionX, currentDirectionY);

                        if (currentAnim != PlayerAnim.Walk) //Move the player and play an animation based on the current direction
                        {
                            currentAnim = PlayerAnim.Walk;
                            UpdateDirectionalAnim();
                        }
                    }
                    else //If the thumbstick is idle, play the idle animation.
                    {
                        if (currentAnim != PlayerAnim.Idle) //Move the player and play an animation based on the current direction
                        {
                            currentAnim = PlayerAnim.Idle;
                            UpdateDirectionalAnim();
                        }
                    }
                }
            }
        }
    }

    public void LeftTriggerInput(int playerNum, float triggerValue)
    {
        if (playerNum == inputSourceIndex)
        {
            if (gameManager.IsGamePaused == false)
            {
                if (triggerValue >= THUMBSTICK_TRIGGER_DEADZONE)
                {
                    UseActive();
                }
            }
        }
    }

    public void RightTriggerInput(int playerNum, float triggerValue)
    {
        if (playerNum == inputSourceIndex)
        {
            if (gameManager.IsGamePaused == false)
            {
                if (triggerValue <= -THUMBSTICK_TRIGGER_DEADZONE)
                {
                    if (canPerformAction == true)
                    {
                        if (selectorBeam.GetComponent<BeamScript>().IsHoldingObject == false)
                            GrabObject();
                        else
                            ThrowObject();
                    }
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

                    //If no movement keys are being held, play the idle animation.
                    if (keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) && keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString())
                        && keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) && keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
                    {
                        if (currentAnim != PlayerAnim.Idle) //Play the idle animation if it's not already playing.
                        {
                            currentAnim = PlayerAnim.Idle;
                            UpdateDirectionalAnim();
                        }
                    }
            }


            if (keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].UpKey.ToString()) || keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].DownKey.ToString())
                        || keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].LeftKey.ToString()) || keysReleased.Contains(inputManager.PlayerKeybindArray[inputSourceIndex].RightKey.ToString()))
            {
                if (currentVelocityX < 0)
                    currentVelocityX = 0;
                if (currentVelocityY < 0)
                    currentVelocityY = 0;

                newPosition.x += currentVelocityX * currentDirectionX * Time.deltaTime;
                newPosition.y += currentVelocityY * currentDirectionY * Time.deltaTime;
                gameObject.transform.position = newPosition;
            }
        }
    }

    private void ApplyDecelerationController(int playerNum, Vector2 leftThumbstick)
    {
        if (playerNum == inputSourceIndex)
        {
            if (gameManager.IsGamePaused == false)
            {
                Vector3 newPosition = gameObject.transform.position;

                if (canMove == true && (currentVelocityX > 0 || currentVelocityY > 0) )
                {
                    if (leftThumbstick.y <= THUMBSTICK_TRIGGER_DEADZONE && leftThumbstick.y >= -THUMBSTICK_TRIGGER_DEADZONE)
                    {
                        if (currentVelocityY > 0)
                        {
                            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.up * currentDirectionY, height * 1.5f) == true)
                                currentVelocityY = 0;
                            else
                                currentVelocityY -= deceleration * Time.deltaTime;
                        }
                    }

                    if (leftThumbstick.x <= THUMBSTICK_TRIGGER_DEADZONE && leftThumbstick.x >= -THUMBSTICK_TRIGGER_DEADZONE)
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
                if (currentVelocityX < 0)
                    currentVelocityX = 0;
                if (currentVelocityY < 0)
                    currentVelocityY = 0;

                newPosition.x += currentVelocityX * currentDirectionX * Time.deltaTime;
                newPosition.y += currentVelocityY * currentDirectionY * Time.deltaTime;
                gameObject.transform.position = newPosition;
            }
        }
    }

    private void MenuInput(int playerNum, List<string> keysButtonsPressed)
    {
        if (keysButtonsPressed.Contains(InputManager.Instance.PlayerKeybindArray[0].SelectKey.ToString()) 
            || keysButtonsPressed.Contains(InputManager.Instance.ControllerArray[inputSourceIndex].startButton)) //Index is always 0 for keyboard since there's only one pause and exit key
        {
            if (gameManager.IsGamePaused == false)
                gameManager.ShowPauseMenu(inputSource, inputSourceIndex);
        }
    }

    private void PlayerMove(int directionX, int directionY)
    {
        Vector3 newPosition = gameObject.transform.position;

        if (currentVelocityX < maxVelocity)  //As long as the player isn't at top speed, increase velocity.
        {
            //Check if the player is going to collide with an object.
            if (SpeculativeContactsScript.PerformSpeculativeContacts(gameObject.transform.position, Vector2.right * currentDirectionX, width * 1.5f) == true)
                currentVelocityX = 0;
            else
                currentVelocityX += acceleration * Time.deltaTime;
        }
        if (currentVelocityY < maxVelocity) //Now do the same for the Y-axis
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
        if (gameManager.IsGamePaused == false)
        {
            float newAngle;
            newAngle = selectorBeam.transform.rotation.eulerAngles.z + turnSpeed * direction * Time.deltaTime;

            Quaternion newRotation = Quaternion.AngleAxis(newAngle, Vector3.forward);

            if (gameObject.transform.FindChild("PlayerArm") != null)
                gameObject.transform.FindChild("PlayerArm").rotation = newRotation;

            selectorBeam.transform.rotation = newRotation;
        }
	}

    private void RotateBeamController(int playerNum, Vector2 thumbstickAxis)
    {
        if (gameManager.IsGamePaused == false)
        {
            if (playerNum == inputSourceIndex)
            {
                float newAngle = Mathf.Atan2(thumbstickAxis.y, thumbstickAxis.x) * Mathf.Rad2Deg * -1;

                Quaternion newRotation = Quaternion.AngleAxis(newAngle, Vector3.forward);

                if (gameObject.transform.FindChild("PlayerArm") != null)
                    gameObject.transform.FindChild("PlayerArm").rotation = newRotation;

                selectorBeam.transform.rotation = newRotation;
            }     
        }
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

        /*if (currentDirection == Direction.Left)
            playerBeam.IsFacingLeft = true;
        else
            playerBeam.IsFacingLeft = false;*/

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
        PlayerAnimation("Fall");

        //Hide the arm and selector beam.
        if (selectorBeam.GetComponent<SpriteRenderer>() != null)
            selectorBeam.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null && gameObject.transform.GetChild(i).name != "PlayerGlowLayer")
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
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

            PlayerAnimation("Idle");
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

    private void PlayerAnimation(string animationName, int startFrame = 0)
    {
        //I have no idea why the layer is -1.

        if (animationName.Contains("Side") || animationName.Contains("Left") ) //If it's a side animation, move the player arm back a layer.
        {
            Vector3 armSidePos = gameObject.transform.FindChild("PlayerArm").transform.position;
            armSidePos.x = gameObject.transform.position.x;

            gameObject.transform.FindChild("PlayerArm").transform.position = armSidePos;
            gameObject.transform.FindChild("PlayerArm").GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
            PlayerBeam.transform.GetComponent<SpriteRenderer>().sortingOrder = gameObject.transform.FindChild("PlayerArm").GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
        else //Otherwise, move the arm to the initial position.
        {
            Vector3 armPos = gameObject.transform.FindChild("PlayerArm").transform.localPosition;
            armPos.x = armInitialPosX;

            gameObject.transform.FindChild("PlayerArm").transform.localPosition = armPos;

            if (animationName.Contains("Front"))
            {
                PlayerBeam.transform.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
                gameObject.transform.FindChild("PlayerArm").GetComponent<SpriteRenderer>().sortingOrder = PlayerBeam.transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
                
            }
            else if (animationName.Contains("Back"))
            {
                PlayerBeam.transform.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
                gameObject.transform.FindChild("PlayerArm").GetComponent<SpriteRenderer>().sortingOrder = PlayerBeam.transform.GetComponent<SpriteRenderer>().sortingOrder + 1;
            }
        }

        gameObject.GetComponent<Animator>().Play("Player_" + animationName, -1, startFrame);

        if (gameObject.transform.FindChild("PlayerGlowLayer") != null)
        {
            gameObject.transform.FindChild("PlayerGlowLayer").GetComponent<Animator>().Play("GlowLayer_" + animationName, -1, startFrame);
        }
    }

    private void UpdateDirectionalAnim()
    {
        //Determine the direction and animation name first.
        string animDirectionName = "";
        string animName = "";

        switch(currentDirection)
        {
            case Direction.Up:
                animDirectionName = "Back";
                break;
            case Direction.Down:
                animDirectionName = "Front";
                break;
            case Direction.Left:
                animDirectionName = "Left";
                break;
            case Direction.Right:
                animDirectionName = "Side";
                break;
        }

        if (animDirectionName != "")
        {
            switch(currentAnim)
            {
                case PlayerAnim.Idle:
                    animName = "Idle";
                    break;
                case PlayerAnim.Walk:
                    animName = "Walk";
                    break;
            }
        }

        if (animDirectionName != "" && animName != "") //If the names aren't blank, piece together the string to be used in the PlayerAnimation call.
        {
            PlayerAnimation(animDirectionName + "_" + animName);
        }
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
                case Active.ActiveType.GravField:
                    currentActive = ActivesTypes.GravityField(this);
                    break;
                case Active.ActiveType.Reflect:
                    currentActive = ActivesTypes.Reflect(this);
                    break;
                case Active.ActiveType.SlipGel:
                    currentActive = ActivesTypes.Slipstream(this);
                    break;
                case Active.ActiveType.Soak:
                    currentActive = ActivesTypes.Soak(this);
                    break;
                case Active.ActiveType.Overclock:
                    currentActive = ActivesTypes.Overclock(this);
                    break;
                case Active.ActiveType.SlowGel:
                    currentActive = ActivesTypes.Slowstream(this);
                    break;
            }

            if (currentActive != null)
            {
                currentActive.UseActive();

                currentActive.Duration.OnTimerComplete += OnActiveDurationEnded;

                if (currentActiveType == Active.ActiveType.Reflect) //Disable collision while Reflect is active.
                {
                    gameObject.GetComponent<Rigidbody2D>().drag = 250; //Well we can't disable collision for a while sooooo crank drag the fuck up.
                }

                //Show the Cooldown overlay on the HUD
                if (Player_Cooldown_Start != null)
                    Player_Cooldown_Start(PlayerNumber);
            }  
        }
    }

    private void OnActiveDurationEnded()
    {
        //Start the Cooldown Timer
        currentActive.Cooldown.StartTimer();
        currentActive.Cooldown.OnTimerComplete += OnActiveCooldownEnded;
        currentActive.Duration.OnTimerComplete -= OnActiveDurationEnded;

        //If the Active is Reflect, stop reflecting objects and reset rigidbody modifications.
        if (currentActiveType == Active.ActiveType.Reflect)
        {
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Reflect_Despawn"), gameObject.transform.FindChild("Reflect(Clone)").position, Quaternion.identity);

            GameObject.Destroy(gameObject.transform.FindChild("Reflect(Clone)").gameObject );

            gameObject.GetComponent<Rigidbody2D>().drag = 1;
        }
    }

    private void OnActiveCooldownEnded()
    {
        //Reset the Active
        currentActive.Cooldown.OnTimerComplete -= OnActiveCooldownEnded;

        //Stop the cooldown UI display.
        if (Player_Cooldown_Complete != null)
            Player_Cooldown_Complete(PlayerNumber);

        currentActive = null;
    }
}
