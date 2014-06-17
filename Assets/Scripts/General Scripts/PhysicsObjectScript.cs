using UnityEngine;
using System.Collections;

public class PhysicsObjectScript : MonoBehaviour 
{
    private GameManager gameManager = null;

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    private void OnCollisionEnter2D(Collision2D collisionObj)
    {
        if (collisionObj.gameObject.tag == "Player")
        {
            if(SlowMoManager.Instance.SlowMoSpeed == 1.0f)
                SlowMoManager.Instance.SlowMoTime(0.25f, 0.5f);
        }
    }
}
