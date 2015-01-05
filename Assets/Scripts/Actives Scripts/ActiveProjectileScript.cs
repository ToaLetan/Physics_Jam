using UnityEngine;
using System.Collections;

public class ActiveProjectileScript : MonoBehaviour 
{
    private const float DESTINATION_DEADZONE = 0.15f;

    public enum ActiveProjectileType { SpeedUp, SpeedDown, Soak };

    public ActiveProjectileType ProjectileType = ActiveProjectileType.SpeedUp;

    private Vector3 destination;

    private float velocity = 1.5f;

    private bool hasReachedDestination = false;

    public Vector3 Destination
    { 
        set { destination = value; } 
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
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_Splash"), gameObject.transform.position, Quaternion.identity);

                GameObject.Destroy(gameObject);
            }
        }
	}

    private void UpdateMovement()
    {
        gameObject.transform.position += gameObject.transform.right * velocity * Time.deltaTime;
    }
}
