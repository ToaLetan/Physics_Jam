using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravFieldScript : MonoBehaviour 
{
    private const float PULL_FORCE = 0.5f;
    private const float ALPHA = 0.75f;

    private List<GameObject> objectsInRange = new List<GameObject>();

    private GameManager gameManager = null;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //Make the grav field slightly transparent
        Color newColour = gameObject.GetComponent<SpriteRenderer>().color;
        newColour.a = ALPHA;
        gameObject.GetComponent<SpriteRenderer>().color = newColour;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(gameManager.IsGamePaused == false)
            PullNearbyObjects();
	}

    private void OnTriggerEnter2D(Collider2D colliderObj)
    {
        if (colliderObj.gameObject.tag == "PhysicsObj")
        {
            if (objectsInRange.Contains(colliderObj.gameObject) == false)
                objectsInRange.Add(colliderObj.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D colliderObj)
    {
        if (colliderObj.gameObject.tag == "PhysicsObj")
        {
            if (objectsInRange.Contains(colliderObj.gameObject) == true)
                objectsInRange.Remove(colliderObj.gameObject);
        }
    }

    private void PullNearbyObjects()
    {
        if (objectsInRange.Count > 0)
        {
            for (int i = 0; i < objectsInRange.Count; i++)
            {
                if (objectsInRange[i].GetComponent<Rigidbody2D>() != null)
                {
                    Vector2 vectorToGravCenter = new Vector2(gameObject.transform.position.x - objectsInRange[i].transform.position.x, 
                                                                gameObject.transform.position.y - objectsInRange[i].transform.position.y);
                    objectsInRange[i].GetComponent<Rigidbody2D>().AddForce(vectorToGravCenter);
                }
            }
        }
    }
}
