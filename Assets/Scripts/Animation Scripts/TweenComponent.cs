using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenComponent : MonoBehaviour 
{
    public enum TweenType { Position, Colour }

    public delegate void TweenEvent();
    public event TweenEvent TweenComplete;

    private TweenType currentTweenType = TweenType.Position;

    private SpriteRenderer spriteRenderer = null;

    private GameManager gameManager = null;

    public Color targetColour = Color.white;

    private Vector3 targetPosition = Vector3.zero;

    private float tweenSpeed = 0.0f;

    private bool isTweening = false;
    private bool affectedByPause = false;

    public TweenType CurrentTweenType
    {
        get { return currentTweenType; }
        set { currentTweenType = value; }
    }

    public GameManager GameManager
    {
        get { return gameManager; }
        set { gameManager = value; }
    }

    public Color TargetColour
    {
        get { return targetColour; }
        set { targetColour = value; }
    }

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set { targetPosition = value; }
    }

    public float TweenSpeed
    {
        get { return tweenSpeed; }
        set { tweenSpeed = value; }
    }

    public bool IsTweening
    {
        get { return isTweening; }
        set { isTweening = value; }
    }

    public bool AffectedByPause
    {
        get { return affectedByPause; }
        set { affectedByPause = value; }
    }

	// Use this for initialization
	void Start () 
    {
        //targetPosition = gameObject.transform.position;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (affectedByPause == true)
        {
            if (gameManager != null && !gameManager.IsGamePaused)
                UpdateTween();
        }
        else
            UpdateTween(); 
	}

    private void UpdateTween()
    {
        if (isTweening == true)
        {
            switch (currentTweenType)
            {
                case TweenType.Position:
                    if (gameObject.transform.position != targetPosition)
                    {
                        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, tweenSpeed * Time.deltaTime);
                    }
                    else
                    {
                        ResolveTween(true);
                    }
                    break;
                case TweenType.Colour:
                    if (spriteRenderer.color != targetColour)
                    {
                        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColour, tweenSpeed * Time.deltaTime);
                    }
                    else
                    {
                        ResolveTween(false);
                    }
                    break;
            }
        }
    }

    public void TweenPositionTo(Vector3 target, float speed)
    {
        targetPosition = target;
        tweenSpeed = speed;
        isTweening = true;
    }

    public void InitInGameTween() //Called for any Tweens that are used in-game to allow them to be paused.
    {
        affectedByPause = true;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void CancelTween()
    {
        isTweening = false;

        Destroy(this);
    }

    private void ResolveTween(bool destroyComponent)
    {
        IsTweening = false;

        if (TweenComplete != null)
            TweenComplete();

        if(destroyComponent)
            Destroy(this); //Remove the script when finished.
    }
}
