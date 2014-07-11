using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour 
{
    public enum PickupType
    { Spread, Boomerang, Enlarge, Homing }

    private PickupType currentPickupType;

    public PickupType CurrentPickupType
    {
        get { return currentPickupType; }
        set { currentPickupType = value; }
    }

	// Use this for initialization
	void Start () 
    {
        GeneratePickup();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void GeneratePickup()
    {
        int randIndex = Random.Range(0, 4);

        switch (randIndex)
        {
            case 0:
                currentPickupType = PickupType.Spread;
                break;
            case 1:
                currentPickupType = PickupType.Boomerang;
                break;
            case 2:
                currentPickupType = PickupType.Enlarge;
                break;
            case 3:
                currentPickupType = PickupType.Homing;
                break;
        }
        string pickupName = "Pickup_" + currentPickupType.ToString();

        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Pickups/" + pickupName);
    }
}
