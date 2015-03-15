using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager
{
    private const float LEFT_PLAYERS_X = -1.16f;
    private const float RIGHT_PLAYERS_X = 0.8f;
    private const float TOP_PLAYERS_Y = 0.98f;
    private const float BOTTOM_PLAYERS_Y = -0.86f;
    private const float COOLDOWN_SPACING = 0.014f;

    private GameObject[] playerArray = new GameObject[4];
    private GameObject[] playerNamesArray = new GameObject[4];
    private GameObject[] playerCooldownsArray = new GameObject[4];

    private bool[] activeCooldowns = new bool[4];
    private bool[] flashingCooldowns = new bool[4];

    private List<GameObject> playerLivesList = new List<GameObject>();

    private GameManager gameManager = null;

    private Camera camera = null;

    private GameObject combinedUI = null;
    private GameObject winnerText = null;
    private GameObject endPromptText = null;

    private Color colourHide = new Color(0, 0, 0, 0);
    private Color colourFade = new Color(0.17f, 0.23f, 0.25f, 1.0f);

    private static UIManager instance = null;

    public static UIManager Instance
    {
        get
        {
            if(instance == null)
                instance = new UIManager();
            return instance;
        }
    }

	// Use this for initialization
	private UIManager() 
    {

	}
	
	// Update is called once per frame
	public void Update () 
    {
        UpdateCooldowns();
	}

    public void ConstructHUD() //Instantiate the HUD, consists of 2-4 Player sectors + info/score/etc., game time remaining.
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            if(playerArray[i] != null && playerNamesArray[i] == null)
            {
                playerNamesArray[i] = GameObject.Instantiate(Resources.Load("Prefabs/GUI/Text_Player" + (i+1) ) ) as GameObject;
                playerNamesArray[i].transform.parent = combinedUI.transform;

                PositionHUDElements(playerNamesArray[i].name);

                GenerateLifeIcons(playerArray[i].GetComponent<PlayerScript>().NumLives, i);

                //Create the cooldown icons and position them accordingly.
                playerCooldownsArray[i] = GameObject.Instantiate(Resources.Load("Prefabs/GUI/Active_Cooldown")) as GameObject;
                playerCooldownsArray[i].transform.parent = combinedUI.transform;

                //Set the cooldown icon to the standard sprite.
                playerCooldownsArray[i].GetComponent<Animator>().Play("CooldownBase", -1);
                playerCooldownsArray[i].GetComponent<Animator>().enabled = false;

                Vector3 cooldownPos = playerNamesArray[i].transform.position;
                cooldownPos.x -= (playerCooldownsArray[i].GetComponent<SpriteRenderer>().sprite.bounds.extents.x) + COOLDOWN_SPACING;

                playerCooldownsArray[i].transform.position = cooldownPos;

                //Subscribe to the Player Event for cooldown timers starting. Hide the cooldown sprite for the time being.
                playerArray[i].GetComponent<PlayerScript>().Player_Cooldown_Start += ShowCooldownDisplay;
                playerCooldownsArray[i].transform.FindChild("Cooldown_TimeDisplay").GetComponent<SpriteRenderer>().color = colourHide;
                activeCooldowns[i] = false;
                flashingCooldowns[i] = false;
            }
        }

        MatchPlayerColours();

        MatchPlayerActives();
    }

    private void PositionHUDElements(string elementName)
    {
        //Positions HUD objects based on camera size. Offset added to leave room for other GUI objects, spacing, etc.
        //z = 1 required for some reason. It's weird.

        float UIOffset = 0.1f;

        elementName = elementName.Remove(elementName.IndexOf("(Clone)") );

        switch (elementName)
        {
            case "Text_Player1":
                playerNamesArray[0].transform.localPosition = new Vector3(LEFT_PLAYERS_X, TOP_PLAYERS_Y, 1);
                break;
            case "Text_Player2":
                playerNamesArray[1].transform.localPosition = new Vector3(RIGHT_PLAYERS_X, TOP_PLAYERS_Y, 1);
                break;
            case "Text_Player3":
                //playerNamesArray[2].transform.localPosition = new Vector3(LEFT_PLAYERS_X, -camera.orthographicSize + (UIOffset * 5), 1);
                playerNamesArray[2].transform.localPosition = new Vector3(LEFT_PLAYERS_X, BOTTOM_PLAYERS_Y, 1);
                break;
            case "Text_Player4":
                //playerNamesArray[3].transform.localPosition = new Vector3(RIGHT_PLAYERS_X, -camera.orthographicSize + (UIOffset * 5), 1);
                playerNamesArray[3].transform.localPosition = new Vector3(RIGHT_PLAYERS_X, BOTTOM_PLAYERS_Y, 1);
                break;
        }
    }

    private void GenerateLifeIcons(int numOfLives, int ownerNumber)
    {
        float UIOffset = 0.1f;

        for (int i = 0; i < numOfLives; i++)
        {
            GameObject newLifeIcon = GameObject.Instantiate(Resources.Load("Prefabs/GUI/Life_Icon") ) as GameObject;

            //float lifeIconWidth = newLifeIcon.transform.GetComponent<SpriteRenderer>().bounds.max.x;
            //float lifeIconHeight = newLifeIcon.transform.GetComponent<SpriteRenderer>().bounds.max.y;

            newLifeIcon.transform.parent = playerNamesArray[ownerNumber].transform;
            //newLifeIcon.transform.localPosition = new Vector3(-UIOffset * 2 + (UIOffset * i * 2), -UIOffset * 1.5f, 0);
            newLifeIcon.transform.localPosition = new Vector3((UIOffset * i * 2), -newLifeIcon.GetComponent<SpriteRenderer>().sprite.bounds.extents.y + COOLDOWN_SPACING, 0);

            playerLivesList.Add(newLifeIcon);
        }
    }

    private void MatchPlayerColours() //Colour all player text and icons to match their player colour.
    {
        for(int i = 0; i < playerArray.Length; i++)
        {
            if(playerArray[i] != null)
            {
                //Set the player name text to the player colour.
                playerNamesArray[i].transform.GetComponent<SpriteRenderer>().material.color = playerArray[i].transform.GetComponent<PlayerScript>().PlayerColour;

                //Set all life icons to match that player's colour.
                for(int j = 0; j < playerNamesArray[i].transform.childCount; j++)
                {
                    playerNamesArray[i].transform.GetChild(j).GetComponent<SpriteRenderer>().material.color = playerArray[i].transform.GetComponent<PlayerScript>().PlayerColour;
                }

                //Set all cooldown icons' backgrounds to match that player's colour
                playerCooldownsArray[i].GetComponent<SpriteRenderer>().color = playerArray[i].transform.GetComponent<PlayerScript>().PlayerColour;
            }
        }
    }

    private void MatchPlayerActives()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (playerArray[i] != null && playerArray[i].GetComponent<PlayerScript>() != null)
            {
                //Get the player's chosen Ability and display it in the UI
                string abilityName = "";

                abilityName = playerArray[i].GetComponent<PlayerScript>().currentActiveType.ToString();

                Debug.Log(abilityName);

                playerCooldownsArray[i].transform.FindChild("Cooldown_Icon").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Cooldowns/Cooldown_" + abilityName);
            }
        }
    }

    public void RemoveLife(int playerNum)
    {
        if(playerNamesArray[playerNum].transform.childCount > 0)
            GameObject.Destroy(playerNamesArray[playerNum].transform.GetChild(playerNamesArray [playerNum].transform.childCount - 1).gameObject );
    }

    //IMPROVE THIS AT A LATER DATE
    public void HideEnding()
    {
        Color objectColour = winnerText.GetComponent<SpriteRenderer>().color;
        objectColour.a = 0;
        winnerText.GetComponent<SpriteRenderer>().color = objectColour;
        
        objectColour = endPromptText.GetComponent<SpriteRenderer>().color;
        objectColour.a = 0;
        endPromptText.GetComponent<SpriteRenderer>().color = objectColour;
    }

    public void ShowEnding(int winnerNum)
    {
        Color objectColour = winnerText.GetComponent<SpriteRenderer>().color;
        objectColour.a = 1;
        winnerText.GetComponent<SpriteRenderer>().color = objectColour;

        /*
        objectColour = endPromptText.GetComponent<SpriteRenderer>().color;
        objectColour.a = 1;
        endPromptText.GetComponent<SpriteRenderer>().color = objectColour;
        */

        //Position the text.
        winnerText.transform.parent = playerNamesArray [winnerNum].transform;
        winnerText.transform.localPosition = new Vector3(0.4f, 0, 0);
       
        endPromptText.transform.localPosition = new Vector3(0, 0, 1);
    }

    public void ResetUI()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform.GetComponent<Camera>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (combinedUI == null)
        {
            combinedUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/CombinedUIHolder"), camera.gameObject.transform.position, camera.gameObject.transform.rotation) as GameObject;
            combinedUI.transform.parent = camera.gameObject.transform;
        }

        if (winnerText == null)
        {
            winnerText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/WinText")) as GameObject;
            winnerText.transform.parent = combinedUI.transform;
        }

        if (endPromptText == null)
        {
            endPromptText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/EndingPrompt")) as GameObject;
            endPromptText.transform.parent = combinedUI.transform;
        }

        //Debug.Log(gameManager.PlayerList.Count); //PROBLEM: FOR SOME REASON THIS IS 3 WHEN THERE'S ONLY 2 PEOPLE
        
        //Arrange the player array to match the player numbers.
        for (int i = 0; i < gameManager.PlayerList.Count; i++)
        {
            if (gameManager.PlayerList[i].transform.GetComponent<PlayerScript>().PlayerNumber == i)
                playerArray[i] = gameManager.PlayerList[i];
        }
        
        ConstructHUD();
        HideEnding();
    }

    private void ShowCooldownDisplay(int playerNum)
    {
        if (playerCooldownsArray[playerNum] != null)
        {
            //Set the Active background colour to a dark grey, un-hide the cooldown display and scale it down, unsub from the event and flag the cooldown display as active.
            playerCooldownsArray[playerNum].GetComponent<SpriteRenderer>().color = colourFade;

            playerCooldownsArray[playerNum].transform.FindChild("Cooldown_TimeDisplay").GetComponent<SpriteRenderer>().color = Color.white;

            Vector3 initialScale = playerCooldownsArray[playerNum].transform.FindChild("Cooldown_TimeDisplay").transform.localScale;
            playerCooldownsArray[playerNum].transform.FindChild("Cooldown_TimeDisplay").transform.localScale = new Vector3(initialScale.x, 0, initialScale.z);

            playerArray[playerNum].GetComponent<PlayerScript>().Player_Cooldown_Complete += CooldownFlash;

            activeCooldowns[playerNum] = true;
        }
    }

    private void HideCooldownDisplay(int playerNum)
    {
        //Match the background colour to the player, hide the cooldown display, unsub from the event.
        playerCooldownsArray[playerNum].transform.FindChild("Cooldown_TimeDisplay").GetComponent<SpriteRenderer>().color = colourHide;
        playerCooldownsArray[playerNum].transform.FindChild("Cooldown_TimeDisplay").localScale = new Vector3(1, 1, 1);

        playerArray[playerNum].GetComponent<PlayerScript>().Player_Cooldown_Complete -= CooldownFlash;
    }

    private void CooldownFlash(int playerNum)
    {
        activeCooldowns[playerNum] = false;
        flashingCooldowns[playerNum] = true;

        HideCooldownDisplay(playerNum);

        playerCooldownsArray[playerNum].GetComponent<SpriteRenderer>().color = Color.white;

        playerCooldownsArray[playerNum].GetComponent<Animator>().enabled = true;
        playerCooldownsArray[playerNum].GetComponent<Animator>().Play("CooldownComplete", -1);

        playerCooldownsArray[playerNum].GetComponent<AnimationObject>().Animation_Complete += OnCooldownFlashComplete;
    }

    private void OnCooldownFlashComplete()
    {
        //Set the cooldown icon to the standard sprite.
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (playerCooldownsArray[i] != null && flashingCooldowns[i] == true)
            {
                flashingCooldowns[i] = false;

                playerCooldownsArray[i].GetComponent<SpriteRenderer>().color = playerArray[i].GetComponent<PlayerScript>().PlayerColour;
                playerCooldownsArray[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/UI/Cooldowns/Cooldown_Base");

                playerCooldownsArray[i].GetComponent<Animator>().Play("CooldownBase", -1);
                playerCooldownsArray[i].GetComponent<Animator>().enabled = false;

                playerCooldownsArray[i].GetComponent<AnimationObject>().Animation_Complete -= OnCooldownFlashComplete;
            }    
        }
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (playerCooldownsArray[i] != null && activeCooldowns[i] == true) //If the player's cooldown timer is active, update the scale.
            {
                Vector3 newScale = playerCooldownsArray[i].transform.FindChild("Cooldown_TimeDisplay").transform.localScale;
                float timerScaleY = playerArray[i].GetComponent<PlayerScript>().PlayerActive.Cooldown.CurrentTime / playerArray[i].GetComponent<PlayerScript>().PlayerActive.Cooldown.TargetTime;

                newScale.y = (float)System.Math.Round(timerScaleY, 2);

                playerCooldownsArray[i].transform.FindChild("Cooldown_TimeDisplay").transform.localScale = newScale;
            }
        }
    }
}
