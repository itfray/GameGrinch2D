using UnityEngine;


/// <summary>
/// BulletHandler is class for handle bullet actions.
/// </summary>
public class BulletHandler : MoveHandler
{
    public GameObject[] destroyerObjs;                                      // inforamtion of objects that call destroy of bullet

    public Vector2 released_pos;                                            // postion in released state

    protected bool is_released;                                             // bullet is released?
    public bool isReleased { get { return is_released; } }

    public GameObject explod_obj;                                           // explosion object, if has

    void Awake()
    {
        Release();
    }

    /// <summary>
    /// Method allows actions of bullet and set bullet in init state.
    /// </summary>
    public virtual void Init()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = true;                                                  // enable collider

        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody) rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;                 // unfreeze rigidgidbody actions

        TrailRenderer trailrndr = GetComponentInChildren<TrailRenderer>();
        if (trailrndr) trailrndr.Clear();                                                       // clear trails of bullets

        is_released = false;
    }

    /// <summary>
    /// Method unallows actions of bullet and set bullet in release state.
    /// </summary>
    public virtual void Release()
    {
        transform.position = released_pos;                                                      // set position in released position

        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = false;                                                 // unenable collider

        TrailRenderer trailrndr = GetComponentInChildren<TrailRenderer>();
        if (trailrndr) trailrndr.Clear();                                                       // freeze rigidgidbody actions

        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody) rgbody.constraints = RigidbodyConstraints2D.FreezeAll;                      // freeze rigidgidbody actions

        is_released = true;
    }

    /// <summary>
    /// Method is handler of bullet destruction.
    /// </summary>
    public virtual void BulletDestruction() {}

    /// <summary>
    /// Method is handler of changing bullet direction.
    /// </summary>
    protected override void UpdateDirection() {}

    /// <summary>
    /// Method is handler of changing bullet postion.
    /// </summary>
    protected override void UpdatePosition()
    {
        if (is_released) return;

        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody) rgbody.velocity = speed;
    }

    void OnCollisionEnter2D(Collision2D collisions)
    {
        for (int i = 0; i < collisions.contactCount && !is_released; i++)
        {
            foreach (GameObject destroyerObj in destroyerObjs)
            {
                if (destroyerObj.tag == collisions.GetContact(i).collider.tag)              // if object contains in list destroyerObjs
                {
                    BulletDestruction();                                                    // bullet destruction                             
                    Release();                                                              // release bullet
                    break;
                }
            }
        }
    }
}
