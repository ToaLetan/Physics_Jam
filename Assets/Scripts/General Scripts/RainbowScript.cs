using UnityEngine;
using System.Collections;

public class RainbowScript : MonoBehaviour 
{
    private const float COLOURTIME = 1.5f;
    private const float FADESPEED = 5.0f;

    private Timer colourTimer;

    private Color targetColour;

	// Use this for initialization
	void Start () 
    {
        colourTimer = new Timer(COLOURTIME, true);
        colourTimer.OnTimerComplete += ChangeColour;

        if (gameObject.GetComponent<SpriteRenderer>() != null)
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;

        ChangeColour();
	}
	
	// Update is called once per frame
	void Update () 
    {
        colourTimer.Update();

        if (colourTimer.IsTimerRunning == true)
            TweenColour();
	}

    private void TweenColour()
    {
        if (gameObject.GetComponent<SpriteRenderer>() != null && gameObject.GetComponent<SpriteRenderer>().color != targetColour)
        {
            float diffR = (targetColour.r - gameObject.GetComponent<SpriteRenderer>().color.r) * FADESPEED * Time.deltaTime;
            float diffG = (targetColour.g - gameObject.GetComponent<SpriteRenderer>().color.g) * FADESPEED * Time.deltaTime;
            float diffB = (targetColour.b - gameObject.GetComponent<SpriteRenderer>().color.b) * FADESPEED * Time.deltaTime;

            Color newColour = new Color(gameObject.GetComponent<SpriteRenderer>().color.r + diffR,
                                        gameObject.GetComponent<SpriteRenderer>().color.g + diffG, 
                                        gameObject.GetComponent<SpriteRenderer>().color.b + diffB, 1.0f);
            gameObject.GetComponent<SpriteRenderer>().color = newColour;
        }
    }

    private void ChangeColour()
    {
        //Randomize a new colour value.
        float randR = Random.Range(0.0f, 1.0f);
        float randG = Random.Range(0.0f, 1.0f);
        float randB = Random.Range(0.0f, 1.0f);

        targetColour = new Color(randR, randG, randB, 1.0f);

        /*if(gameObject.GetComponent<SpriteRenderer>() != null)
        {
            gameObject.GetComponent<SpriteRenderer>().color = targetColour;
        }*/

        if (colourTimer.IsTimerRunning == false)
            colourTimer.ResetTimer(true);
    }
}
