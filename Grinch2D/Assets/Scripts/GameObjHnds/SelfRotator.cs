using UnityEngine;

/// <summary>
/// SelfRotator is class that rotate itself on round self Axis
/// </summary>
public class SelfRotator : MonoBehaviour
{
    public float startAngle = 0;                                    // start angle value
    public float dAngle = 1;                                        // angle derivative value

    public bool randDAngle = false;
    public float dStartAngle = 0.5f;                                                                        // start value for creation dAngle value for saw
    public float dEndAngle = 2f;                                                                            // end value for creation dAngle value for saw

    private float curr_angle = 0;                                   // current angle value

    // Start is called before the first frame update
    void Start()
    {
        curr_angle = startAngle;                                    // init current angle value

        if (randDAngle) setRandDAngle();                            // set random value for angle of rotation
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = Vector3.forward * curr_angle;       // roate object on angle
        curr_angle += dAngle;

        if (curr_angle >= 360) 
            curr_angle -= 360;
        else if (curr_angle <= -360)
            curr_angle += 360;
    }

    /// <summary>
    /// Method is set random value for dAngle
    /// </summary>
    public void setRandDAngle()
    {
        dAngle = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(dStartAngle, dEndAngle);     // change dAngle value
    }
}
