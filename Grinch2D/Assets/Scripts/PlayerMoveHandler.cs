using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    private bool down_collision = false;                           // player have down collisions?
    private bool left_collision = false;                           // player have left collisions?
    private bool right_collision = false;
    private bool is_capturing = false;                             // the player captures near the block

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        if (IsCapturing())
        {
            // if the player presses the button in the opposite direction from the direction of movement
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
                Jump();                                                             // pefrom jump
                down_collision = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                capturing = left_collision || right_collision;                      // condition for capturing
            }
        }

        // if condition for capturing true
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

    /// <summary>
    /// Method for capture peforming
    /// </summary>
    private void Capture()
    {
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        rgbody.constraints = RigidbodyConstraints2D.FreezeAll;
        is_capturing = true;
    }

    /// <summary>
    /// Method for uncapture peforming
    /// </summary>
    private void Uncapture()
    {
        if (is_capturing)
        {
            Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
            rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            is_capturing = false;
        }
    }

    /// <summary>
    /// Method check the player capturing near block
    /// </summary>
    /// <returns></returns>
    public bool IsCapturing()
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
            
            if (collisions.contacts[i].normal.x < 0)
            {
                right_collision = true;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        down_collision = false;                                     // unset all collisions flags
        left_collision = false;
        right_collision = false;
    }
}
