using UnityEngine;
using System.Collections;

public class AnimationObject : MonoBehaviour 
{
    public enum ResolutionType { Stop, Destroy, Loop, FireEvent_Stop, FireEvent_Destroy };
    public ResolutionType currentResolutionType = ResolutionType.Stop;

    public delegate void ResolvedAnimationEvent();
    public event ResolvedAnimationEvent Animation_Complete;

	// Use this for initialization
	void Start () 
    {

	}
	
	// Update is called once per frame
	void Update () 
    {

	}

    void OnAnimationComplete() //Do something based on the selected resolution to the animation.
    {
        switch (currentResolutionType)
        {
            case ResolutionType.Stop: //Nothing happens, object persists.
                gameObject.GetComponent<Animator>().enabled = false;
                break;
            case ResolutionType.Destroy: //Delete the object.
                Destroy(gameObject);
                break;
            case ResolutionType.Loop: //Let the animation repeat
                break;
            case ResolutionType.FireEvent_Stop: //Fire an event to be handled by specialized functions of other objects.
                if (Animation_Complete != null)
                    Animation_Complete();
                gameObject.GetComponent<Animator>().enabled = false;
                break;
            case ResolutionType.FireEvent_Destroy: //Fire an event to be handled by specialized functions of other objects, then destroy the animation object.
                if (Animation_Complete != null)
                    Animation_Complete();
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
