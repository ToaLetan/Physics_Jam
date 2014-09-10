using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour 
{
    private const int MAX_NUM_OF_PICKUPS = 5; //The limit for the amount of pickups that can exist on the map at any given time.

    private Timer pickupSpawnTime;

    private GameObject spawningPickup = null;

    private bool hasReachedPickupCap = false; //Whether or not the max amount of pickups has been reached. If so, no new ones should spawn.

	// Use this for initialization
	void Start () 
    {
        RandomizeSpawnTime();
        pickupSpawnTime.OnTimerComplete += SpawnPickup;
	}
	
	// Update is called once per frame
	void Update () 
    {
        UpdatePickupCap();

        //Only update the pickup spawn timer as long as the game is running and the cap hasn't been reached.
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IsGamePaused == false && hasReachedPickupCap == false)
        {
            if (pickupSpawnTime != null)
                pickupSpawnTime.Update();
        }
	}

    private void RandomizeSpawnTime()
    {
        if (pickupSpawnTime == null)
            pickupSpawnTime = new Timer(Random.Range(5.0f, 10.0f), true);
        else
        {
            pickupSpawnTime.TargetTime = Random.Range(5.0f, 10.0f);
            pickupSpawnTime.ResetTimer(true);
        }
    }

    private void SpawnPickup()
    {
        BoxCollider2D spawnZone = gameObject.GetComponent<BoxCollider2D>();

        //Randomize a location within the spawn zone.
        float spawnBoundsLeft = spawnZone.bounds.min.x;
        float spawnBoundsRight = spawnZone.bounds.max.x;
        float spawnBoundsTop = spawnZone.bounds.min.y;
        float spawnBoundsBottom = spawnZone.bounds.max.y;

        float randSpawnX = Random.Range(spawnBoundsLeft, spawnBoundsRight);
        float randSpawnY = Random.Range(spawnBoundsTop, spawnBoundsBottom);

        //Instantiate the Pickup Spawn Animation and subscribe to the event fired when the animation is done.
        spawningPickup = GameObject.Instantiate(Resources.Load("Prefabs/AnimatedPrefabs/PickupSpawnAnim"), 
            new Vector3(randSpawnX, randSpawnY, gameObject.transform.position.z), gameObject.transform.rotation) as GameObject;

        if (spawningPickup.GetComponent<AnimationObject>() != null)
            spawningPickup.GetComponent<AnimationObject>().Animation_Complete += OnSpawnAnimationComplete;


        //Reset the timer.
        RandomizeSpawnTime();
    }

    private void OnSpawnAnimationComplete()
    {
        GameObject.Instantiate(Resources.Load("Prefabs/AnimatedPrefabs/Pickup"), spawningPickup.transform.position, spawningPickup.transform.rotation);
    }

    private void UpdatePickupCap()
    {
        if (GameObject.FindGameObjectsWithTag("Pickup").Length >= MAX_NUM_OF_PICKUPS)
            hasReachedPickupCap = true;
        else
            hasReachedPickupCap = false;
    }
}
