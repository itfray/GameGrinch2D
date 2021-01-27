using UnityEngine;


/// <summary>
/// MathWay is static class, extension for Mathf
/// </summary>
public static class MathWay
{
    /// <summary>
    /// Function calculates angle between points.
    /// p1
    /// |
    /// |
    /// |_ 90'
    /// |_|_______0'
    ///p0
    /// </summary>
    /// <param name="main_point"> main point relative to which the angle is calculated</param>
    /// <param name="point"> point </param>
    /// <returns> angle degrees </returns>
    public static float calcAngle(Vector2 main_point, Vector2 point)
    {
        return calcAngle(calcDirect(main_point, point));
    }

    /// <summary>
    /// Function calculates direction of point reltive main point
    /// </summary>
    /// <param name="main_point"> main point </param>
    /// <param name="point"> point </param>
    /// <returns> direction {cos(a), sin(a)} </returns>
    public static Vector2 calcDirect(Vector2 main_point, Vector2 point)
    {
        float AB = point.x - main_point.x;
        float BC = point.y - main_point.y;
        float AC = Mathf.Sqrt(AB * AB + BC * BC);

        Vector2 direct = Vector2.zero;
        if (AC != 0)
        {
            direct.x = AB / AC;
            direct.y = BC / AC;
        }
        return direct;
    }

    /// <summary>
    /// Function calculates angle by direction
    /// </summary>
    /// <param name="direct"> direction </param>
    /// <returns> angle degrees </returns>
    public static float calcAngle(Vector2 direct)
    {
        float angle = 0;
        if (direct.y >= 0)
            angle = Mathf.Acos(direct.x);
        else
            if (direct.x < 0)
            angle = Mathf.PI - Mathf.Asin(direct.y);
        else
            angle = 2 * Mathf.PI + Mathf.Asin(direct.y);
        return angle * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Function is calculate how many lines with specified length can fit between two positions.
    /// Example: 
    /// pos1 = -7.5, pos2 = 0, len = 4, result = -2
    /// pos1 = 7.5, pos2 = 0, len = 4, result = 2
    /// </summary>
    /// <param name="line_len"> Checked line length </param>
    /// <param name="pos1"> Position 1 </param>
    /// <param name="pos2"> Position 2  </param>
    /// <returns> Signed count </returns>
    public static int countLinesFitOnBetween(float line_len, float pos1, float pos2)
    {
        float fc = (pos1 - pos2) / line_len;
        int fcsign = fc > 0 ? 1 : -1;
        int c = fcsign * Mathf.CeilToInt(Mathf.Abs(fc));
        return c;
    }
}
