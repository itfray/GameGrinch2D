using UnityEngine;

/// <summary>
/// PlayerHealthHnd is class for player, that 
/// has health bar.
/// </summary>
public class PlayerHealthHnd : HealthHandler
{ 
    public GameObject blood_spltr_pref;                             // blood splatter prefab
    private GameObject blood_spltr;                                 // blood splatter object

    void Start()
    {
        CreateBloodSplttr();                                        // create blood splatter object
    }

    /// <summary>
    /// Method is handler damage player.
    /// </summary>
    protected override void Damaging()
    {
        Animator animator = GetComponent<Animator>();
        if (animator) animator.SetTrigger("Hitting");               // play hitting animation
    }

    /// <summary>
    /// Method is handler of player death.
    /// </summary>
    protected override void Dying()
    {
        BloodSplash();
    }

    /// <summary>
    /// Method instantiates blood splatter object
    /// </summary>
    private void CreateBloodSplttr()
    {
        if (blood_spltr_pref)
            blood_spltr = Instantiate(blood_spltr_pref, transform.position, Quaternion.identity, transform.parent);
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
            Animator animator = blood_spltr.GetComponent<Animator>();
            if (animator)
                if (animator.parameterCount > 0)
                    animator.SetTrigger(animator.parameters[0].name);
        }
    }
}
