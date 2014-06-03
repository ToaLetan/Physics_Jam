using UnityEngine;
using System.Collections;

public class BeamScript : MonoBehaviour 
{
	private GameObject currentObjectSelected = null;
	private bool isHoldingObject = false;

	public GameObject CurrentObjectSelected
	{
		get { return currentObjectSelected; }
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerEnter2D(Collider2D collisionObj)
	{
		switch (collisionObj.gameObject.tag) 
		{
			case "PhysicsObj":
				if(!isHoldingObject)
				{
					currentObjectSelected = collisionObj.gameObject;
					isHoldingObject = true;
				}
				break;
		}
	}

	void OnTriggerExit2D(Collider2D collisionObj)
	{
		if (currentObjectSelected != null) 
		{
			if(collisionObj.gameObject == currentObjectSelected)
				currentObjectSelected = null;
		}
	}
}
