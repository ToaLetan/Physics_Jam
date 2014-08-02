using UnityEngine;
using System.Collections;

public class PickupAlertScript : MonoBehaviour 
{
    private const float MAXLIFETIME = 2.5f;
    private const float MOVESPEED = 0.5f;
    private const float FADESPEED = 1.0f;

    private Timer lifeTimer;

	// Use this for initialization
	void Start () 
    {
        lifeTimer = new Timer(MAXLIFETIME, true);
        lifeTimer.OnTimerComplete += OnLifeTimerComplete;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().IsGamePaused == false)
        {
            lifeTimer.Update();

            //Gradually move the object upwards and fade.
            Vector3 newPos = gameObject.transform.position + new Vector3(0, MOVESPEED * Time.deltaTime, 0);
            gameObject.transform.position = newPos;

            if(gameObject.GetComponent<SpriteRenderer>().color.a > 0)
            {
                Color newColour = gameObject.GetComponent<SpriteRenderer>().color;
                newColour.a -= (FADESPEED * Time.deltaTime);
                gameObject.GetComponent<SpriteRenderer>().color = newColour;
            }
        }
	}

    public void PickupText(string pickupName)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/In-Game/PickupAlert_" + pickupName);
    }

    private void OnLifeTimerComplete()
    {
        Destroy(gameObject);
    }
}
