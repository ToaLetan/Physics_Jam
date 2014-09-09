using UnityEngine;
using System.Collections;

public class ProjectileAttributeScript : MonoBehaviour 
{
    private const float ROTATIONSPEED = 45.0f;
    private const float HOMINGVELOCITY = 2.5f;

    public enum ProjectileType { Boomerang, Homing }
    private ProjectileType currentProjectileType;

    private GameObject targetPlayer = null;

    private Vector3 targetPoint = Vector3.zero;

    private Timer lifeTimer;

    public ProjectileType CurrentProjectileType
    {
        get { return currentProjectileType; }
        set { currentProjectileType = value; } 
    }

    public GameObject TargetPlayer
    {
        get { return targetPlayer; }
        set { targetPlayer = value; }
    }

    public Vector3 TargetPoint
    {
        get { return targetPoint; }
        set { targetPoint = value; }
    }

	// Use this for initialization
	void Start () 
    {
        lifeTimer = new Timer(2.5f, true);
        lifeTimer.StartTimer();
        lifeTimer.OnTimerComplete += OnLifeTimerComplete;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IsGamePaused == false)
        {
            lifeTimer.Update();

            if (lifeTimer.IsTimerRunning == true)
            {
                switch (currentProjectileType)
                {
                    case ProjectileType.Boomerang: //Rotate around a point.
                        gameObject.transform.RotateAround(targetPoint, Vector3.forward, ROTATIONSPEED * Time.deltaTime);
                        break;
                    case ProjectileType.Homing: //Rotate and move towards the target position.
                        //Get the angle towards the target and rotate.

                        targetPoint = targetPlayer.transform.position;

                        float angleToTarget = Mathf.Atan2(targetPoint.y - gameObject.transform.position.y, targetPoint.x - gameObject.transform.position.x) 
                            * Mathf.Rad2Deg;

                        gameObject.transform.rotation = Quaternion.Euler(0, 0, angleToTarget);

                        //Move towards the target.
                        //First, calculate the magnitude of the current velocity.
                        //float velocityMagnitude = Mathf.Sqrt(Mathf.Pow(gameObject.GetComponent<Rigidbody2D>().velocity.x, 2)
                                                             //+ Mathf.Pow(gameObject.GetComponent<Rigidbody2D>().velocity.y, 2) );

                        Vector3 newPosition = gameObject.transform.position + (gameObject.transform.right * HOMINGVELOCITY * Time.deltaTime);
                        gameObject.transform.position = newPosition;

                        break;
                }
            }
        }
	}

    void OnLifeTimerComplete() //Remove the component once the timer has stopped.
    {
        Destroy(this);
    }
}
