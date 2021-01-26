using System.Collections.Generic;
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

    public void CreateBullets(Vector2 inst_bullet_pos)
    {
        if (bulletPrefab == null) return;

        for (int i = 0; i < maxcBullets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, inst_bullet_pos, Quaternion.identity, transform.parent);              // generate bullet
            bullets.AddLast(bullet);

            BulletHndT bulletHndlr = bullet.GetComponent<BulletHndT>();                                                         // get bullet handler
            if (bulletHndlr != null)
            {
                bulletHndlr.owner = gameObject;                                                                                 // set owner info
                bulletHndlr.released_pos = inst_bullet_pos;
                if (explodPrefab)
                {
                    GameObject explod = Instantiate(explodPrefab, inst_bullet_pos, Quaternion.identity, transform.parent);      // generate bullet explod
                    bulletHndlr.explod_obj = explod;
                }
            }
        }
    }

    void Update()
    {
        if (curRchrgTime > 0)
            curRchrgTime -= Time.deltaTime;                                       // update current recharge time
    }

    /// <summary>
    /// Method shoots from weapon
    /// </summary>
    /// <typeparam name="BulletHndT"> type bullet handler </typeparam>
    /// <param name="inst_pos"> instantiate position for bullet </param>
    /// <param name="shoot_direct"> shoot direction </param>
    /// <returns> bullet </returns>
    public GameObject Attack(Vector2 shoot_pos, Vector2 shoot_direct)
    {
        if (!canShoot || bullets.Count == 0) return null;                                                       // check possibility of a shot

        curRchrgTime = rechrgTime;                                                                              // update current recharge time

        GameObject bullet = bullets.FirstOrDefault();                                                           // generate bullet
        BulletHndT bulletHndlr = bullet.GetComponent<BulletHndT>();                                             // get bullet handler

        if (bulletHndlr != null)
        {
            if (!bulletHndlr.isReleased)
            {
                bulletHndlr.BulletDestruction();
                bulletHndlr.Release();
            }
        }

        bullet.transform.position = shoot_pos;
        bullet.transform.eulerAngles = Vector3.forward * MathWay.calcAngle(shoot_direct);                       // set shoot angle

        if (bulletHndlr != null)
        {
            bulletHndlr.Init();
            bulletHndlr.direction = shoot_direct;                                                               // set shoot direction
        }

        bullets.RemoveFirst();
        bullets.AddLast(bullet);
        return bullet;
    }
}

