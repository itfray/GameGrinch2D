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
    private bool is_capture = false;

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        /*        if (Input.GetKeyDown(KeyCode.Space) && down_collision)
                {
                    Jump();                                                 // peform jump
                }*/

        is_capture = false;
        if (Input.GetKey(KeyCode.Space))
        {
            if (down_collision)
            {
                Jump();
                down_collision = false;
            }
            else
            {
                bool left_key = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow));
                bool right_key = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow));
                bool is_capture_left = left_key && !right_key && left_collision;
                bool is_capture_right = right_key && !left_key && right_collision;
                is_capture = is_capture_left || is_capture_right;

                Debug.Log("is_capture_left" + is_capture_left.ToString());
                Debug.Log("is_capture_right" + is_capture_right.ToString());
                Debug.Log("is_capture: " + is_capture.ToString());
                Debug.Log("\n");
            }
        }

        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        if (is_capture) 
            rgbody.constraints = RigidbodyConstraints2D.FreezeAll;
        else
            rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;

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

            if (collisions.contacts[i].normal.x < 0)
            {
                right_collision = true;
            }

            if (collisions.contacts[i].normal.x > 0)
            {
                left_collision = true;
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
