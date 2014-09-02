using UnityEngine;
using System.Collections;

public class PhysicsObjectScript : MonoBehaviour 
{
    public const float MIN_SLOWMO_TRIGGER_SPEED = 0.5f;

    private Vector3 startPosition;
    private Vector3 startScale;
    private Quaternion startRotation;

	// Use this for initialization
	void Start () 
    {
        startPosition = gameObject.transform.position;
        startScale = gameObject.transform.localScale;
        startRotation = gameObject.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void OnCollisionEnter2D(Collision2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "Player")
        {
            if(collisionObj.gameObject.GetComponent<PlayerScript>().CanMove == true && 
               (gameObject.transform.GetComponent<Rigidbody2D>().velocity.x > MIN_SLOWMO_TRIGGER_SPEED ||
                gameObject.transform.GetComponent<Rigidbody2D>().velocity.y > MIN_SLOWMO_TRIGGER_SPEED) )
            {
                if (SlowMoManager.Instance.IsSlowMoRunning == false)
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "KillBox")
        {
            if(gameObject.name.Contains("(Clone)") )
            {
                //Unscribe to any events and destroy the object.
                if(gameObject.GetComponent<PauseRigidBodyScript>() != null)
                    gameObject.GetComponent<PauseRigidBodyScript>().UnsubscribeFromEvents();
                Destroy(gameObject);
            }
            else
                Respawn();
        }
    }

    private void Respawn()
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.localScale = startScale;
        gameObject.transform.rotation = startRotation;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;

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
}
