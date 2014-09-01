using UnityEngine;
using System.Collections;

public class TweenComponent : MonoBehaviour 
{
    public delegate void TweenEvent();
    public event TweenEvent TweenComplete;

    public Vector3 targetPosition = Vector3.zero;
    public float moveSpeed = 0.0f;
    public bool isTweening = false;

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public bool IsTweening
    {
        get { return isTweening; }
        set { isTweening = value; }
    }

	// Use this for initialization
	void Start () 
    {
        //targetPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isTweening == true)
        {
            if (gameObject.transform.position != targetPosition)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                if (TweenComplete != null)
                    TweenComplete();
                IsTweening = false;
                Destroy(this); //Remove the script when finished.
            }
        }
	}

    public void TweenPositionTo(Vector3 target, float speed)
    {
        targetPosition = target;
        moveSpeed = speed;
        isTweening = true;
    }
}
