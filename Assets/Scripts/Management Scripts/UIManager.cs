using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager
{
    private GameObject[] playerArray = new GameObject[4];
    private GameObject[] playerNamesArray = new GameObject[4];
    private List<GameObject> playerLivesList = new List<GameObject>();

    private Camera camera;

    private GameObject combinedUI;
    private GameObject winnerText;
    private GameObject endPromptText;

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
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform.GetComponent<Camera>();

        combinedUI = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CombinedUIHolder"), camera.gameObject.transform.position, camera.gameObject.transform.rotation) as GameObject;
        combinedUI.transform.parent = camera.gameObject.transform;

        winnerText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/WinText") ) as GameObject;
        winnerText.transform.parent = combinedUI.transform;

        endPromptText = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/EndingPrompt") ) as GameObject;
        endPromptText.transform.parent = combinedUI.transform;

        GameObject[] activePlayers = GameObject.FindGameObjectsWithTag("Player");

        //Arrange the player array to match the player numbers.
        //THIS IS REALLY GHETTO. FIX IT LATER WHEN NOT TIRED.
        for (int i = 0; i < activePlayers.Length; i++)
        {
            if(activePlayers[i].transform.GetComponent<PlayerScript>().PlayerNumber == 0)
                playerArray[0] = activePlayers[i];
            if(activePlayers[i].transform.GetComponent<PlayerScript>().PlayerNumber == 1)
                playerArray[1] = activePlayers[i];
            if(activePlayers[i].transform.GetComponent<PlayerScript>().PlayerNumber == 2)
                playerArray[2] = activePlayers[i];
            if(activePlayers[i].transform.GetComponent<PlayerScript>().PlayerNumber == 3)
                playerArray[3] = activePlayers[i];
        }

        ConstructHUD();
        HideEnding();
	}
	
	// Update is called once per frame
	public void Update () 
    {
        if (camera != null)
        {
            //Testing UI repositioning.
            combinedUI.transform.localScale = camera.gameObject.transform.localScale * 0.75f;
            playerNamesArray [0].transform.localPosition = new Vector3(-camera.orthographicSize, camera.orthographicSize, 1);
            camera.WorldToViewportPoint(playerNamesArray [0].transform.localPosition);

            playerNamesArray [1].transform.localPosition = new Vector3(camera.orthographicSize, camera.orthographicSize, 1);
            camera.WorldToViewportPoint(playerNamesArray [1].transform.localPosition);
        }
	}

    public void ConstructHUD() //Instantiate the HUD, consists of 2-4 Player sectors + info/score/etc., game time remaining.
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            if(playerArray[i] != null)
            {
                playerNamesArray[i] = GameObject.Instantiate(Resources.Load("Prefabs/GUI/Text_Player" + (i+1) ) ) as GameObject;
                playerNamesArray[i].transform.parent = combinedUI.transform;

                PositionHUDElements(playerNamesArray[i].name);

                GenerateLifeIcons(playerArray[i].GetComponent<PlayerScript>().NumLives, i);
            }
        }

        MatchPlayerColours();
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
                playerNamesArray[0].transform.localPosition = new Vector3(-camera.orthographicSize - UIOffset, camera.orthographicSize - UIOffset, 1);
                break;
            case "Text_Player2":
                playerNamesArray[1].transform.localPosition = new Vector3(camera.orthographicSize - UIOffset * 2.6f, camera.orthographicSize - UIOffset, 1);
                break;
            case "Text_Player3":
                playerNamesArray[2].transform.localPosition = new Vector3(-camera.orthographicSize - UIOffset, -camera.orthographicSize + (UIOffset * 2), 1);
                break;
            case "Text_Player4":
                playerNamesArray[3].transform.localPosition = new Vector3(camera.orthographicSize - UIOffset * 2.6f, -camera.orthographicSize + (UIOffset * 2), 1);
                break;
        }
    }

    private void GenerateLifeIcons(int numOfLives, int ownerNumber)
    {
        float UIOffset = 0.1f;

        for (int i = 0; i < numOfLives; i++)
        {
            GameObject newLifeIcon = GameObject.Instantiate(Resources.Load("Prefabs/GUI/Life_Icon") ) as GameObject;

            float lifeIconWidth = newLifeIcon.transform.GetComponent<SpriteRenderer>().bounds.max.x;
            float lifeIconHeight = newLifeIcon.transform.GetComponent<SpriteRenderer>().bounds.max.y;

            newLifeIcon.transform.parent = playerNamesArray[ownerNumber].transform;
            newLifeIcon.transform.localPosition = new Vector3(-UIOffset * 2 + (UIOffset * i * 2), -UIOffset * 1.5f, 0);

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
}
