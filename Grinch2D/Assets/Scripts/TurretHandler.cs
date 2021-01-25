using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TurretHandler is class for handle turret actions
/// </summary>
public class TurretHandler : MonoBehaviour
{
    public string targetTag = "Player";                                     // target object tag
    public GameObject target;                                               // target object

    private GameObject gun;                                                 // turret gun

    private WeaponHandler weapon_hnd;                                       // turret weapon handler

    // Start is called before the first frame update
    void Start()
    {
        gun = transform.GetChild(0).gameObject;                             // get gun reference
        weapon_hnd = GetComponent<WeaponHandler>();                         // get weapon
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)                                                                                                 
        {
            UpdateTarget();                                                                                                 // update target object reference
            return;
        }

        Vector2 direct = MathWay.calcDirect(new Vector2(transform.position.x, transform.position.y),                        
                                            new Vector2(target.transform.position.x, target.transform.position.y));
        RotateGun(direct);
        Attack(direct);
    }

    /// <summary>
    /// Method for find target object by tag.
    /// </summary>
    void UpdateTarget()
    {
        if (targetTag != null && targetTag.Length > 0)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);
            if (objs.Length > 0)
                target = objs[0];
        }
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
        GameObject bullet = weapon_hnd.Attack<TurretBulletHnd>(gun.transform.position, direct);                             // shoot in target

        if (bullet == null) return;
        TurretBulletHnd bulletHnd = bullet.GetComponent<TurretBulletHnd>();
        if (bulletHnd) bulletHnd.target = target;                                                                           // set target for bullet
    }
}
