using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInfoManager
{
    private static GameInfoManager instance = null;

	public List<Color> PlayerColours = new List<Color> ();
    
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
        //TEMPORARY
        PlayerColours.Add(Color.white);
        PlayerColours.Add(Color.white);

        //Default all player colours to white in case something goes wrong.
		for(int i = 0; i < PlayerColours.Count; i++)
		{
			PlayerColours[i] = Color.white;
		}
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
