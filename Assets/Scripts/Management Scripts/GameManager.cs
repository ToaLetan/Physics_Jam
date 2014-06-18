using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
    public List<GameObject> PlayerList = new List<GameObject>();

	// Use this for initialization
	void Start () 
	{
        //Get all players, add them to the exposed PlayerList and subscribe to their Player_Death events.
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            PlayerList.Add(GameObject.FindGameObjectsWithTag("Player") [i]);
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
