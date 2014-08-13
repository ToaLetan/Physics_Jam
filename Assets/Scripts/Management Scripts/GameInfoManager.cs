using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInfoManager
{
    private const int NUM_MAX_PLAYERS = 4;

    private static GameInfoManager instance = null;

    public List<Color> PlayerColours = new List<Color>();
    public List<string> PlayerInputSources = new List<string>(); //Where players are getting their controls from (controller 1-4 or keyboard 1-2) by name.
    
    public static GameInfoManager Instance
    {
        get
        {
            if(instance == null)
                instance = new GameInfoManager();
            return instance;
        }
    }
    
    // Use this for initialization
    private GameInfoManager()
    {
        //Default all player colours to white in case something goes wrong.
        for (int i = 0; i < NUM_MAX_PLAYERS; i++)
		{
			PlayerColours.Add(Color.white);
            PlayerInputSources.Add("");
		}
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
