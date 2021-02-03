using UnityEngine;


/// <summary>
/// TurretSounder is class for playing turret action sounds
/// </summary>
public class TurretSounder : MonoBehaviour
{
    public AudioSource shootSound;                                                                // shoo sound

    public TurretWeaponHnd weaponHnd;                                                           // weapon handler of turret

    void Start()
    {
        if (weaponHnd)
        {
            if (shootSound)
            {
                shootSound.transform.parent = null;
                weaponHnd.OnAttacked += () => shootSound.Play();         // add playing of sound in callback
            }
        }
    }

    void OnDestroy()
    {
        if (shootSound) 
            Destroy(shootSound.gameObject);
    }
}
