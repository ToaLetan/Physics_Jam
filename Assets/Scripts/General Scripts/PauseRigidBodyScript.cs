using UnityEngine;
using System.Collections;

public class PauseRigidBodyScript : MonoBehaviour 
{
    public Vector3 pausedVelocity;
    public float pausedAngularVelocity;

	// Use this for initialization
	void Start () 
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GamePaused += OnGamePaused;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameResumed += OnGameResumed;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void OnGamePaused()
    {
        if (gameObject.transform.GetComponent<Rigidbody2D>() != null)
        {
            pausedVelocity = gameObject.transform.GetComponent<Rigidbody2D>().velocity;
            pausedAngularVelocity = gameObject.transform.GetComponent<Rigidbody2D>().angularVelocity;
            gameObject.transform.GetComponent<Rigidbody2D>().Sleep();
        }
    }
    
    private void OnGameResumed()
    {
        if (gameObject.transform.GetComponent<Rigidbody2D>() != null)
        {
            //gameObject.transform.GetComponent<Rigidbody2D>().AddForce(pausedVelocity * 50); //THIS IS REALLY CHEATY. LIKE USING A GAMESHARK TO COMPLETE YOUR POKEDEX LEVEL OF CHEATY.
            gameObject.transform.GetComponent<Rigidbody2D>().AddForce(pausedVelocity, ForceMode2D.Impulse);
            gameObject.transform.GetComponent<Rigidbody2D>().AddTorque(pausedAngularVelocity);
            gameObject.transform.GetComponent<Rigidbody2D>().WakeUp();
        }
    }

    public void UnsubscribeFromEvents()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GamePaused -= OnGamePaused;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().GameResumed -= OnGameResumed;
    }
}
