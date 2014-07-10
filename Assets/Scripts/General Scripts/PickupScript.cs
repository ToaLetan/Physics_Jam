using UnityEngine;
using System.Collections;

public class PickupScript : MonoBehaviour 
{
    public enum PickupType
    { Spread, Boomerang, Enlarge, Homing }

    private PickupType pickupType;

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
                pickupType = PickupType.Spread;

                break;
            case 1:
                pickupType = PickupType.Boomerang;
                break;
            case 2:
                pickupType = PickupType.Enlarge;
                break;
            case 3:
                pickupType = PickupType.Homing;
                break;
        }
        Debug.Log(pickupType.ToString() );
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load("Sprites/Pickups/Pickup_" + pickupType.ToString() ) as Sprite;
    }
}
