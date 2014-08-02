using UnityEngine;
using System.Collections;

public class IgnoreCameraScalingScript : MonoBehaviour 
{
    private Camera theCamera = null;

    private Vector3 initialScale = Vector3.zero;

    private float prevOrthoSize = 0.0f;

    private float initialCameraSize = 0.0f;

	// Use this for initialization
	void Start () 
    {
        theCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        prevOrthoSize = theCamera.orthographicSize;

        initialScale = gameObject.transform.localScale;
        initialCameraSize = theCamera.orthographicSize;

        NegateScaling();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (theCamera.orthographicSize != prevOrthoSize)
        {
            NegateScaling();
            prevOrthoSize = theCamera.orthographicSize;
        }
	}

    private void NegateScaling()
    {
        //OLD CODE WORKS IF INITIALSCALE = 1
        //gameObject.transform.localScale = new Vector3(theCamera.orthographicSize/initialScale.x, theCamera.orthographicSize/initialScale.y, 1);

        //THE FOLLOWING FORMULA FAVOURS INITIAL SIZES OF 1+
        //float newScale = theCamera.orthographicSize * initialScale.x;

        float newScale = 0.0f;

        if(initialScale == Vector3.one)
            newScale = theCamera.orthographicSize;
        else
            newScale = theCamera.orthographicSize * (initialScale.x / initialCameraSize);

        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
    }
}
