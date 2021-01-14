using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    private bool down_collision = false;
    private bool left_collision = false;
    private bool right_collision = false;
    private bool is_capturing = false;

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        if (IsCapturing())
        {
            if ((left_collision && inputx > 0))
            {
                Jump();
            }
            else if (right_collision && inputx < 0)
            {
                Jump();
            }
        }

        bool capturing = false;
        if (down_collision)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
                down_collision = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                capturing = left_collision || right_collision;
            }
        }

        if (capturing)
            Capture();
        else
            Uncapture();

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

    private void Capture()
    {
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        rgbody.constraints = RigidbodyConstraints2D.FreezeAll;
        is_capturing = true;
    }

    private void Uncapture()
    {
        if (is_capturing)
        {
            Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
            rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            is_capturing = false;
        }
    }

    private bool IsCapturing()
    {
        return is_capturing;
    }

    void OnCollisionStay2D(Collision2D collisions)
    {
        for (int i = 0; i < collisions.contactCount; i++)
        {
            if (collisions.contacts[i].normal.y > 0)                  // if there are objects under player
            {
                down_collision = true;
            }

            if (collisions.contacts[i].normal.x > 0)
            {
                left_collision = true;
            }
            else if (collisions.contacts[i].normal.x < 0)
            {
                right_collision = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        down_collision = false;
        left_collision = false;
        right_collision = false;
    }
}
