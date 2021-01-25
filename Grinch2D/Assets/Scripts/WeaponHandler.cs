using System.Collections.Generic;
using System.Linq;
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

    public int maxcBullets = 10;                                                  // max count bullets

    private LinkedList<GameObject> bullets;                                      // list generated bullets

    void Start()
    {
        bullets = new LinkedList<GameObject>();
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

        bullets.AddLast(bullet);

        DestroyExtraBullets();                                                                                  // destroy old and extra bullets

        return bullet;
    }

    /// <summary>
    /// Destroy old and extra bullets
    /// </summary>
    public void DestroyExtraBullets()
    {
        for (int i = 0; i < bullets.Count - maxcBullets; i++)                                                   // if maximum limit of bullets reached
        {
            GameObject bullet = bullets.FirstOrDefault();
            if (bullet) Destroy(bullet, rechrgTime);
            bullets.RemoveFirst();
        }
    }
}

