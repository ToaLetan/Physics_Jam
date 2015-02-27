using UnityEngine;
using System.Collections;

public class ActiveProjectileScript : MonoBehaviour 
{
    private const float DESTINATION_DEADZONE = 0.15f;

    public enum ActiveProjectileType { SpeedUp, SpeedDown, Soak };

    public ActiveProjectileType ProjectileType = ActiveProjectileType.SpeedUp;

    private PlayerScript ownerPlayer = null;

    private Vector3 destination;

    private float velocity = 1.5f;

    private bool hasReachedDestination = false;

    public PlayerScript OwnerPlayer
    {
        set { ownerPlayer = value; }
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
	
	}
	
	// Update is called once per frame
	void Update () 
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
                switch(ProjectileType)
                {
                    case ActiveProjectileType.SpeedUp:
                        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_Splash"), gameObject.transform.position, Quaternion.identity);
                        break;
                    case ActiveProjectileType.SpeedDown:
                        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_SlowSplash"), gameObject.transform.position, Quaternion.identity);
                        break;
                    case ActiveProjectileType.Soak:
                        break;

                }
                GameObject.Destroy(gameObject);
            }
        }
	}

    private void UpdateMovement()
    {
        gameObject.transform.position += gameObject.transform.right * velocity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (ProjectileType == ActiveProjectileType.Soak)
        {
            if (coll.gameObject.GetComponent<PlayerScript>() != null && coll.gameObject.GetComponent<PlayerScript>().PlayerNumber != ownerPlayer.PlayerNumber)
            {
                //Flag the player as being soaked, reduce their speed and instantiate and animated prefab attached to them. Finally, destroy the projectile.

                Debug.Log("Projectile collided with player");

                GameObject.Destroy(gameObject);
            }
        }
    }
}
