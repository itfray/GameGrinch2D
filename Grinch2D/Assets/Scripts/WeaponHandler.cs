using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// WeaponHandler is class for handle of shoots.
/// </summary>
public class WeaponHandler : MonoBehaviour
{
    public GameObject bulletPrefab;                                               // bullet prefab

    public float rechrgTime = 0.25f;                                              // recharge time in seconds
    private float curRchrgTime = 0f;                                              // current recharge time in seconds 

    public bool canShoot { get{ return curRchrgTime <= 0f; } }                    // can shoot?

    void Update()
    {
        if (curRchrgTime > 0)
            curRchrgTime -= Time.deltaTime;                                       // update current recharge time 
    }

    public GameObject Attack<BulletHndT>(Vector2 inst_pos, Vector2 shoot_direct) where BulletHndT: BulletHandler
    {
        if (!canShoot || bulletPrefab == null) return null;                                                     // check possibility of a shot

        curRchrgTime = rechrgTime;                                                                              // update current recharge time 

        GameObject bullet = Instantiate(bulletPrefab, inst_pos, Quaternion.identity, transform.parent);         // generate bullet
        BulletHndT bulletHndlr = bullet.GetComponent<BulletHndT>();                                             // get bullet handler
        if (bulletHndlr != null)
        {
            bulletHndlr.owner = gameObject;                                                                     // set owner info
            bulletHndlr.direction = shoot_direct;                                                               // set shoot direction
        }
        bullet.transform.eulerAngles = Vector3.forward * MathWay.calcAngle(shoot_direct);                       // set shoot angle
        return bullet;
    }
}

