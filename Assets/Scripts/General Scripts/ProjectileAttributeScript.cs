using UnityEngine;
using System.Collections;

public class ProjectileAttributeScript : MonoBehaviour 
{
    private const float ROTATIONSPEED = 45.0f;

    public enum ProjectileType { Boomerang, Homing }
    private ProjectileType currentProjectileType;

    private Vector3 targetPoint = Vector3.zero;
    private Timer lifeTimer;

    public ProjectileType CurrentProjectileType
    {
        get { return currentProjectileType; }
        set { currentProjectileType = value; } 
    }

    public Vector3 TargetPoint
    {
        get { return targetPoint; }
        set { targetPoint = value; }
    }

	// Use this for initialization
	void Start () 
    {
        lifeTimer = new Timer(2.5f, true);
        lifeTimer.StartTimer();
        lifeTimer.OnTimerComplete += OnLifeTimerComplete;
	}
	
	// Update is called once per frame
	void Update () 
    {
        lifeTimer.Update();

        if (lifeTimer.IsTimerRunning == true)
        {
            switch(currentProjectileType)
            {
                case ProjectileType.Boomerang:
                    gameObject.transform.RotateAround(targetPoint, Vector3.forward, ROTATIONSPEED * Time.deltaTime);
                    break;
                case ProjectileType.Homing:
                    //gameObject.transform.RotateAround(targetPoint, Vector3.forward, ROTATIONSPEED * Time.deltaTime);
                    break;
            }
        }
	}

    void OnLifeTimerComplete() //Remove the boomerang component once the timer has stopped.
    {
        Destroy(this);
    }
}
