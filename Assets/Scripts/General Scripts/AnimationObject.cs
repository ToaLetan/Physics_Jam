using UnityEngine;
using System.Collections;

public class AnimationObject : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    void OnAnimationComplete()
    {
        Destroy(gameObject);
    }
}
