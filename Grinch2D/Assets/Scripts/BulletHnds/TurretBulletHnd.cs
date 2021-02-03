using UnityEngine;


/// <summary>
/// TurretBulletHnd is  is class for handle bullet of turret actions.
/// </summary>
public class TurretBulletHnd : BulletHandler
{
    public float dDirect = 0.01f;                                    // derivative of direction for updating bullet direction
    public float dAngle = 0.4f;                                      // derivative of angle for updating bullet angle

    public GameObject target;                                        // target game object

    protected Animator explod_animtr;

    void Start()
    {
        if (explod_obj)
            explod_animtr = explod_obj.GetComponent<Animator>();
    }

    /// <summary>
    /// Method is handler of changing bullet direction.
    /// </summary>
    protected override void UpdateDirection()
    {
        if (target == null) return;

        Vector2 direct = MathWay.calcDirect(new Vector2(transform.position.x, transform.position.y),
                                            new Vector2(target.transform.position.x, target.transform.position.y));         // calculate target direction
        direction = Vector2.Lerp(direction, direct, dDirect);                                                               // update direction

        float angle = MathWay.calcAngle(Vector2.Lerp(new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),        // calculate angle
                                                                 Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad)),
                                                     direct, dAngle));
        transform.eulerAngles = Vector3.forward * angle;                                                                    // update angle
    }

    /// <summary>
    /// Method is handler of bullet destruction.
    /// </summary>
    public override void BulletDestruction()
    {
        if (explod_obj)
        {
            explod_obj.transform.position =                                                                                             // set postion of explosion
                       new Vector3(transform.position.x, transform.position.y, explod_obj.transform.position.z);

            if (explod_animtr)
            {
                if (explod_animtr.parameterCount > 0)
                    explod_animtr.SetTrigger(explod_animtr.parameters[0].name);                                                         // play explosion animation
            }
        }

        base.BulletDestruction();
    }
}
