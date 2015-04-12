using UnityEngine;
using System.Collections;

public class SpeedZoneScript : MonoBehaviour 
{
    public float SpeedModifier = 2.0f;

    private GameManager gameManager = null;

    private Active abilityInfo = null;

    public Active AbilityInfo
    {
        get { return abilityInfo; }
        set { abilityInfo = value; }
    }

	// Use this for initialization
	void Start () 
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        /*if (gameManager.IsGamePaused == false)
        {

        }*/
	}

    public void TieAbilityInfo(Active ability)
    {
        abilityInfo = ability;
        abilityInfo.Duration.OnTimerComplete += Despawn;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
