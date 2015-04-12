using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraEffectsScript : MonoBehaviour 
{
    private const float MINZOOMLEVEL = 0.96f;

    private List<GameObject> playerList = new List<GameObject>();

	private float moveSpeed = 20.0f;
    private float zoomSpeed = 20.0f;

	// Use this for initialization
	void Start () 
	{
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
            playerList.Add(GameObject.FindGameObjectsWithTag("Player") [i]);
	}
	
	// Update is called once per frame
	void Update () 
	{
        if(playerList.Count > 0)
            KeepPlayersInView();
	}

	public void RotateToAngle(float angleInDegrees, float rotateSpeed) //Rotate camera towards a specified angle on the Z-axis
	{
		Quaternion newRotation = Quaternion.AngleAxis (angleInDegrees, Vector3.forward);

		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
	}

	public void ZoomLevel(float zoomScale, float zoomSpeed) //Zoom in/out (positive
	{
		Camera theCamera = gameObject.GetComponent<Camera>();

        if (theCamera.orthographicSize != zoomScale)
        {
            theCamera.orthographicSize += (zoomScale - theCamera.orthographicSize) * zoomSpeed * Time.deltaTime;
            //theCamera.orthographicSize = Mathf.Round(theCamera.orthographicSize * 100.0f)/100.0f; //Caused jitter, probably leaving this out.
        }
	}

    public void MoveToLocation(Vector2 location, float moveSpeed) //Gradually translate the camera
    {
        Vector3 location3D = new Vector3(location.x, location.y, gameObject.transform.position.z);

        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, location3D, moveSpeed * Time.deltaTime);
    }

    private void KeepPlayersInView()
    {
        float zoomModifier = 0.65f; //Arbitrary value used to reduce zoom slightly.
        float furthestDistance = 0.0f; //The furthest distance between any two players used to determine how far the camera should zoom in/out.
        float midPointX = 0.0f; //Midpoints used to position camera in the center.
        float midPointY = 0.0f;

        //Get the largest distances between X and Y values.
        for (int i = 0; i < playerList.Count; i++)
        {
            //Compare the current player to all others, tracking the furthest distance of X and Y values.
            for (int j = 0; j < playerList.Count; j++)
            {
                if (playerList[i] != playerList[j])
                {
                    float currentDistance = SpeculativeContactsScript.GetDistance(playerList[i].transform.position, playerList[j].transform.position);

                    if (currentDistance > furthestDistance)
                        furthestDistance = currentDistance;
                }
            }
            midPointX += playerList[i].transform.position.x;
            midPointY += playerList[i].transform.position.y;
        }

        furthestDistance *= zoomModifier; //Multiply the furthest distance by the zoom reduction modifier.

        if (furthestDistance < MINZOOMLEVEL)
            furthestDistance = MINZOOMLEVEL;

        Vector2 playerMidPoint = new Vector2(midPointX/playerList.Count, midPointY/playerList.Count);

        ZoomLevel(furthestDistance, zoomSpeed);
        MoveToLocation(playerMidPoint, moveSpeed);
    }
}
