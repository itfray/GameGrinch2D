using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveHandler : JumpHandler
{
    protected override void UpdateDirection()
    {
        if (Input.GetKey(KeyCode.D))
        {
            direction.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            direction.x = -1;
        }
        else
        {
            direction.x = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            direction.y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction.y = -1;
        }
        else
        {
            direction.y = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        base.UpdateDirection();
    }

    protected override void UpdatePosition()
    {
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        rgbody.position += speed * Time.deltaTime;
    }
}
