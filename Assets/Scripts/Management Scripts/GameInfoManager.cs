using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInfoManager
{
    private const int NUM_MAX_PLAYERS = 4;

    private static GameInfoManager instance = null;

    public List<Color> PlayerColours = new List<Color>();
    public List<Active.ActiveType> PlayerActives = new List<Active.ActiveType>();
    public List<string> PlayerInputSources = new List<string>(); //Where players are getting their controls from (controller 1-4 or keyboard 1-2) by name.
    public List<bool> JoinedPlayers = new List<bool>(); //Whether or not players 1-4 have joined.
    
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
        InitializeInfo();
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void InitializeInfo()
    {
        //Default everyone to white, add 4 blank input sources and set 4 players to joined = false.
        for (int i = 0; i < NUM_MAX_PLAYERS; i++)
        {
            PlayerColours.Add(Color.white);
            PlayerActives.Add(Active.ActiveType.GravField);
            PlayerInputSources.Add("");
            JoinedPlayers.Add(false);
        }
    }

    public void Reset()
    {
        //Wipe all existing info and initialize again.
        PlayerColours.Clear();
        PlayerActives.Clear();
        PlayerInputSources.Clear();
        JoinedPlayers.Clear();

        InitializeInfo();
    }
}
