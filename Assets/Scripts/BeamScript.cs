using UnityEngine;
using System.Collections;

public class BeamScript : MonoBehaviour 
{
	private GameObject currentObjectSelected = null;

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

	// TOO FRAMEY, FIND A BETTER SOLUTION.

	/*void OnTriggerEnter2D(Collider2D collisionObj)
	{
		switch (collisionObj.gameObject.tag) 
		{
			case "PhysicsObj":
				currentObjectSelected = collisionObj.gameObject;
				Debug.Log(currentObjectSelected.name);
				break;
		}
	}*/
}
