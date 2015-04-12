using UnityEngine;
using System.Collections;

public class GlobPuddleScript : MonoBehaviour 
{
    public enum PuddleType { SpeedUp, SpeedDown, Decoration, GravField }

    public PuddleType PuddleClassification;

    private AnimationObject animScript = null;

    private Active abilityInfo = null;

    private bool hasSpawnedObj = false;

    public Active AbilityInfo
    {
        get { return abilityInfo; }
        set { abilityInfo = value; }
    }

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
                    GameObject slipGel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/OilSlick"), gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                    slipGel.GetComponent<SpeedZoneScript>().TieAbilityInfo(abilityInfo);
                    break;
                case PuddleType.SpeedDown:
                    GameObject slowGel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/SlowPuddle"), gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                    slowGel.GetComponent<SpeedZoneScript>().TieAbilityInfo(abilityInfo);
                    break;
                case PuddleType.Decoration:
                    break;
                case PuddleType.GravField:
                    GameObject gravField = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/GravField"), gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                    gravField.transform.FindChild("GravField_Anim").GetComponent<GravFieldScript>().TieAbilityInfo(abilityInfo);
                    break;
            }
            
        }
        
        animScript.Animation_Complete -= OnAnimationComplete;
    }
}
