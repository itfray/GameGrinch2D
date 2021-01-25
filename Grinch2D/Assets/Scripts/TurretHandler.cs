using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHandler : MonoBehaviour
{
    public string targetTag = "Player";
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            UpdateTarget();

        UpdateGunAngle();
    }

    void UpdateTarget()
    {
        if (targetTag != null && targetTag.Length > 0)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(targetTag);
            if (objs.Length > 0)
                target = objs[0];
        }
    }

    void UpdateGunAngle()
    {
        if (target != null)
        {
            float angle = MathWay.angleBetween(new Vector2(transform.position.x, transform.position.y),
                                               new Vector2(target.transform.position.x, target.transform.position.y));
            transform.eulerAngles = Vector3.forward * angle;
        }
    }
}
