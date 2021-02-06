using UnityEngine;


/// <summary>
/// DisappearHandler is class handler of disappearing object
/// </summary>
public abstract class DisappearHandler : MonoBehaviour
{
    public GameObject[] activators;                                                                  // activators for block

    public delegate void DisappEventHandler();
    public event DisappEventHandler OnAppear;                                                         // onAppear event called if called Appear(true)
    public event DisappEventHandler OnDisappear;

    protected bool is_appeared = false; 
    public bool IsAppeared { get { return is_appeared; } }                                            // object is appeared?

    public virtual void Appear(bool value)
    {
        is_appeared = value;
        if (value)
            OnAppear?.Invoke();
        else
            OnDisappear?.Invoke();
    }

    void OnCollisionEnter2D(Collision2D collisions)
    {
        for (int icol = 0; icol < collisions.contactCount && is_appeared; icol++)
        {
            foreach (GameObject activator in activators)
            {
                if (collisions.GetContact(icol).collider.gameObject.tag == activator.tag)               // if have collison with activator
                {
                    Appear(false);                                                                      // object disappears
                    break;
                }
            }
        }
    }
}
