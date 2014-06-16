using UnityEngine;
using System.Collections;

public static class AnimationPlayer
{
    public static void PlayAnimation(GameObject owner, string animName)
    {

    }

    public static void ChangeSprite(GameObject owner, string spritePath)
    {
        if (owner.transform.GetComponent<SpriteRenderer>() != null)
        {
            Debug.Log("ye");
            owner.transform.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(spritePath);
        }
    }
}
