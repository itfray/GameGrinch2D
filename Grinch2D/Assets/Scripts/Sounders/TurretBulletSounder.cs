using UnityEngine;


/// <summary>
/// TurretBulletSounder is class for playing bullet of turret action sounds
/// </summary>
public class TurretBulletSounder: Sounder
{
    public AudioSource bulletExplosion;                                                             // sound explosion

    public BulletHandler bulletHnd;                                                                 // bullet handler

    protected override void Init()
    {
        if (bulletHnd)
        {
            if (bulletExplosion)
            {
                bulletHnd.OnDestructed += () => bulletExplosion.Play();                             // add playing of sound in callback
                audios.Add(bulletExplosion);
            }
        }

        base.Init();
    }
}
