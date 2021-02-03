using UnityEngine;


/// <summary>
/// TurretSoundPlayer is class for playing turret action sounds
/// </summary>
public class TurretSoundPlayer : SoundPlayer
{
    public AudioClip shootSound;                                                                // shoo sound

    public TurretWeaponHnd weaponHnd;                                                           // weapon handler of turret

    void Start()
    {
        if (weaponHnd)
        {
            if (shootSound)
                weaponHnd.OnAttacked += () => PlaySound(shootSound, transform.position);        // add playing of sound in callback
        }
    }
}
