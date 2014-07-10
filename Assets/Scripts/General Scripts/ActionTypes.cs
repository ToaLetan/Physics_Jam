using UnityEngine;
using System.Collections;

public static class ActionTypes //All actions players can perform. Everyone starts with Basic, can gain others through pick-ups.
{
    private const float THROWVELOCITY = 250.0f;

    public static void Throw_Basic(BeamScript playerBeam) //Launches held object directly where the player is aiming.
    {
        if (playerBeam.CurrentObjectHeld != null)
        {
            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().AddForce(playerBeam.CurrentObjectHeld.transform.right * THROWVELOCITY);
            
            playerBeam.CurrentObjectHeld.GetComponent<BoxCollider2D>().enabled = true;
            
            playerBeam.CurrentObjectHeld.transform.parent = null;
            playerBeam.ReleaseObject();
        }
    }

    public static void Throw_Spread(BeamScript playerBeam) //Creates 2 additional copies of held object and launches them in a spread where the player is aiming.
    {
        if (playerBeam.CurrentObjectHeld != null)
        {
            GameObject[] clonedObjs = new GameObject[2];
            float angleModifier = 1;
            float spreadAngle = 30;

            for(int i = 0; i < clonedObjs.Length; i++)
            {
                clonedObjs[i] = GameObject.Instantiate(playerBeam.CurrentObjectHeld, 
                                                       playerBeam.CurrentObjectHeld.transform.position, 
                                                       playerBeam.CurrentObjectHeld.transform.rotation) as GameObject;

                //Rotate the cloned object to create the spread trajectory.
                if(i == 0)
                    angleModifier = -1;
                else
                    angleModifier = 1;

                float newAngle = clonedObjs[i].transform.rotation.eulerAngles.z + (spreadAngle * angleModifier);
                Quaternion newRotation = Quaternion.AngleAxis (newAngle, Vector3.forward);
                clonedObjs[i].transform.rotation = newRotation;

                clonedObjs[i].transform.localScale = new Vector2(1, 1);

                //Launch the object
                clonedObjs[i].GetComponent<Rigidbody2D>().AddForce(clonedObjs[i].transform.right * THROWVELOCITY);
                clonedObjs[i].GetComponent<BoxCollider2D>().enabled = true;
            }

            //Launch the initial object
            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().AddForce(playerBeam.CurrentObjectHeld.transform.right * THROWVELOCITY);
            playerBeam.CurrentObjectHeld.GetComponent<BoxCollider2D>().enabled = true;
            
            playerBeam.CurrentObjectHeld.transform.parent = null;

            playerBeam.ReleaseObject();
        }
    }

    public static void Throw_Boomerang(BeamScript playerBeam) //Launches object along a boomerang trajectory.
    {
        if (playerBeam.CurrentObjectHeld != null)
        {
            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().AddForce(playerBeam.CurrentObjectHeld.transform.right * THROWVELOCITY/2);

            //Create the boomerang effect by rotating around a point.
            playerBeam.CurrentObjectHeld.AddComponent<ProjectileAttributeScript>();
            playerBeam.CurrentObjectHeld.GetComponent<ProjectileAttributeScript>().CurrentProjectileType = ProjectileAttributeScript.ProjectileType.Boomerang;
            playerBeam.CurrentObjectHeld.GetComponent<ProjectileAttributeScript>().TargetPoint = playerBeam.transform.position;
            
            playerBeam.CurrentObjectHeld.GetComponent<BoxCollider2D>().enabled = true;
            
            playerBeam.CurrentObjectHeld.transform.parent = null;
            playerBeam.ReleaseObject();
        }
    }

    public static void Throw_Enlarge(BeamScript playerBeam) //Increases object's size and mass at a cost of reduced throw distance.
    {
        if (playerBeam.CurrentObjectHeld != null)
        {
            //Increase the object's mass and size.
            playerBeam.CurrentObjectHeld.transform.localScale *= 2;
            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().mass *= 2;

            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().AddForce(playerBeam.CurrentObjectHeld.transform.right * THROWVELOCITY);
            
            playerBeam.CurrentObjectHeld.GetComponent<BoxCollider2D>().enabled = true;
            
            playerBeam.CurrentObjectHeld.transform.parent = null;
            playerBeam.ReleaseObject();
        }
    }

    public static void Throw_Homing(BeamScript playerBeam) //Homes in on the opposing player with a slight offset for balancing.
    {
        if (playerBeam.CurrentObjectHeld != null)
        {
            playerBeam.CurrentObjectHeld.GetComponent<Rigidbody2D>().AddForce(playerBeam.CurrentObjectHeld.transform.right * THROWVELOCITY/2);
            
            //Get the opponent's location and home in on them with an offset.
            Vector3 opponentLocation = Vector3.zero;



            playerBeam.CurrentObjectHeld.AddComponent<ProjectileAttributeScript>();
            playerBeam.CurrentObjectHeld.GetComponent<ProjectileAttributeScript>().CurrentProjectileType = ProjectileAttributeScript.ProjectileType.Boomerang;
            playerBeam.CurrentObjectHeld.GetComponent<ProjectileAttributeScript>().TargetPoint = playerBeam.transform.position;
            
            playerBeam.CurrentObjectHeld.GetComponent<BoxCollider2D>().enabled = true;
            
            playerBeam.CurrentObjectHeld.transform.parent = null;
            playerBeam.ReleaseObject();
        }
    }
}
