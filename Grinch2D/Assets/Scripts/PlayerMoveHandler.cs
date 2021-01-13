using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();                                                 // peform jump

        base.UpdateDirection();
    }

    /// <summary>
    /// Update player position by Rigidbody2D
    /// </summary>
    protected override void UpdatePosition()
    {
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        rgbody.MovePosition(rgbody.position + speed * Time.deltaTime);
    }
}
