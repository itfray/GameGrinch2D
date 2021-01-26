using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    public GameObject[] damageObjs;
    public int health = 1;
    public GameObject blood_spltr;
    public GameObject blood_spltr_pref;

    void Start()
    {
        if (blood_spltr_pref) 
            blood_spltr = Instantiate(blood_spltr_pref, transform.position, Quaternion.identity, transform.parent);

        Init();
    }

    protected virtual void Init()
    {
    }

    public virtual void Damage(int damage)
    {
        if (blood_spltr)
        {
            blood_spltr.transform.position = new Vector3(transform.position.x, transform.position.y,
                                                         blood_spltr.transform.position.z);
            Animator animator = blood_spltr.GetComponent<Animator>();
            if (animator)
                if (animator.parameterCount > 0)
                    animator.SetTrigger(animator.parameters[0].name);
        }

        if (health > 0)
        {
            health -= damage;

            if (health <= 0)
            {
                Die();
            }
        }
    }

    protected virtual void Die()
    {
    }


    void OnCollisionEnter2D(Collision2D collisions)
    {
        bool damaged = false;
        for (int i = 0; i < collisions.contactCount && !damaged; i++)
        {
            GameObject obj = collisions.GetContact(i).collider.gameObject;
            DamageHandler dmg_hnd = obj.GetComponent<DamageHandler>();
            if (dmg_hnd == null) continue;

            foreach (GameObject dmg_obj in damageObjs)
            {
                if (dmg_hnd.owner)
                {
                    if (dmg_obj.tag == dmg_hnd.owner.tag)
                    {
                        Damage(dmg_hnd.damage);
                        damaged = true;
                        break;
                    }
                }
            }
        }
    }
}
