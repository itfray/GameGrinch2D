using UnityEngine;


/// <summary>
/// TurretHandler is class for handle turret actions
/// </summary>
public class TurretHandler : MonoBehaviour
{
    public GameObject target;                                               // target object

    public float minDistance4Shoot = 100f;                                  // minimal distance for shoot

    private GameObject gun;                                                 // turret gun

    private TurretWeaponHnd weapon_hnd;                                     // turret weapon handler

    // Start is called before the first frame update
    void Start()
    {
        gun = transform.GetChild(0).gameObject;                             // get gun object
        weapon_hnd = GetComponent<TurretWeaponHnd>();                       // get weapon
        weapon_hnd.CreateBullets(transform.position);                       // create bullets
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        if (Vector2.Distance(transform.position, target.transform.position) > minDistance4Shoot) return;                    // check target distance

        // calculate target direction
        Vector2 direct = MathWay.calcDirect(new Vector2(transform.position.x, transform.position.y),                        
                                            new Vector2(target.transform.position.x, target.transform.position.y));
        RotateGun(direct);
        Attack(direct);
    }

    /// <summary>
    /// Method rotates gun of turret
    /// </summary>
    /// <param name="direct"> direction </param>
    void RotateGun(Vector2 direct)
    {
        gun.transform.eulerAngles = Vector3.forward * MathWay.calcAngle(direct);                                            // update rotation angle of turret gun
    }

    /// <summary>
    /// Method shoots in direction of target
    /// </summary>
    /// <param name="direct"> direction </param>
    void Attack(Vector2 direct)
    {
        if (weapon_hnd == null) return;

        TurretBulletHnd bullet_hnd = weapon_hnd.Attack(transform.position, direct);                                                            // shoot in target
        if (bullet_hnd) bullet_hnd.target = target;                                                                                            // set target for bullet
    }
}
