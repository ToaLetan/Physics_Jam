using UnityEngine;
using System.Collections;

public class PickupSpawner : MonoBehaviour 
{
    private Timer pickupSpawnTime;

	// Use this for initialization
	void Start () 
    {
        RandomizeSpawnTime();
        pickupSpawnTime.OnTimerComplete += SpawnPickup;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IsGamePaused == false)
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

        GameObject.Instantiate(Resources.Load("Prefabs/Pickup"), new Vector3(randSpawnX, randSpawnY, gameObject.transform.position.z), gameObject.transform.rotation );

        //Reset the timer.
        RandomizeSpawnTime();
    }
}
