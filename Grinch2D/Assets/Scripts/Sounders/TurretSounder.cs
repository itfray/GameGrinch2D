using UnityEngine;


/// <summary>
/// TurretSounder is class for playing turret action sounds
/// </summary>
public class TurretSounder : Sounder
{
    public AudioSource shootSound;                                                                // shoot sound

    public TurretWeaponHnd weaponHnd;                                                           // weapon handler of turret

    protected override void Init()
    {
        if (weaponHnd)
        {
            if (shootSound)
            {
                shootSound.transform.parent = null;
                weaponHnd.OnAttacked += () => shootSound.Play();         // add playing of sound in callback

                audios.Add(shootSound);
            }
        }

        base.Init();
    }
}
