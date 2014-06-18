using UnityEngine;
using System.Collections;

public class GameInfoManager
{
    private static GameInfoManager instance = null;

    //GHETTO FOR NOW, FIX IT.
    public Color ColourPlayer1 = Color.white;
    public Color ColourPlayer2 = Color.white;
    
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
        
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
