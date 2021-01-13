using UnityEngine;

/// <summary>
/// class MoveHandler is handler to control move game object.
/// </summary>
public class MoveHandler : MonoBehaviour
{
    public Vector2 max_speed = new Vector2(10, 10);                     // max speed value
    public Vector2 direction = new Vector2(0, 0);                       // current move direction

    protected Vector2 speed;                                            // current speed value

    void Update()
    {
        UpdateDirection();                                                                      // update current move direction
        speed = new Vector2(max_speed.x * direction.x, max_speed.y * direction.y);              // update current speed value
    }

    void FixedUpdate()
    {
        UpdatePosition();                                                                       // update game object position                                              
    }

    protected virtual void UpdateDirection()
    {
        // code updating current move direction
    }

    protected virtual void UpdatePosition()
    {
        // code updating game object position
    }
}
