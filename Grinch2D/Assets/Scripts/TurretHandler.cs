using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHandler : MonoBehaviour
{
    public Vector2 main_point = new Vector2(0, 0);
    public Vector2 point = new Vector2(4, 0);

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float angle = calcAngle(main_point, point);
        Debug.Log("main_point: " + main_point);
        Debug.Log("point: " + point);
        Debug.Log("angle: " + angle);
        Debug.Log("===========================================");
    }

    public static float calcAngle(Vector2 main_point, Vector2 point)
    {
        float AB = point.x - main_point.x;
        float BC = point.y - main_point.y;
        float AC = Mathf.Sqrt(AB * AB + BC * BC);
        float cos_angl = AB / AC;
        float sin_angl = BC / AC;

        float angle = 0;
        if (sin_angl >= 0)
            angle = Mathf.Acos(cos_angl);
        else
            if (cos_angl < 0)
                angle = Mathf.PI - Mathf.Asin(sin_angl);
            else
                angle = 2 * Mathf.PI + Mathf.Asin(sin_angl);

        return angle * Mathf.Rad2Deg;
    }
}
