using UnityEngine;


/// <summary>
/// TurretBulletSounder is class for playing bullet of turret action sounds
/// </summary>
public class TurretBulletSounder: MonoBehaviour
{
    public AudioClip bulletExplosion;                                                               // sound explosion

    public BulletHandler bulletHnd;                                                                 // bullet handler

    void Start()
    {
        if (bulletHnd)
        {
            if (bulletExplosion)
                bulletHnd.OnDestructed += () => 
                          AudioSource.PlayClipAtPoint(bulletExplosion, transform.position);        // add playing of sound in callback
        }
    }
}
