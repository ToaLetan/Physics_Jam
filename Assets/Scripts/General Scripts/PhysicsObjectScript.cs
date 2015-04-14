using UnityEngine;
using System.Collections;

public class PhysicsObjectScript : MonoBehaviour 
{
    private const float MIN_SLOWMO_TRIGGER_SPEED = 0.5f;
    private const float MAX_VELOCITY_MOVEMENT = 5.0f;
    private const float MAX_VELOCITY_ROTATION = 10.0f;
    private const float MIN_VELOCITY_ROTATION = 0.5f;
    private const float SCALE_FACTOR = -0.85f;

    private Timer attributeRemovalTimer;

    private Vector3 startPosition;
    private Vector3 startScale;
    private Quaternion startRotation;

    private bool isFalling = false;
    private bool capVelocity = true;
    private bool hasSpedUp = false;

	// Use this for initialization
	void Start () 
    {
        startPosition = gameObject.transform.position;
        startScale = gameObject.transform.localScale;
        startRotation = gameObject.transform.rotation;

        attributeRemovalTimer = new Timer(0.5f, false);
        attributeRemovalTimer.OnTimerComplete += OnRemovalTimerComplete;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(capVelocity == true)
            CapVelocity();

        if (attributeRemovalTimer != null)
        {
            if(attributeRemovalTimer.IsTimerRunning == true)
                attributeRemovalTimer.Update();
        }

        if (isFalling == true)
            PlayFallAnim();
	}

    private void OnCollisionEnter2D(Collision2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "Player")
        {
            if(collisionObj.gameObject.GetComponent<PlayerScript>().CanMove == true && 
               (gameObject.transform.GetComponent<Rigidbody2D>().velocity.x > MIN_SLOWMO_TRIGGER_SPEED ||
                gameObject.transform.GetComponent<Rigidbody2D>().velocity.y > MIN_SLOWMO_TRIGGER_SPEED) )
            {
                if (SlowMoManager.Instance.IsSlowMoRunning == false) //Play the slow motion effect.
                {
                    if (GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("SlowmoEmphasis(Clone)") == null)
                    {
                        GameObject slowmoEmphasis = GameObject.Instantiate(Resources.Load("Prefabs/AnimatedPrefabs/SlowmoEmphasis")) as GameObject;

                        slowmoEmphasis.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
                        slowmoEmphasis.transform.localPosition = new Vector3(0, 0, 1);

                        slowmoEmphasis.GetComponent<AnimationObject>().Animation_Complete += OnSlowmoAnimComplete;
                    }
                }
            }
            //Reflect along the initial trajectory if the player's Reflect Active is in use.
            if (collisionObj.gameObject.GetComponent<PlayerScript>().PlayerActive != null
                && collisionObj.gameObject.GetComponent<PlayerScript>().currentActiveType == Active.ActiveType.Reflect 
                && collisionObj.gameObject.GetComponent<PlayerScript>().PlayerActive.IsReflective == true)
            {
                Vector2 newVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;

                newVelocity *= -1;

                gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
            }

            //If the object is a homing projectile, run a timer to remove the homing attribute.
            if (gameObject.GetComponent<ProjectileAttributeScript>() != null)
            {
                if (gameObject.GetComponent<ProjectileAttributeScript>().CurrentProjectileType == ProjectileAttributeScript.ProjectileType.Homing
                    && attributeRemovalTimer.IsTimerRunning == false)
                {
                    attributeRemovalTimer.StartTimer();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "KillBox")
        {
            //Disable the rigidbody
            gameObject.transform.GetComponent<BoxCollider2D>().enabled = false;

            if(isFalling == false)
                isFalling = true;
        }
        else if (collisionObj.gameObject.tag == "ActiveZone")
        {
            if (collisionObj.gameObject.GetComponent<SpeedZoneScript>() != null)
            {
                if (hasSpedUp == false) //Prevent speed boosts from stacking.
                {
                    hasSpedUp = true;
                    capVelocity = false;

                    Vector2 newVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;

                    newVelocity *= collisionObj.gameObject.GetComponent<SpeedZoneScript>().SpeedModifier;

                    gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
                }
            }
        }
    }

    private void Respawn()
    {
        isFalling = false;
        hasSpedUp = false;

        gameObject.transform.position = startPosition;
        gameObject.transform.localScale = startScale;
        gameObject.transform.rotation = startRotation;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;

        gameObject.transform.GetComponent<BoxCollider2D>().enabled = true;

        capVelocity = true;

        if (gameObject.GetComponent<ProjectileAttributeScript>() != null)
        {
            Destroy(gameObject.GetComponent<ProjectileAttributeScript>() );
        }

        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/AnimatedPrefabs/SpawnAnimation"), gameObject.transform.position, gameObject.transform.rotation);
    }

    private void OnSlowmoAnimComplete()
    {
        SlowMoManager.Instance.SlowMoTime(0.25f, 0.3f);
        SlowMoManager.Instance.SlowMoEnded += OnSlowMoStopped;
    }

    private void OnSlowMoStopped()
    {
        //Remove the slowmo emphasizer.
        if (GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("SlowmoEmphasis(Clone)") != null)
        {
            GameObject.Destroy(GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("SlowmoEmphasis(Clone)").gameObject);
        }
    }

    private void OnRemovalTimerComplete()
    {
        if (gameObject.GetComponent<ProjectileAttributeScript>() != null)
            Destroy(gameObject.GetComponent<ProjectileAttributeScript>());

        //attributeRemovalTimer.OnTimerComplete -= OnRemovalTimerComplete;
    }

    private void CapVelocity()
    {
        if (gameObject.GetComponent<Rigidbody2D>().velocity.x != 0 || gameObject.GetComponent<Rigidbody2D>().velocity.y != 0 || gameObject.GetComponent<Rigidbody2D>().angularVelocity != 0)
        {
            Vector2 newVelocity = gameObject.gameObject.GetComponent<Rigidbody2D>().velocity;
            float newAngularVelocity = gameObject.gameObject.GetComponent<Rigidbody2D>().angularVelocity;

            //Cap movement velocity
            if (gameObject.GetComponent<Rigidbody2D>().velocity.x > MAX_VELOCITY_MOVEMENT)
                newVelocity.x = MAX_VELOCITY_MOVEMENT;
            if (gameObject.GetComponent<Rigidbody2D>().velocity.x < -MAX_VELOCITY_MOVEMENT)
                newVelocity.x = -MAX_VELOCITY_MOVEMENT;

            if (gameObject.GetComponent<Rigidbody2D>().velocity.y > MAX_VELOCITY_MOVEMENT)
                newVelocity.y = MAX_VELOCITY_MOVEMENT;
            if (gameObject.GetComponent<Rigidbody2D>().velocity.y < -MAX_VELOCITY_MOVEMENT)
                newVelocity.y = -MAX_VELOCITY_MOVEMENT;

            //Cap rotation velocity
            if (gameObject.GetComponent<Rigidbody2D>().angularVelocity > MAX_VELOCITY_ROTATION)
                newAngularVelocity = MAX_VELOCITY_ROTATION;
            if (gameObject.GetComponent<Rigidbody2D>().angularVelocity < -MAX_VELOCITY_ROTATION)
                newAngularVelocity = -MAX_VELOCITY_ROTATION;

            //Stop constant rotation
            if (gameObject.GetComponent<Rigidbody2D>().angularVelocity < MIN_VELOCITY_ROTATION)
                newAngularVelocity = 0;
            if (gameObject.GetComponent<Rigidbody2D>().angularVelocity > -MIN_VELOCITY_ROTATION)
                newAngularVelocity = 0;

            gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = newAngularVelocity;
        }
    }

    private void PlayFallAnim()
    {
        if (gameObject.transform.localScale.x > (0.5f) || gameObject.transform.localScale.y > (0.5f))
        {
            //Scale the object down.
            Vector2 newScale = new Vector2(gameObject.transform.localScale.x + (SCALE_FACTOR * startScale.x * Time.deltaTime), 
                                            gameObject.transform.localScale.y + (SCALE_FACTOR * startScale.y * Time.deltaTime));

            gameObject.transform.localScale = newScale;
        }
        else
        {
            if (gameObject.name.Contains("(Clone)"))
            {
                //Unscribe to any events and destroy the object.
                if (gameObject.GetComponent<PauseRigidBodyScript>() != null)
                    gameObject.GetComponent<PauseRigidBodyScript>().UnsubscribeFromEvents();
                Destroy(gameObject);
            }
            else
                Respawn();
        }
    }
}
