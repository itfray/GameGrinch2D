using UnityEngine;


/// <summary>
/// MathWay is static class, extension for Mathf
/// </summary>
public static class MathWay
{
    /// <summary>
    /// Method calculates angle between points.
    /// p1
    /// |
    /// |
    /// |_ 90'
    /// |_|_______0'
    ///p0
    /// </summary>
    /// <param name="main_point"> main point relative to which the angle is calculated</param>
    /// <param name="point"> point </param>
    /// <returns> angle value </returns>
    public static float angleBetween(Vector2 main_point, Vector2 point)
    {
        float AB = point.x - main_point.x;
        float BC = point.y - main_point.y;
        float AC = Mathf.Sqrt(AB * AB + BC * BC);

        float angle = 0;
        if (AC != 0)
        {
            float cos_angl = AB / AC;
            float sin_angl = BC / AC;

            if (sin_angl >= 0)
                angle = Mathf.Acos(cos_angl);
            else
                if (cos_angl < 0)
                angle = Mathf.PI - Mathf.Asin(sin_angl);
            else
                angle = 2 * Mathf.PI + Mathf.Asin(sin_angl);
        }
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
        int c = fcsign * Mathf.RoundToInt(Mathf.Abs(fc));
        return c;
    }
}
