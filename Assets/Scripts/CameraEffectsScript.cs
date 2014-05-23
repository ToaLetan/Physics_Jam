using UnityEngine;
using System.Collections;

public class CameraEffectsScript : MonoBehaviour 
{
	private float moveSpeed = 0.5f;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		RotateToAngle (275, moveSpeed);
		ZoomLevel (0.8f, moveSpeed);
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
			theCamera.orthographicSize += (zoomScale - theCamera.orthographicSize) * Time.deltaTime * zoomSpeed;

		//theCamera.orthographicSize = zoomScale;
	}
}
