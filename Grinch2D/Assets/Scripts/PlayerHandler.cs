using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class PlayerHandler is handler to control move player.
/// </summary>
public class PlayerHandler : JumpHandler
{
    private Animator animator;

    enum CollisionDirect { Left = 0, Right = 1, Down = 2 , Up = 3};     // type for checking collisions information
    int[] counts_collisions = new int[4];                               // count collisions down, left, up, right that have player

    private bool capturing = false;                                     // the player captures near the block
    public bool Capturing { get { return capturing; } }                 // Method check the player capturing near block

    void Start()
    {
        animator = GetComponent<Animator>();
    }


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

        if (Capturing)
        {
            // if the player presses the button in the opposite direction from the direction of movement
            if ((collision_maxcs.Contains(CollisionDirect.Left) && inputx > 0))
            {
                Jump();
                animator.SetTrigger("Jumping");
            }
            else if (collision_maxcs.Contains(CollisionDirect.Right) && inputx < 0)
            {
                Jump();
                animator.SetTrigger("Jumping");
            }
        }

        bool is_capturing = false;
        if (counts_collisions[(int)CollisionDirect.Down] > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
            animator.SetTrigger("Jumping");
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // condition for capturing
                bool left_chk = collision_maxcs.Contains(CollisionDirect.Left);
                bool right_chk = collision_maxcs.Contains(CollisionDirect.Right);
                is_capturing = ((left_chk && !right_chk) || (!left_chk && right_chk))
                               && !collision_maxcs.Contains(CollisionDirect.Up)
                               && !collision_maxcs.Contains(CollisionDirect.Down);
            }
        }

        if (is_capturing)
        {
            Capture();
            animator.SetBool("Capturing", true);
        }
        else
        {
            Uncapture();
            animator.SetBool("Capturing", false);
        }
        
        animator.SetBool("Falling", !collision_maxcs.Contains(CollisionDirect.Down) 
                                    || counts_collisions[(int)CollisionDirect.Down] == 0);
        animator.SetBool("Running", inputx != 0);

        if (inputx != 0 && !is_capturing)
        {
            int sign = inputx > 0 ? 1 : -1;
            transform.localScale = new Vector3(sign * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

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
        if (!capturing)
        {
            Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
            rgbody.constraints = RigidbodyConstraints2D.FreezeAll;
            capturing = true;
        }
    }

    /// <summary>
    /// Method for uncapture peforming
    /// </summary>
    private void Uncapture()
    {
        if (capturing)
        {
            Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
            rgbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            capturing = false;
        }
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
        Vector2 size2 = collider.size / 2 * new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
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
