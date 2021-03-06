﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SpeculativeContactsScript
{
    public static GameObject PerformSpeculativeContacts(Vector3 startPos, Vector2 vecOrientation, float rayLength = 0.0f) //Prevents players from nudging walls/objects continuously.
    {
        //bool collisionResult = false;
        GameObject imminentCollisionObj = null;
        
        int layerMask = 1 << 2; //bitshifting the index of layer to get a bit mask
        layerMask = ~layerMask; //inverting the bitmask
        
        RaycastHit2D raycast = Physics2D.Raycast(startPos, vecOrientation, rayLength, layerMask);
        
        if(raycast)
        {
            //collisionResult = true;
            if(raycast.collider.gameObject.tag != "KillBox" && 
               raycast.collider.gameObject.tag != "SpawnZone" &&
               raycast.collider.gameObject.tag != "Pickup" &&
                raycast.collider.gameObject.tag != "ActiveZone")
            {
                imminentCollisionObj = raycast.collider.gameObject;
                Debug.DrawRay(startPos, vecOrientation);
            }
        }
        
        return imminentCollisionObj;
    }
    
    public static float GetDistance(float point1, float point2)
    {
        return Mathf.Sqrt( (point1 - point2) * (point1 - point2) );
    }
    
    public static float GetDistance(Vector2 point1, Vector2 point2)
    {
        return Mathf.Sqrt( (point1.x - point2.x) * (point1.x - point2.x) + (point1.y - point2.y) * (point1.y - point2.y) );
    }
}
