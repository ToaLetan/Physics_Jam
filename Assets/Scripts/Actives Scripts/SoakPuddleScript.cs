using UnityEngine;
using System.Collections;

public class SoakPuddleScript : MonoBehaviour 
{
    private const float PUDDLE_LIFE_DURATION = 15.0f;

    private AnimationObject animationScript = null;

    private Timer lifeTimer;

    public Timer LifeTimer
    {
        get { return lifeTimer; }
        set { lifeTimer = value; }
    }

	// Use this for initialization
	void Start () 
    {
        animationScript = gameObject.GetComponent<AnimationObject>();
        animationScript.Animation_Complete += OnSpawnAnimComplete;

        lifeTimer = new Timer(PUDDLE_LIFE_DURATION);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (lifeTimer != null)
            lifeTimer.Update();
	}

    private void OnSpawnAnimComplete()
    {
        //Unsub and start the life timer.
        animationScript.Animation_Complete -= OnSpawnAnimComplete;

        lifeTimer.StartTimer();
        lifeTimer.OnTimerComplete += OnLifeTimerEnded;

        Debug.Log("~Done spawn~");
    }

    private void OnLifeTimerEnded()
    {
        lifeTimer.OnTimerComplete -= OnLifeTimerEnded;

        //Play the despawn animation
        gameObject.GetComponent<Animator>().enabled = true;
        gameObject.GetComponent<Animator>().Play("SoakPuddle_Despawn", -1, 0);
        animationScript.Animation_Complete += OnDespawnAnimComplete;

        Debug.Log("~Done life~");
    }

    private void OnDespawnAnimComplete()
    {
        Destroy(gameObject);
    }
}
