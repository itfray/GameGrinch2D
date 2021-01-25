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

    protected override void UpdateDirection() {}

    protected override void UpdatePosition()
    {
        transform.position += new Vector3(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0);
    }
}
