using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraEffectsScript : MonoBehaviour 
{
    private const float MINZOOMLEVEL = 1.0f;

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
            theCamera.orthographicSize += (zoomScale - theCamera.orthographicSize) * zoomSpeed * Time.deltaTime;
	}

    public void MoveToLocation(Vector2 location, float moveSpeed) //Gradually translate the camera
    {
        Vector3 location3D = new Vector3(location.x, location.y, gameObject.transform.position.z);

        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, location3D, moveSpeed * Time.deltaTime);
    }

    private void KeepPlayersInView()
    {
        float combinedDist = SpeculativeContactsScript.GetDistance(playerList [0].transform.position, playerList [1].transform.position)/2;

        if (combinedDist < MINZOOMLEVEL)
            combinedDist = MINZOOMLEVEL;

        Vector2 playerMidPoint = new Vector2( (playerList [0].transform.position.x + playerList [1].transform.position.x)/2, 
                                             (playerList [0].transform.position.y + playerList [1].transform.position.y)/2 );

        ZoomLevel(combinedDist * 1.1f, zoomSpeed);
        MoveToLocation(playerMidPoint, moveSpeed);
    }
}
