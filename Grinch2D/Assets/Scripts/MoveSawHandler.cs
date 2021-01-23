using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// class MoveSawHandler is handler to control move moving saw.
/// </summary>
public class MoveSawHandler : MoveHandler
{
    public Vector2 minPosition = Vector3.zero;
    public Vector2 maxPosition = Vector3.zero;

    protected override void UpdateDirection()
    {
        if (transform.position.y >= maxPosition.y || transform.position.y <= minPosition.y)                 // check boundaries
            direction.y = -direction.y;

        if (transform.position.x >= maxPosition.x || transform.position.x <= minPosition.x)
            direction.x = -direction.x;
    }

    protected override void UpdatePosition()
    {
        if ((transform.position.x > maxPosition.x && speed.x > 0) || 
            (transform.position.x < minPosition.x && speed.x < 0))
            speed.x = 0;

        if ((transform.position.y > maxPosition.y && speed.y > 0) ||
            (transform.position.y < minPosition.y && speed.y < 0))
            speed.y = 0;

        transform.position = transform.position + new Vector3(speed.x * Time.deltaTime, speed.y * Time.deltaTime, 0);
    }
}
