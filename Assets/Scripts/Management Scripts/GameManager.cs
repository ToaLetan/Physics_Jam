using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		SlowMoManager.Instance.GetAllPhysicsObjects();
		//SlowMoManager.Instance.SlowMoTime (0.25f, 2.0f);

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerScript>().Player_Death += OnPlayerDeath;
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update any management systems
		InputManager.Instance.Update();
		SlowMoManager.Instance.Update();
        UIManager.Instance.Update();
	}

    private void OnPlayerDeath(int playerNum)
    {
        //Remove 1 life from the player.
        UIManager.Instance.RemoveLife(playerNum);
    }
}
