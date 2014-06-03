using UnityEngine;
using System.Collections;

public class XboxController
{	
	public string buttonA = "Fire1";
	public string buttonB = "Fire2";
	public string buttonX = "Fire3";
	public string buttonY = "Jump";

	private int id;

	public int Controller_ID
	{
		get { return id; }
	}

	// Use this for initialization
	public XboxController(int controllerID) 
	{
		id = controllerID;
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	//Input functions
	public bool GetButtonDown(string buttonName)
	{
		return Input.GetButtonDown(buttonName);
	}

	public bool GetButtonUp(string buttonName)
	{
		return Input.GetButtonUp(buttonName);
	}

	public float GetAxis(string axisName)
	{
		return Input.GetAxis(axisName);
	}
	
}
