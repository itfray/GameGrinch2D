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
}
