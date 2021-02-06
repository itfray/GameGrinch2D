using UnityEngine;


/// <summary>
/// class MoveBlockHandler is handler to control move moving saw.
/// </summary>
public class MoveBlockHandler : MoveHandler
{
    public Vector2 minPosition = Vector2.zero;
    public Vector2 maxPosition = Vector2.zero;
    public Vector2 minMaxSpeed = new Vector2(5, 5);
    public Vector2 maxMaxSpeed = new Vector2(10, 10);

    protected override void UpdateDirection()
    {
        float dir_x = Mathf.Abs(direction.x);
        float dir_y = Mathf.Abs(direction.y);

        if (transform.position.x > maxPosition.x)
            direction.x = -dir_x;
        else if (transform.position.x < minPosition.x)
            direction.x = dir_x;

        if (transform.position.y > maxPosition.y)                 // check boundaries
            direction.y = -dir_y;
        else if (transform.position.y < minPosition.y)
            direction.y = dir_y;
    }

    protected override void UpdatePosition()
    {
        base.UpdatePosition();
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
