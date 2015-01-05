using UnityEngine;
using System.Collections;

public class Active //The base Active class, features cooldown timer and duration.
{
    public enum ActiveType { None, GravityField, Reflect, Slipstream, Soak, Overclock, Slowstream }

    private const float SHOT_DELAY_TIME = 0.25f;

    private PlayerScript owner = null;

    private Timer cooldown;
    private Timer duration;
    private Timer shotDelay; //Used to space out projectile-type Actives (Speed Up, Speed Down)

    private int currentProjectileNum = 0;
    private int totalProjectiles = 0;

    public Timer Cooldown
    {
        get { return cooldown; }
    }

    public Timer Duration
    {
        get { return duration; }
    }

    public Timer ShotDelay
    {
        get { return shotDelay; }
    }

    public Active(float cooldownTime, float durationTime)
    {
        cooldown = new Timer(cooldownTime);
        duration = new Timer(durationTime);
        shotDelay = new Timer(SHOT_DELAY_TIME);
    }

    public void Update()
    {
        if (cooldown.IsTimerRunning == true)
            cooldown.Update();
        if (duration.IsTimerRunning == true)
            duration.Update();
        if (shotDelay.IsTimerRunning == true)
            shotDelay.Update();
    }

    public void UseActive()
    {
        if (cooldown.IsTimerRunning == false && duration.IsTimerRunning == false)
        {
            duration.StartTimer();
        }
    }

    public void PrepareProjectiles(int numProjectiles, PlayerScript playerUsingActive)
    {
        owner = playerUsingActive;
        totalProjectiles = numProjectiles;

        //Fire the first projectile and start the delay timer.
        //ShootProjectile();

        shotDelay.StartTimer();
        shotDelay.OnTimerComplete += ShootProjectile;
    }

    private void ShootProjectile()
    {
        if (currentProjectileNum < totalProjectiles)
        {
            currentProjectileNum++;

            //Instantiate the Slipstream object here.
            float projectileDistanceModifier = 0.25f;
            float minimumDistance = 1.0f;

            //Space the objects out.
            Vector3 objectPos = owner.gameObject.transform.position + (owner.PlayerBeam.transform.right * ((currentProjectileNum * projectileDistanceModifier) + minimumDistance));

            objectPos.z = owner.transform.position.z;

            GameObject newProjectile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_Projectile"), owner.PlayerBeam.transform.position, owner.PlayerBeam.transform.rotation) as GameObject;

            newProjectile.GetComponent<ActiveProjectileScript>().Destination = objectPos;

            shotDelay.ResetTimer(true);
        }
        else
            shotDelay.OnTimerComplete -= ShootProjectile;
        
    }
}

public static class ActivesTypes //All Actives players can start with. Players select 1 Active after choosing their colour, and keep this Active for the entire duration of the game.
                            //All Actives have a cooldown timer between uses and a duration timer for how long the Active lasts for.
                            //Actives are not meant to be an offensive option, these are meant for map control and buffs/de-buffs.
{
    public static Active GravityField(PlayerScript owner) //Create a zone at the end of the player's beam, zone pulls in objs.
    {
        Active returnActive = new Active(20.0f, 5.0f);

        //Instantiate a Gravity Field Zone object here and UseActive().

        return returnActive;
    }

    public static Active Reflect(PlayerScript owner) //Immediately reflect connecting projectile back along its trajectory.
    {
        Active returnActive = new Active(20.0f, 1.0f);

        //Get the connecting object and send it back here, call UseActive();
        //Instantiate and play the anim on the player.

        return returnActive;
    }

    public static Active Slipstream(PlayerScript owner) //Create a rectangular strip oriented with the player's beam that speeds up any objects that cross it.
    {
        Active returnActive = new Active(20.0f, 5.0f);

        returnActive.PrepareProjectiles(3, owner);

        /*
        for (int i = 0; i < 1; i++)
        {
            //Instantiate the Slipstream object here.
            float projectileDistanceModifier = 0.25f;
            float minimumDistance = 1.0f;

            //Space the objects out.
            Vector3 objectPos = owner.gameObject.transform.position + (owner.PlayerBeam.transform.right * ((i * projectileDistanceModifier) + minimumDistance));

            objectPos.z = owner.transform.position.z;

            GameObject newProjectile = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Actives/Glob_Projectile"), owner.PlayerBeam.transform.position, owner.PlayerBeam.transform.rotation) as GameObject;

            newProjectile.GetComponent<ActiveProjectileScript>().Destination = objectPos;
        }*/

        return returnActive;
    }

    public static Active Soak(PlayerScript owner) //Fire a water projectile where the player is aiming, slows down the player it collides with.
    {
        Active returnActive = new Active(20.0f, 5.0f);

        //Instantiate the projectile here.

        return returnActive;
    }

    public static Active Overclock(PlayerScript owner) //Self-buff that increases player's speed.
    {
        Active returnActive = new Active(20.0f, 5.0f);

        //Buff the player here and play anim.

        return returnActive;
    }

    public static Active Slowstream(PlayerScript owner) //Create a rectangular strip along the player's beam that slows down objects that cross it.
    {
        Active returnActive = new Active(20.0f, 5.0f);

        //Instantiate the Slowstream object here.

        return returnActive;
    }
}
