using UnityEngine;


/// <summary>
/// TurretBulletSoundPlayer is class for playing bullet of turret action sounds
/// </summary>
public class TurretBulletSoundPlayer: SoundPlayer
{
    public AudioClip bulletExplosion;                                                               // sound explosion

    public BulletHandler bulletHnd;                                                                 // bullet handler

    void Start()
    {
        if (bulletHnd)
        {
            if (bulletExplosion)
                bulletHnd.OnDestructed += () => PlaySound(bulletExplosion, transform.position);     // add playing of sound in callback
        }
    }
}
