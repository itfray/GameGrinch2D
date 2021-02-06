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

    public delegate void BulletEventHnd();                                        // type handler of events of BulletHandler
    public event BulletEventHnd OnDestructed;                                     // invoke when bullet is destructed

    protected Collider2D collider2d;
    protected Rigidbody2D rgbody2d;
    protected TrailRenderer trailrndr;

    void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        rgbody2d = GetComponent<Rigidbody2D>();
        trailrndr = GetComponentInChildren<TrailRenderer>();

        Release();
    }

    /// <summary>
    /// Method allows actions of bullet and set bullet in init state.
    /// </summary>
    public virtual void Init()
    {
        if (trailrndr) trailrndr.Clear();                                                           // clear trails of bullets
        if (collider2d) collider2d.enabled = true;                                                  // enable collider
        if (rgbody2d) rgbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;                 // unfreeze rigidgidbody actions

        is_released = false;
    }

    /// <summary>
    /// Method unallows actions of bullet and set bullet in release state.
    /// </summary>
    public virtual void Release()
    {
        if (trailrndr) trailrndr.Clear();                                                           // clear trails of bullets
        if (collider2d) collider2d.enabled = false;                                                 // unenable collider
        if (rgbody2d) rgbody2d.constraints = RigidbodyConstraints2D.FreezeAll;                      // freeze rigidgidbody actions

        is_released = true;
    }

    public virtual void PlaceReleasedPos()
    {
        transform.position = released_pos;                                                          // set position in released position
    }

    /// <summary>
    /// Method is handler of bullet destruction.
    /// </summary>
    public virtual void BulletDestruction()
    {
        OnDestructed?.Invoke();
    }

    /// <summary>
    /// Method is handler of changing bullet direction.
    /// </summary>
    protected override void UpdateDirection() {}

    /// <summary>
    /// Method is handler of changing bullet postion.
    /// </summary>
    protected override void UpdatePosition()
    {
        base.UpdatePosition();

        if (!is_released && rgbody2d != null)
            rgbody2d.velocity = speed;
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
                    PlaceReleasedPos();
                    Release();                                                              // release bullet
                    break;
                }
            }
        }
    }
}
