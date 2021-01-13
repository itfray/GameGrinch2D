using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    private bool in_air = false;                                    // player in air?

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        if (Input.GetKeyDown(KeyCode.Space) && !in_air)
        {
            Jump();                                                 // peform jump
            in_air = true;
        }

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

    void OnCollisionStay2D(Collision2D collisions)
    {
        for (int i = 0; i < collisions.contactCount; i++)
        {
            if (transform.position.y > collisions.contacts[i].point.y)                  // if there are objects under player
            {
                in_air = false;             
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        in_air = true;                                                                  // set flag in air
    }
}
