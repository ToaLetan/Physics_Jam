using UnityEngine;
using System.Collections;

public class AnimatePreviewScript : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        gameObject.GetComponent<Animator>().Play("Player_Fall");
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
