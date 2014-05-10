using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		InputManager bleh = InputManager.Instance;
		//assignedController = new XboxController(0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		InputManager.Instance.Update ();

		//float thumbstickX = InputManager.Instance.ControllerArray[0].GetAxis("Horizontal");
		// thumbstickY = InputManager.Instance.ControllerArray[0].GetAxis("Vertical");

		//float angle = Mathf.Rad2Deg * Mathf.Atan2 (thumbstickX, thumbstickY);
		//Vector3 newRotation = new Vector3 (gameObject.transform.rotation.x, gameObject.transform.rotation.y, angle);


		//gameObject.transform.rotation = Quaternion.Lerp (gameObject.transform.rotation, Quaternion.Euler(newRotation), Time.deltaTime);
	}
}
