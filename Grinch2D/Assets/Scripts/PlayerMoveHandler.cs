using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerMoveHandler is handler to control move player.
/// </summary>
public class PlayerMoveHandler : JumpHandler
{
    enum CollisionDirect { Left = 0, Right = 1, Down = 2 , Up = 3};     // type for checking collisions information
    int[] counts_collisions = new int[4];                               // count collisions down, left, up, right that have player
    private bool is_capturing = false;                                  // the player captures near the block

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    /// 
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        // get list of directions, that have maximum count collisions 
        List<CollisionDirect> collision_maxcs = getDirectsMaxCollisions();

        if (IsCapturing())
        {
            // if the player presses the button in the opposite direction from the direction of movement
            if ((collision_maxcs.Contains(CollisionDirect.Left) && inputx > 0))
            {
                Jump();
            }
            else if (collision_maxcs.Contains(CollisionDirect.Right) && inputx < 0)
            {
                Jump();
            }
        }

        bool capturing = false;
        if (counts_collisions[(int)CollisionDirect.Down] > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();                                                             // pefrom jump
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // condition for capturing
                capturing = (collision_maxcs.Contains(CollisionDirect.Left) 
                          || collision_maxcs.Contains(CollisionDirect.Right))
                               && !collision_maxcs.Contains(CollisionDirect.Up)
                               && !collision_maxcs.Contains(CollisionDirect.Down);
            }
        }

        if (capturing)
            Capture();
        else
            Uncapture();

        base.UpdateDirection();
    }

    /// <summary>
    /// Method for getting list of directions, that have maximum count collisions 
    /// </summary>
    /// <returns> list of directions </returns>
    List<CollisionDirect> getDirectsMaxCollisions()
    {
        List<CollisionDirect> directs = new List<CollisionDirect>();

        int maxc_ind = (int)CollisionDirect.Up;                             // assume that the index of maximum element
        int maxc = counts_collisions[maxc_ind];

        for (int i = 0; i < counts_collisions.Length; i++)                  // find maximum element
        {
            if (maxc < counts_collisions[i])
            {
                maxc = counts_collisions[i];
                maxc_ind = i;
            }
        }

        directs.Add((CollisionDirect)maxc_ind);

        for (int i = 0; i < counts_collisions.Length; i++)                  // find all elements, that equal maximum 
        {
            if (i != maxc_ind && maxc == counts_collisions[i])
            {
                directs.Add((CollisionDirect)i);
            }
        }

        return directs;
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

    /// <summary>
    /// Method for resetting all collision count values
    /// </summary>
    void unsetCountsCollisions()
    {
        for (int i = 0; i < counts_collisions.Length; i++)
            counts_collisions[i] = 0;
    }

    void OnCollisionStay2D(Collision2D collisions)
    {
        unsetCountsCollisions();

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 size2 = collider.size / 2 * transform.localScale;
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2[] direct_pos = new Vector2[counts_collisions.Length];                       // array of extreme boundary points of the player game object

        for (int i = 0; i < direct_pos.Length; i++)
        {
            direct_pos[i] = position + size2 * DirectToVector2((CollisionDirect)i);         // fill extreme boundary points (left, right, up, down)
        }

        for (int ic = 0; ic < collisions.contactCount; ic++)
        {
            for (int i = 0; i < counts_collisions.Length; i++)
            {
                Vector2 diffs = direct_pos[i] - collisions.GetContact(ic).point;            // calculate difference extreme boundary point with collision point

                if (i == (int)CollisionDirect.Up || i == (int)CollisionDirect.Down)
                {
                    if (Mathf.Abs(diffs.y) < Mathf.Abs(diffs.x))                            // all up and down points must be less on Y-Axis relative to the collision point, than on X-Axis
                        counts_collisions[i]++;
                }
                else if (i == (int)CollisionDirect.Left || i == (int)CollisionDirect.Right)
                {
                    if (Mathf.Abs(diffs.y) > Mathf.Abs(diffs.x))
                        counts_collisions[i]++;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collisions)
    {
        unsetCountsCollisions();
    }

    /// <summary>
    /// Function for transform CollisionDirect to Vector2
    /// </summary>
    /// <param name="direct"> collison direct </param>
    /// <returns> vector value </returns>
    static Vector2 DirectToVector2(CollisionDirect direct)
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
