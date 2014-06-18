using UnityEngine;
using System.Collections;

public class PhysicsObjectScript : MonoBehaviour 
{
    private Vector3 startPosition;
    private Quaternion startRotation;

	// Use this for initialization
	void Start () 
    {
        startPosition = gameObject.transform.position;
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
            if(collisionObj.gameObject.GetComponent<PlayerScript>().CanMove == true)
            {
                if(SlowMoManager.Instance.SlowMoSpeed == 1.0f)
                    SlowMoManager.Instance.SlowMoTime(0.25f, 0.5f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "KillBox")
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        gameObject.transform.position = startPosition;
        gameObject.transform.rotation = startRotation;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.transform.GetComponent<Rigidbody2D>().angularVelocity = 0;
        
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SpawnAnimation"), gameObject.transform.position, gameObject.transform.rotation);
    }
}
