using UnityEngine;
using System.Collections;

public static class AnimationPlayer
{
    public static void PlayAnimation(GameObject owner, string animationName)
    {
        if (owner.transform.GetComponent<Animator>() != null)
        {
            owner.transform.GetComponent<Animator>().Play(animationName);
        }
    }

    public static void ChangeSprite(GameObject owner, string spritePath)
    {
        if (owner.transform.GetComponent<SpriteRenderer>() != null)
        {
            owner.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        }
    }
}
