using UnityEngine;
using System.Collections;

public class ActiveProjectileScript : MonoBehaviour 
{
    private const float DESTINATION_DEADZONE = 0.15f;
    private const float PUDDLE_OFFSET = -0.15f;
    private const float BOUNDS_LIFE_TIMER = 2.5f;

    public enum ActiveProjectileType { SpeedUp, SpeedDown, Soak, GravField };

    public ActiveProjectileType ProjectileType = ActiveProjectileType.SpeedUp;

    public delegate void ProjectileEvent(GameObject collisionObject);
    public event ProjectileEvent OnProjectileCollision;

    private GameManager gameManager = null;

    private PlayerScript ownerPlayer = null;

    private GameObject collidedPlayer = null;

    private Timer lifeTimer = null;

    private Color soakColour = new Color(0.22f, 0.38f, 0.62f, 1.0f);

    private Vector3 destination;

    private float velocity = 1.5f;

    private bool hasReachedDestination = false;
    private bool ignoreDestination = false;

    public PlayerScript OwnerPlayer
    {
        set { ownerPlayer = value; }
    }

    public GameObject CollidedPlayer
    {
        get { return collidedPlayer; }
    }

    public Timer LifeTimer
    {
        get { return lifeTimer; }
        set { lifeTimer = value; }
    }

    public Vector3 Destination
    { 
        set { destination = value; } 
    }

    public float Velocity
    {
        set { velocity = value; }
    }

	// Use this for initialization
	void Start () 
    {
        lifeTimer = new Timer(BOUNDS_LIFE_TIMER);
        lifeTimer.OnTimerComplete += Despawn;

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.IsGamePaused == false)
        {
            if (lifeTimer != null)
                lifeTimer.Update();

            if (ignoreDestination == false)
            {
                float distance = Vector3.Distance(gameObject.transform.position, destination);

                if (distance > DESTINATION_DEADZONE)
                {
                    UpdateMovement();
                }
                else
                {
                    if (hasReachedDestination == false)
                    {
                        hasReachedDestination = true;

                        //Instantiate the next animation object, destroy this one.
                        switch (ProjectileType)
                        {
                            case ActiveProjectileType.SpeedUp:
                                GameObject slipSplash = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_Splash"), gameObject.transform.position, Quaternion.identity) as GameObject;
                                slipSplash.GetComponent<GlobPuddleScript>().AbilityInfo = ownerPlayer.PlayerActive;
                                break;
                            case ActiveProjectileType.SpeedDown:
                                GameObject slowSplash = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_SlowSplash"), gameObject.transform.position, Quaternion.identity) as GameObject;
                                slowSplash.GetComponent<GlobPuddleScript>().AbilityInfo = ownerPlayer.PlayerActive;
                                break;
                            case ActiveProjectileType.Soak:
                                break;
                            case ActiveProjectileType.GravField:
                                //GameObject gravField = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/GravField"), gameObject.transform.position, Quaternion.identity) as GameObject;
                                GameObject gravField = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Grav_Impact"), gameObject.transform.position, Quaternion.identity) as GameObject;
                                gravField.GetComponent<GlobPuddleScript>().AbilityInfo = ownerPlayer.PlayerActive;
                                break;
                        }
                        GameObject.Destroy(gameObject);
                    }
                }
            }
            else //If ignoring the destination, just keep moving forward.
                UpdateMovement();
        }
	}

    private void UpdateMovement()
    {
        gameObject.transform.position += gameObject.transform.right * velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "KillBox")
        {
            //Start the timer to despawn out of bounds.
            ignoreDestination = true;

            lifeTimer.StartTimer();
        }

        if (ProjectileType == ActiveProjectileType.Soak)
        {
            if (coll.gameObject.GetComponent<PlayerScript>() != null && coll.gameObject.GetComponent<PlayerScript>().PlayerNumber != ownerPlayer.PlayerNumber)
            {
                if (coll.gameObject.GetComponent<PlayerScript>().CanMove == true) //Prevents cheap shots being taken on falling players.
                {
                    collidedPlayer = coll.gameObject;

                    //Fire the event.
                    if (OnProjectileCollision != null)
                        OnProjectileCollision(collidedPlayer);

                    //Play the splash animation
                    GameObject splashAnim = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Soak_Splash"), gameObject.transform.position, Quaternion.identity) as GameObject;

                    //Play the puddle animation
                    Vector3 puddlePos = coll.gameObject.transform.position;
                    puddlePos.y += PUDDLE_OFFSET;

                    GameObject puddleAnim = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Soak_Puddle"), puddlePos, Quaternion.identity) as GameObject;

                    //Flag the player as being soaked, reduce their speed and instantiate and animated prefab attached to them. Finally, destroy the projectile.
                    GameObject dripAnim = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Soak_Drip"), coll.gameObject.transform.position, Quaternion.identity) as GameObject;
                    dripAnim.transform.parent = coll.gameObject.transform;

                    //Slow down the player.
                    coll.gameObject.GetComponent<PlayerScript>().MaxVelocity = 0.25f;
                    coll.gameObject.GetComponent<PlayerScript>().Acceleration = 1.0f;

                    //Colour the player.
                    if (coll.gameObject.GetComponent<SpriteRenderer>() != null)
                    {
                        coll.gameObject.GetComponent<SpriteRenderer>().color = soakColour;
                        coll.gameObject.transform.FindChild("PlayerArm").GetComponent<SpriteRenderer>().color = soakColour;
                    }

                    GameObject.Destroy(gameObject);
                }
            }
        }
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
