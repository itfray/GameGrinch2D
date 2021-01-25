using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHandler : MonoBehaviour
{
    public string targetTag = "Player";
    public GameObject target;

    private GameObject gun;

    // Start is called before the first frame update
    void Start()
    {
        gun = transform.GetChild(0).gameObject;
        if (gun.name != "gun")
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform obj = transform.GetChild(i);
                if (obj.gameObject.name == "gun")
                    gun = obj.gameObject;
            }
        }
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
            float angle = MathWay.calcAngle(new Vector2(transform.position.x, transform.position.y),
                                               new Vector2(target.transform.position.x, target.transform.position.y));
            gun.transform.eulerAngles = Vector3.forward * angle;
        }
    }
}
