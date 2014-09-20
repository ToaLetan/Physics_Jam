using UnityEngine;
using System.Collections;

public class Active //The base Active class, features cooldown timer and duration.
{
    public enum ActiveType { None, GravityField, Reflect, Slipstream, Soak, Overclock, Slowstream }

    private Timer cooldown;
    private Timer duration;

    public Timer Cooldown
    {
        get { return cooldown; }
    }

    public Timer Duration
    {
        get { return duration; }
    }

    public Active(float cooldownTime, float durationTime)
    {
        cooldown = new Timer(cooldownTime);
        duration = new Timer(durationTime);
    }

    public void Update()
    {
        if (cooldown.IsTimerRunning == true)
            cooldown.Update();
        if (duration.IsTimerRunning == true)
            duration.Update();
    }

    public void UseActive()
    {
        if (cooldown.IsTimerRunning == false && duration.IsTimerRunning == false)
        {
            duration.StartTimer();
        }
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

        //Instantiate the Slipstream object here.

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
