using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// BulletHandler is class for handle bullet actions.
/// </summary>
public class BulletHandler : MoveHandler
{
    public GameObject[] destroyerObjs;                                      // tags of objects that call destroy of bullet

    public GameObject explod_obj;                                       // prefab for explosion

    public Vector2 released_pos;

    private bool is_released;
    public bool isReleased { get { return is_released; } }

    void Awake()
    {
        Release();
    }

    public virtual void Init()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = true;
        is_released = false;
        TrailRenderer trailrndr = GetComponentInChildren<TrailRenderer>();
        if (trailrndr) trailrndr.Clear();
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody) rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    protected override void UpdateDirection() {}

    protected override void UpdatePosition()
    {
        if (is_released) return;

        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody)
        {
            rgbody.velocity = speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collisions)
    {
        for (int i = 0; i < collisions.contactCount && !is_released; i++)
        {
            foreach (GameObject destroyerObj in destroyerObjs)
            {
                if (destroyerObj.tag == collisions.GetContact(i).collider.tag)
                {
                    BulletDestruction();
                    Release();
                    break;
                }
            }
        }
    }

    public virtual void Release()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = false;
        is_released = true;
        transform.position = released_pos;
        TrailRenderer trailrndr = GetComponentInChildren<TrailRenderer>();
        if (trailrndr) trailrndr.Clear();
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody) rgbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public virtual void BulletDestruction()
    {
    }
}
