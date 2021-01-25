using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBulletHnd : BulletHandler
{
    public float ddirect = 0.1f;
    public float dangle = 0.1f;
    public GameObject target;

    protected override void UpdateDirection()
    {
        if (target == null) return;

        Vector2 direct = MathWay.calcDirect(new Vector2(transform.position.x, transform.position.y),
                                            new Vector2(target.transform.position.x, target.transform.position.y));
        direction = Vector2.Lerp(direction, direct, ddirect);
        float angle = MathWay.calcAngle(Vector2.Lerp(new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),
                                                                 Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad)),
                                                     direct, dangle));
        transform.eulerAngles = Vector3.forward * angle;
    }
}
