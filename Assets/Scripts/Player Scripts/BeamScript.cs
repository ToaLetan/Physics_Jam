﻿using UnityEngine;
using System.Collections;

public class BeamScript : MonoBehaviour 
{
	private GameObject currentObjectSelected = null;
    private GameObject currentObjectHeld = null;
	private bool isHoldingObject = false;

	public GameObject CurrentObjectSelected
	{
		get { return currentObjectSelected; }
	}

    public GameObject CurrentObjectHeld
    {
        get { return currentObjectHeld; }
    }

    public bool IsHoldingObject
    {
        get { return isHoldingObject; }
    }

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerEnter2D(Collider2D collisionObj)
	{
		switch (collisionObj.gameObject.tag) 
		{
			case "PhysicsObj":
				if(!isHoldingObject)
				{
					currentObjectSelected = collisionObj.gameObject;
				}
				break;
		}
	}

	void OnTriggerExit2D(Collider2D collisionObj)
	{
		if (currentObjectSelected != null) 
		{
			if(collisionObj.gameObject == currentObjectSelected)
				currentObjectSelected = null;
		}
	}

    public void GrabObject()
    {
        if (currentObjectSelected != null)
        {
            currentObjectSelected.GetComponent<PhysicsObjectScript>().IsFalling = false;

            isHoldingObject = true;
            currentObjectHeld = currentObjectSelected;

            currentObjectSelected.transform.localScale = Vector3.one;

            if (currentObjectHeld.GetComponent<ProjectileAttributeScript>() != null)
                Destroy(currentObjectHeld.GetComponent<ProjectileAttributeScript>());
        }
    }

    public void ReleaseObject(float scaleModifier = 1.0f)
    {
        isHoldingObject = false;
        currentObjectHeld.transform.localScale = Vector3.one * scaleModifier;
        currentObjectHeld = null;
    }
}
