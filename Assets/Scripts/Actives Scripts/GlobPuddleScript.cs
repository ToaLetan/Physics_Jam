using UnityEngine;
using System.Collections;

public class GlobPuddleScript : MonoBehaviour 
{
    public enum PuddleType { SpeedUp, SpeedDown, Decoration }

    public PuddleType PuddleClassification;

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

    private void OnAnimationComplete() //Spawn the puddle object that actually interacts with gameplay.
    {
        if (hasSpawnedObj == false) //Prevent multiple objects from spawning (weird issue that was encountered earlier).
        {
            hasSpawnedObj = true;

            switch (PuddleClassification)
            {
                case PuddleType.SpeedUp:
                    GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/OilSlick"), gameObject.transform.position, gameObject.transform.rotation);
                    break;
                case PuddleType.SpeedDown:
                    GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/SlowPuddle"), gameObject.transform.position, gameObject.transform.rotation);
                    break;
                case PuddleType.Decoration:
                    break;
            }
            
        }
        
        animScript.Animation_Complete -= OnAnimationComplete;
    }
}
