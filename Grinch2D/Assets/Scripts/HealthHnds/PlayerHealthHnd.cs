using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerHealthHnd is class for player, that 
/// has health bar.
/// </summary>
public class PlayerHealthHnd : HealthHandler
{ 
    public GameObject blood_spltr_pref;                             // blood splatter prefab
    private GameObject blood_spltr;                                 // blood splatter object
    private Animator blood_spltr_anmtr;                             // animator of blood splatter object

    private Animator animator;

    private List<ContactPoint2D> stay_contacts = new List<ContactPoint2D>();

    protected override void InitHealthHnd()
    {
        base.InitHealthHnd();

        animator = GetComponent<Animator>();
        CreateBloodSplttr();                                        // create blood splatter object
    }

    /// <summary>
    /// Method is handler damage player.
    /// </summary>
    protected override void Damaging(GameObject obj)
    {
        if (animator) 
            animator.SetTrigger("Hitting");               // play hitting animation

        base.Damaging(obj);
    }

    /// <summary>
    /// Method is handler of player death.
    /// </summary>
    protected override void Dying(GameObject obj)
    {
        BloodSplash();
        gameObject.SetActive(false);

        base.Dying(obj);
    }

    /// <summary>
    /// Method instantiates blood splatter object
    /// </summary>
    private void CreateBloodSplttr()
    {
        if (blood_spltr_pref)
        {
            blood_spltr = Instantiate(blood_spltr_pref, transform.position, Quaternion.identity, transform.parent);
            blood_spltr_anmtr = blood_spltr.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Method calls blood splash animation.
    /// </summary>
    private void BloodSplash()
    {
        if (blood_spltr)
        {
            blood_spltr.transform.position = new Vector3(transform.position.x, transform.position.y,
                                                         blood_spltr.transform.position.z);
            if (blood_spltr_anmtr)
                if (blood_spltr_anmtr.parameterCount > 0)
                    blood_spltr_anmtr.SetTrigger(blood_spltr_anmtr.parameters[0].name);
        }
    }

    void OnCollisionStay2D(Collision2D collisions)
    {
        stay_contacts.AddRange(collisions.contacts);
    }

    /// <summary>
    /// Contains check of condition of player object pressing(crushing)
    /// </summary>
    void FixedUpdate()
    {
        MoveHandler move_hnd = null;
        Vector2 move_normal = Vector2.zero;
        for (int i = 0; i < stay_contacts.Count; i++)                               // if player have collision with move object
        {
            ContactPoint2D contact = stay_contacts[i];
            GameObject obj = contact.collider.gameObject;
            move_normal = contact.normal;
            move_hnd = obj.GetComponent<MoveHandler>();
            if (move_hnd != null) break;
        }

        for (int i = 0; i < stay_contacts.Count && move_hnd != null; i++)           // if there is object behind the player
        {
            ContactPoint2D contact = stay_contacts[i];
            GameObject obj = contact.collider.gameObject;
            Vector2 move_dir_normal = new Vector2(move_hnd.direction.x, move_hnd.direction.y);
            move_dir_normal.Normalize();
            if (contact.normal == -move_normal && move_dir_normal == move_normal)   // and if move object crushs player
                Damage(health, move_hnd.gameObject);                                // kill player
        }
        stay_contacts.Clear();
    }
}
