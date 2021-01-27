﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// WeaponHandler is class for handle of shoots.
/// </summary>
public class WeaponHandler<BulletHndT> : MonoBehaviour where BulletHndT: BulletHandler
{
    public GameObject bulletPrefab;                                               // bullet prefab
    public GameObject explodPrefab;                                               // explosion prefab

    public float rechrgTime = 0.25f;                                              // recharge time in seconds
    private float curRchrgTime = 0f;                                              // current recharge time in seconds 

    public bool canShoot { get{ return curRchrgTime <= 0f; } }                    // can shoot?

    public int maxcBullets = 10;                                                  // max count bullets

    private LinkedList<GameObject> bullets = new LinkedList<GameObject>();        // list generated bullets

    /// <summary>
    /// Method for creating bullets for weapon
    /// </summary>
    /// <param name="inst_bullet_pos"> position for instantiate bullets </param>
    public void CreateBullets(Vector2 inst_bullet_pos)
    {
        if (bulletPrefab == null) return;

        for (int i = 0; i < maxcBullets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, inst_bullet_pos, Quaternion.identity, transform.parent);              // generate bullet
            bullets.AddLast(bullet);

            BulletHndT bulletHndlr = bullet.GetComponent<BulletHndT>();                                                         // get bullet handler
            if (bulletHndlr)
            {
                bulletHndlr.released_pos = inst_bullet_pos;
                if (explodPrefab)
                {
                    GameObject explod = Instantiate(explodPrefab, inst_bullet_pos, Quaternion.identity, transform.parent);      // generate bullet explod
                    bulletHndlr.explod_obj = explod;
                }
            }

            DamageHandler dmg_hnd = bullet.GetComponent<DamageHandler>();
            if (dmg_hnd) dmg_hnd.owner = gameObject;                                                                            // set owner info
        }
    }

    void Update()
    {
        if (curRchrgTime > 0)
            curRchrgTime -= Time.deltaTime;                                                                     // update current recharge time
    }

    /// <summary>
    /// Method shoots from weapon.
    /// </summary>
    /// <param name="shoot_pos"> shoot position for bullet </param>
    /// <param name="shoot_direct"> shoot direction </param>
    /// <returns> bullet game object </returns>
    public GameObject Attack(Vector2 shoot_pos, Vector2 shoot_direct)
    {
        if (!canShoot || bullets.Count == 0) return null;                                                       // check possibility of a shot

        curRchrgTime = rechrgTime;                                                                              // update current recharge time

        GameObject bullet = bullets.FirstOrDefault();                                                           // get bullet of bullets list
        BulletHndT bulletHndlr = bullet.GetComponent<BulletHndT>();                                             // get bullet handler

        if (bulletHndlr != null)
        {
            if (!bulletHndlr.isReleased)                                                                        // if bullet not released
            {
                bulletHndlr.BulletDestruction();                                                                // release bullet
                bulletHndlr.Release();
            }
        }

        bullet.transform.position = shoot_pos;                                                                  // set shoot positon
        bullet.transform.eulerAngles = Vector3.forward * MathWay.calcAngle(shoot_direct);                       // set shoot angle

        if (bulletHndlr != null)
        {
            bulletHndlr.Init();                                                                                 // init bullet
            bulletHndlr.direction = shoot_direct;                                                               // set shoot direction
        }

        bullets.RemoveFirst();                                                                                  // put the bullet at the end of the list
        bullets.AddLast(bullet);

        return bullet;
    }
}

