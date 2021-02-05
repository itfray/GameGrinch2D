using UnityEngine;

/// <summary>
/// DisappBlockHnd is class handler of disappearing block
/// </summary>
public class DisappBlockHnd : MonoBehaviour
{
    public GameObject activator;                                                // activator for block
    public Animator animator;
    public const string activeParam = "Activate";                               // activate parameter

    private bool activated = false;                                             // disappearing is activated?

    public bool IsAppeared { get { return !activated; } }


    /// <summary>
    /// Activates disappearing of block
    /// </summary>
    public void Disappear()
    {
        activated = true;
        SetActiveParam(activated);
    }

    /// <summary>
    /// Activates appearing of block
    /// </summary>
    public void Appear()
    {
        activated = false;
        SetActiveParam(activated);
    }

    private void SetActiveParam(bool val)
    {
        if (animator) animator.SetBool(activeParam, val);
    }

    void OnCollisionEnter2D(Collision2D collisions)
    {
        for (int icol = 0; icol < collisions.contactCount; icol++)
        {
            if (collisions.GetContact(icol).collider.gameObject.tag == activator.tag)               // if have collison with activator
            {
                Disappear();                
            }
        }
    }
}
