using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    enum CollisionDirect { Left = 0, Right = 1, Down = 2 , Up = 3};
    bool[] flags_collisions = new bool[4];                         // player have down, left, up, right collisions?
    int[] counts_collisions = new int[4];                          // count collisions down, left, up, right that have player
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
            if ((flags_collisions[(int)CollisionDirect.Left] && inputx > 0))
            {
                Jump();
                flags_collisions[(int)CollisionDirect.Down] = false;
            }
            else if (flags_collisions[(int)CollisionDirect.Right] && inputx < 0)
            {
                Jump();
                flags_collisions[(int)CollisionDirect.Down] = false;
            }
        }

        bool capturing = false;
        if (flags_collisions[(int)CollisionDirect.Down])
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();                                                             // pefrom jump
                flags_collisions[(int)CollisionDirect.Down] = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {                    
                capturing = flags_collisions[(int)CollisionDirect.Left] ||          // condition for capturing
                            flags_collisions[(int)CollisionDirect.Right];
            }
        }

        // if condition for capturing true
        if (capturing)
            Capture();
        else
            Uncapture();

        Debug.Log("count_left_collisions: " + counts_collisions[(int)CollisionDirect.Left].ToString());
        Debug.Log("count_right_collisions: " + counts_collisions[(int)CollisionDirect.Right].ToString());
        Debug.Log("count_down_collisions: " + counts_collisions[(int)CollisionDirect.Down].ToString());
        Debug.Log("count_up_collisions: " + counts_collisions[(int)CollisionDirect.Up].ToString());
        Debug.Log("===================================================================================");
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

    void unsetCountsCollisions()
    {
        for (int i = 0; i < counts_collisions.Length; i++)
        {
            counts_collisions[i] = 0;
        }
    }

    void unsetFlagsCollisions()
    {
        for (int i = 0; i < flags_collisions.Length; i++)
        {
            flags_collisions[i] = false;
        }
    }

    void OnCollisionStay2D(Collision2D collisions)
    {
        unsetCountsCollisions();
        unsetFlagsCollisions();

        for (int i = 0; i < collisions.contactCount; i++)
        {
            if (collisions.GetContact(i).normal.y > 0)
            {
                flags_collisions[(int)CollisionDirect.Down] = true;
            }
            else if (collisions.GetContact(i).normal.y < 0)
            {
                flags_collisions[(int)CollisionDirect.Up] = true;
            }

            if (collisions.GetContact(i).normal.x > 0)
            {
                flags_collisions[(int)CollisionDirect.Left] = true;
            }
            else if (collisions.GetContact(i).normal.x < 0)
            {
                flags_collisions[(int)CollisionDirect.Right] = true;
            }
        }

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 size2 = collider.size / 2 * transform.localScale;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2[] direct_pos = new Vector2[counts_collisions.Length];
        for (int i = 0; i < direct_pos.Length; i++)
        {
            direct_pos[i] = position + size2 * CollisionDirectToVector2((CollisionDirect)i);
        }

        for (int ic = 0; ic < collisions.contactCount; ic++)
        {
            Vector2[] diffs = new Vector2[counts_collisions.Length];

            for (int i = 0; i < diffs.Length; i++)
            {
                diffs[i] = direct_pos[i] - collisions.GetContact(ic).point;
                
                if (i == (int)CollisionDirect.Up || i == (int)CollisionDirect.Down)
                {
                    if (Mathf.Abs(diffs[i].y) < Mathf.Abs(diffs[i].x))
                        counts_collisions[i]++;
                }

                if (i == (int)CollisionDirect.Left || i == (int)CollisionDirect.Right)
                {
                    if (Mathf.Abs(diffs[i].y) > Mathf.Abs(diffs[i].x))
                        counts_collisions[i]++;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        unsetCountsCollisions();
        unsetFlagsCollisions();
    }

    static Vector2 CollisionDirectToVector2(CollisionDirect direct)
    {
        switch (direct)
        {
            case CollisionDirect.Left:
                return Vector2.left;
            case CollisionDirect.Right:
                return Vector2.right;
            case CollisionDirect.Down:
                return Vector2.down;
            case CollisionDirect.Up:
                return Vector2.up;
            default:
                return Vector2.up;
        }
    }
}
