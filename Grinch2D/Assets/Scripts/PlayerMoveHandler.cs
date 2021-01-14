using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    private bool up_collision = false;
    private bool down_collision = false;
    private bool left_collision = false;
    private bool right_collision = false;
    private bool is_capture = false;

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        if (is_capture)
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

        is_capture = false;
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
                is_capture = left_collision || right_collision;
                Debug.Log("left_collision: " + left_collision.ToString());
                Debug.Log("right_collision: " + right_collision.ToString());
                Debug.Log("up_collision: " + up_collision.ToString());
                Debug.Log("down_collision: " + down_collision.ToString());
            }
        }

        Debug.Log("is_capture: " + is_capture.ToString());
        Debug.Log("===================================================");

        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (is_capture)
        {
            rgbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;
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
            if (collisions.contacts[i].normal.y > 0)                  // if there are objects under player
            {
                down_collision = true;
            }

            if (collisions.contacts[i].normal.y < 0)                  // if there are objects under player
            {
                up_collision = true;
            }

            if (collisions.contacts[i].normal.x > 0)
            {
                left_collision = true;
            }

            if (collisions.contacts[i].normal.x < 0)
            {
                right_collision = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        up_collision = false;
        down_collision = false;
        left_collision = false;
        right_collision = false;
        is_capture = false;
    }
}
