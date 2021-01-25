using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MoveHandler
{
    public int damage = 1;
    public GameObject owner;

    protected override void UpdateDirection() {}

    protected override void UpdatePosition()
    {
        transform.position += new Vector3(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0);
    }
}
