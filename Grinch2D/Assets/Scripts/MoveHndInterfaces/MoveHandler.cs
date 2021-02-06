using UnityEngine;

/// <summary>
/// class MoveHandler is handler to control move game object.
/// </summary>
public abstract class MoveHandler : MonoBehaviour
{
    public Vector2 max_speed = new Vector2(10, 10);                     // max speed value
    public Vector2 direction = new Vector2(0, 0);                       // current move direction

    protected Vector2 speed;                                            // current speed value

    void Update()
    {
        UpdateDirection();                                                                      // update current move direction
    }

    void FixedUpdate()
    {
        UpdatePosition();                                                                       // update game object position                                              
    }

    /// <summary>
    /// Method for updating current move direction
    /// </summary>
    protected abstract void UpdateDirection();

    /// <summary>
    /// Method for updating game object position
    /// </summary>
    protected virtual void UpdatePosition()
    {
        speed = new Vector2(max_speed.x * direction.x, max_speed.y * direction.y);              // update current speed value
    }
}
