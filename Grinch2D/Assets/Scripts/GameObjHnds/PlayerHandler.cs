using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// class PlayerHandler is handler to control move player.
/// </summary>
public class PlayerHandler : JumpHandler
{
    public enum CollisionDirect { Left = 0, Right = 1, Down = 2 , Up = 3};      // type for checking collisions information
    private int[] counts_collisions = new int[4];                               // count collisions down, left, up, right that have player
    private bool capturing = false;                                             // the player captures near the block
    public bool Capturing { get { return capturing; } }                         // Method check the player capturing near block

    private Animator animator;
    private Rigidbody2D rgbody2d;
    private BoxCollider2D collider2d;

    void Start()
    {
        animator = GetComponent<Animator>();
        rgbody2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Method contains code for updating current move direction
    /// </summary>
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");                 // change move direction on left or right
        direction.x = inputx;

        // get directions information, true value that have maximum count collisions 
        Dictionary<CollisionDirect, bool> max_colls_dirs = 
            getDirectsMaxCollisions(CollisionDirect.Up, CollisionDirect.Down, CollisionDirect.Left, CollisionDirect.Right);


        // if max count collisions only up collisions
        if (max_colls_dirs[CollisionDirect.Up] && onlyOneTrue(max_colls_dirs, el => el.Value))
        {
            StopJump();                                                                                 // reset jump
            animator.ResetTrigger("Jumping");
        }

        if (Capturing)
        {
            // if the player presses the button in the opposite direction from the direction of movement
            if ((max_colls_dirs[CollisionDirect.Left] && inputx > 0))
            {
                Jump();
                animator.SetTrigger("Jumping");
            }
            else if (max_colls_dirs[CollisionDirect.Right] && inputx < 0)
            {
                Jump();
                animator.SetTrigger("Jumping");
            }
        }

        bool is_capturing = false;
        if (counts_collisions[(int)CollisionDirect.Down] > 0 && Input.GetKeyDown(KeyCode.Space))            // if player have down collsions
        {
            Jump();                                                                                         // peform jump
            animator.SetTrigger("Jumping");
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // condition for capturing
                is_capturing = ((max_colls_dirs[CollisionDirect.Left] && !max_colls_dirs[CollisionDirect.Right]) ||
                                (!max_colls_dirs[CollisionDirect.Left] && max_colls_dirs[CollisionDirect.Right]))
                               && !max_colls_dirs[CollisionDirect.Up] && !max_colls_dirs[CollisionDirect.Down];
            }
        }

        if (is_capturing)
        {
            Capture();                                                                      // peform capture, fix player position
            animator.SetBool("Capturing", true);
            rotatePlayer(!max_colls_dirs[CollisionDirect.Left]);                            // rotate player in side
        }
        else
        {
            Uncapture();
            animator.SetBool("Capturing", false);
            if (inputx != 0) rotatePlayer(inputx > 0);                                      // rotate player in side
        }


        // get directions information, true value that have minimum count collisions 
        Dictionary<CollisionDirect, bool> min_collds_wthout = max_colls_dirs[CollisionDirect.Left] ?
                                                              getDirectsMinCollisions(CollisionDirect.Left, CollisionDirect.Down, CollisionDirect.Up):
                                                              getDirectsMinCollisions(CollisionDirect.Right, CollisionDirect.Down, CollisionDirect.Up);
        animator.SetBool("Falling", counts_collisions[(int)CollisionDirect.Down] == 0 || min_collds_wthout[CollisionDirect.Down]);
        animator.SetBool("Running", direction.x != 0);
    }

    /// <summary>
    /// Update player position by Rigidbody2D
    /// </summary>
    protected override void UpdatePosition()
    {
        base.UpdatePosition();

        if (rgbody2d)
        {
            Vector2 velocity = new Vector2(rgbody2d.velocity.x + speed.x, rgbody2d.velocity.y + speed.y);
            rgbody2d.velocity = velocity;
        }
    }

    /// <summary>
    /// Method rotate player in side. 
    /// </summary>
    /// <param name="side"> side value {true: right side, false: left side} </param>
    void rotatePlayer(bool side)
    {
        int sign = side ? 1 : -1;
        transform.localScale = new Vector3(sign * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Method for getting inforamtion of specified directions, 
    /// where true value have direction that have maximum count collisions
    /// </summary>
    /// <returns> dictionary of directions </returns>
    Dictionary<CollisionDirect, bool> getDirectsMaxCollisions(params CollisionDirect[] req_directs)
    {
        return getDirectsMCollisions(true, req_directs);
    }

    /// <summary>
    /// Method for getting inforamtion of specified directions, 
    /// where true value have direction that have minimum count collisions
    /// </summary>
    /// <returns> dictionary of directions </returns>
    Dictionary<CollisionDirect, bool> getDirectsMinCollisions(params CollisionDirect[] req_directs)
    {
        return getDirectsMCollisions(false, req_directs);
    }

    /// <summary>
    /// Method for getting inforamtion of specified directions, 
    /// where true value have direction that have maximum / minimum count collisions
    /// </summary>
    /// <returns> dictionary of directions </returns>
    Dictionary<CollisionDirect, bool> getDirectsMCollisions(bool maximum, params CollisionDirect[] req_directs)
    {
        int m_ind = (int)req_directs[0];                                    // assume that the index of maximum/minimum element
        int m = counts_collisions[m_ind];
        System.Func<int, int, bool> com_op;                                 // select comparison operation
        if (maximum) 
            com_op = (max, val) => max < val;
        else 
            com_op = (min, val) => min > val;

        for (int i = 0; i < req_directs.Length; i++)                        // find element with maximum / minimum count collisions
        {
            if (com_op(m, counts_collisions[(int)req_directs[i]]))
            {
                m = counts_collisions[(int)req_directs[i]];
                m_ind = (int)req_directs[i];
            }
        }

        Dictionary<CollisionDirect, bool> dict_directs = new Dictionary<CollisionDirect, bool>();
        for (int i = 0; i < req_directs.Length; i++)
        {
            dict_directs.Add(req_directs[i], false);
        }
        dict_directs[(CollisionDirect)m_ind] = true;

        for (int i = 0; i < req_directs.Length; i++)
        {
            if ((int)req_directs[i] != m_ind && m == counts_collisions[(int)req_directs[i]])        // find all elements, that equal maximum/minimum count collisions 
            {
                dict_directs[req_directs[i]] = true;
            }
        }
        return dict_directs;
    }

    /// <summary>
    /// Method for capture peforming
    /// </summary>
    private void Capture()
    {
        if (!capturing)
        {
            if (rgbody2d) rgbody2d.constraints = RigidbodyConstraints2D.FreezeAll;
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
            if (rgbody2d)  rgbody2d.constraints = RigidbodyConstraints2D.FreezeRotation;
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

        Vector2 size2 = SizeScripts.sizeObjBy(collider2d) / 2;
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

    /// <summary>
    /// Function to check that only one element of dictionary pass check
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="dict"> dictionary </param>
    /// <param name="check"> predicate </param>
    /// <returns></returns>
    static bool onlyOneTrue<T, U>(Dictionary<T, U> dict, System.Func<KeyValuePair<T, U>, bool> check)
    {
        int count = 0;
        foreach (KeyValuePair<T, U> pair in dict)
        {
            if (check(pair))
                count++;

            if (count > 1)
                return false;
        }
        return true;
    }
}
