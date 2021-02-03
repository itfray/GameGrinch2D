using UnityEngine;


/// <summary>
/// TurretBulletSounder is class for playing bullet of turret action sounds
/// </summary>
public class TurretBulletSounder: MonoBehaviour
{
    public AudioSource bulletExplosion;                                                             // sound explosion

    public BulletHandler bulletHnd;                                                                 // bullet handler

    void Start()
    {
        if (bulletHnd)
        {
            if (bulletExplosion)
            {
                bulletExplosion.transform.parent = null;
                bulletHnd.OnDestructed += () => bulletExplosion.Play();                             // add playing of sound in callback
            }
        }
    }

    void OnDestroy()
    {
        if (bulletExplosion) 
            Destroy(bulletExplosion.gameObject);
    }
}
