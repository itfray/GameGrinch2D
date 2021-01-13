using UnityEngine;

public class MoveHandler : MonoBehaviour
{
    public Vector2 max_speed = new Vector2(10, 10);

    public Vector2 direction = new Vector2(0, 0);

    protected Vector2 speed;

    void Update()
    {
        UpdateDirection();
        speed = new Vector2(max_speed.x * direction.x, max_speed.y * direction.y);
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    protected virtual void UpdateDirection()
    {
        // code updating direction
    }

    protected virtual void UpdatePosition()
    {
        // code updating game object position
    }
}
