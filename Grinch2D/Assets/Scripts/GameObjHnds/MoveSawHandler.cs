using UnityEngine;


/// <summary>
/// class MoveSawHandler is handler to control move moving saw.
/// </summary>
public class MoveSawHandler : MoveHandler
{
    public Vector2 minPosition = Vector2.zero;
    public Vector2 maxPosition = Vector2.zero;
    public Vector2 minMaxSpeed = new Vector2(5, 5);
    public Vector2 maxMaxSpeed = new Vector2(10, 10);

    protected override void UpdateDirection()
    {
        if (transform.position.y >= maxPosition.y || transform.position.y <= minPosition.y)                 // check boundaries
            direction.y = -direction.y;

        if (transform.position.x >= maxPosition.x || transform.position.x <= minPosition.x)
            direction.x = -direction.x;
    }

    protected override void UpdatePosition()
    {
        if ((transform.position.x > maxPosition.x && speed.x > 0) ||                                        // check boundaries
            (transform.position.x < minPosition.x && speed.x < 0))
            speed.x = 0;

        if ((transform.position.y > maxPosition.y && speed.y > 0) ||
            (transform.position.y < minPosition.y && speed.y < 0))
            speed.y = 0;

        transform.position = transform.position + new Vector3(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0);
    }

    /// <summary>
    /// Method for generation random speed value
    /// </summary>
    public void setRandSpeed()
    {
        max_speed.x = Random.Range(minMaxSpeed.x, maxMaxSpeed.x);
        max_speed.y = Random.Range(minMaxSpeed.y, maxMaxSpeed.y);
    }
}
