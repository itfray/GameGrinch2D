using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// BulletHandler is class for handle bullet actions.
/// </summary>
public class BulletHandler : MoveHandler
{
    public int damage = 1;                                              // bullet damage
    public GameObject owner;                                            // reference of owner object
    public string[] destroyerTags;                                      // tags of objects that call destroy of bullet

    protected override void UpdateDirection() {}

    protected override void UpdatePosition()
    {
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (rgbody)
        {
            rgbody.velocity = speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collisions)
    {
        for (int i = 0; i < collisions.contactCount; i++)
        {
            foreach (string destroyerTag in destroyerTags)
            {
                if (destroyerTag == collisions.GetContact(i).collider.tag)
                {
                    BulletDestruction();
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    public virtual void BulletDestruction()
    {
    }
}
