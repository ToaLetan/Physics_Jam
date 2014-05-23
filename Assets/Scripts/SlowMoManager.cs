using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlowMoManager
{
	public float SlowMoSpeed = 1.0f; //1.0f being normal game speed.

	private float slowMoTimer = 0.0f;
	private float currentSlowMoDuration = 0.0f;
	private bool isSlowMoRunning = false;

	private GameObject[] physicsObjects;

	private static SlowMoManager instance = null;

	public static SlowMoManager Instance
	{
		get
		{
			if(instance == null)
				instance = new SlowMoManager();
			return instance;
		}
	}

	// Use this for initialization
	private SlowMoManager()
	{

	}
	
	// Update is called once per frame
	public void Update () 
	{
		UpdateSlowMo ();
	}

	public void GetAllPhysicsObjects()
	{
		physicsObjects = GameObject.FindGameObjectsWithTag ("PhysicsObj");

		for(int i = 0; i < physicsObjects.Length; i++)
		{
			Debug.Log(physicsObjects[i].name);
		}
	}

	public void SlowMoTime(float timeSpeed, float duration) //Change the game speed for a specified length of time.
	{
		currentSlowMoDuration = duration;
		SlowMoSpeed = timeSpeed;
		isSlowMoRunning = true;

		for(int i = 0; i < physicsObjects.Length; i++)
		{
			physicsObjects[i].GetComponent<Rigidbody2D>().drag = 10/SlowMoSpeed;
			physicsObjects[i].GetComponent<Rigidbody2D>().angularDrag = 10/SlowMoSpeed;
		}
	}

	private void UpdateSlowMo()
	{
		if (isSlowMoRunning) 
		{
			slowMoTimer += Time.deltaTime;

			if(slowMoTimer >= currentSlowMoDuration) //Reset timer, reset game speed to be 1.
			{
				isSlowMoRunning = false;
				slowMoTimer = 0;
				SlowMoSpeed = 1.0f;

				for(int i = 0; i < physicsObjects.Length; i++)
				{
					physicsObjects[i].GetComponent<Rigidbody2D>().drag = 0;
					physicsObjects[i].GetComponent<Rigidbody2D>().angularDrag = 0;
				}
			}
		}
	}
}
