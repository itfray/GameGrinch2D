using UnityEngine;


/// <summary>
/// HealthHandler is class for game objects, that 
/// have health bar.
/// </summary>
public abstract class HealthHandler : MonoBehaviour
{
    public int max_health = 1;                                                      // max_health_value
    public int health = 1;                                                          // health value
    public GameObject[] damageObjs;                                                 // information of game objects that can damage this game object

    public delegate void HealthEventHnd();                          // type handler of events of HealthHandler
    public event HealthEventHnd OnDied;                             // invoke when player is died
    public event HealthEventHnd OnDamaged;                          // invoke when player is damaged

    void Start()
    {
        InitHealthHnd();
    }

    /// <summary>
    /// Method for initialization health handler
    /// </summary>
    protected virtual void InitHealthHnd()
    {
        InitHealth();
    }

    /// <summary>
    /// Method for initializtion health value
    /// </summary>
    public void InitHealth()
    {
        health = max_health;
    }

    /// <summary>
    /// Method is handler damage game object.
    /// </summary>
    protected virtual void Damaging()
    {
        OnDamaged?.Invoke();
    }

    /// <summary>
    /// Method is handler of gameobject death.
    /// </summary>
    protected virtual void Dying()
    {
        OnDied?.Invoke();
    }

    /// <summary>
    /// Method damages game object.
    /// </summary>
    /// <param name="damage"> damage value </param>
    public virtual void Damage(int damage)
    {
        if (health > 0)
        {
            Damaging();

            health -= damage;

            if (health <= 0)
                Dying();
        }
    }

    void OnCollisionEnter2D(Collision2D collisions)
    {
        bool damaged = false;
        for (int i = 0; i < collisions.contactCount && !damaged; i++)
        {
            GameObject obj = collisions.GetContact(i).collider.gameObject;
            DamageHandler dmg_hnd = obj.GetComponent<DamageHandler>();                      // check, that object have DamageHandler
            if (dmg_hnd == null) continue;

            foreach (GameObject dmg_obj in damageObjs)
            {
                if (dmg_hnd.owner)
                {
                    if (dmg_obj.tag == dmg_hnd.owner.tag)                                   // if tag of checked object contains in list damageObjs
                    {
                        Damage(dmg_hnd.damage);                                             // damage
                        damaged = true;
                        break;
                    }
                }
            }
        }
    }
}
