using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		SlowMoManager.Instance.GetAllPhysicsObjects();
		//SlowMoManager.Instance.SlowMoTime (0.25f, 2.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Update any management systems
		InputManager.Instance.Update();
		SlowMoManager.Instance.Update();
        UIManager.Instance.Update();
	}

}
