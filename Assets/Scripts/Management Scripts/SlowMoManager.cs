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
	}

	public void SlowMoTime(float timeSpeed, float duration) //Change the game speed for a specified length of time.
	{
		currentSlowMoDuration = duration;
		SlowMoSpeed = timeSpeed;
		isSlowMoRunning = true;

        ApplySlowMotion(false);
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
                //SlowMoSpeed = 1.0f;

                ApplySlowMotion(true);
			}
		}
	}

    private void ApplySlowMotion(bool returnToNormalSpeed)
    {
        if (returnToNormalSpeed == false)
        {
            Time.timeScale = Time.timeScale * SlowMoSpeed;  
            Time.fixedDeltaTime = Time.fixedDeltaTime * SlowMoSpeed;  
            Time.maximumDeltaTime = Time.maximumDeltaTime * SlowMoSpeed;
        } 
        else
        {
            Time.timeScale = Time.timeScale / SlowMoSpeed;  
            Time.fixedDeltaTime = Time.fixedDeltaTime / SlowMoSpeed;  
            Time.maximumDeltaTime = Time.maximumDeltaTime / SlowMoSpeed;

            SlowMoSpeed = 1.0f;
        }

        Debug.Log(Time.timeScale);
    }
}
