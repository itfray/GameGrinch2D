using UnityEngine;


/// <summary>
/// TurretSounder is class for playing turret action sounds
/// </summary>
public class TurretSounder : MonoBehaviour
{
    public AudioClip shootSound;                                                                // shoo sound

    public TurretWeaponHnd weaponHnd;                                                           // weapon handler of turret

    void Start()
    {
        if (weaponHnd)
        {
            if (shootSound)
                weaponHnd.OnAttacked += () => 
                          AudioSource.PlayClipAtPoint(shootSound, transform.position);         // add playing of sound in callback
        }
    }
}
