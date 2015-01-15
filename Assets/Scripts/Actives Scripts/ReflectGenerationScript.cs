using UnityEngine;
using System.Collections;

public class ReflectGenerationScript : MonoBehaviour 
{
    private AnimationObject animScript = null;
    private bool hasSpawnedObj = false;

	// Use this for initialization
	void Start () 
    {
        animScript = gameObject.GetComponent<AnimationObject>();
        animScript.Animation_Complete += OnAnimationComplete;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void OnAnimationComplete()
    {
        if (hasSpawnedObj == false)
        {
            hasSpawnedObj = true;

            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Reflect"), gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
