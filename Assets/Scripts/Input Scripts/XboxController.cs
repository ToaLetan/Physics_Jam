﻿using UnityEngine;
using System.Collections;

public class XboxController
{
	public string buttonA = "Button A";
	public string buttonB = "Button B";
	public string buttonX = "Button X";
	public string buttonY = "Button Y";

    public string leftThumbstickHorizontal = "Left Stick Horizontal";
    public string leftThumbstickVertical = "Left Stick Vertical";

    private string controllerIdentifier;

	private int id;

	public int Controller_ID
	{
		get { return id; }
	}

	// Use this for initialization
	public XboxController(int controllerID) 
	{
		id = controllerID;
        controllerIdentifier = "Controller" + (id+1) + " ";
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	//Input functions
    public bool GetButtonHeld(string buttonName)
    {
        return Input.GetButton(controllerIdentifier + buttonName);
    }

	public bool GetButtonDown(string buttonName)
	{
        return Input.GetButtonDown(controllerIdentifier + buttonName);
	}

	public bool GetButtonUp(string buttonName)
	{
        return Input.GetButtonUp(controllerIdentifier + buttonName);
	}

	public float GetThumbstickAxis(string axisName)
	{
        return Input.GetAxis(controllerIdentifier + axisName);
	}
	
}
