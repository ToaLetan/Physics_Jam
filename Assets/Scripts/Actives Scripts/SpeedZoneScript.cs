using UnityEngine;
using System.Collections;

public class SpeedZoneScript : MonoBehaviour 
{
    public float SpeedModifier = 2.0f;

    private GameManager gameManager = null;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (gameManager.IsGamePaused == false)
        {

        }
	}
}
