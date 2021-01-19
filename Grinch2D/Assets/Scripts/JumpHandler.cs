﻿using UnityEngine;


/// <summary>
/// class JumpHandler is handler to control jump game object.
/// </summary>
public abstract class JumpHandler : MoveHandler
{
    public float ddirect = 0.00025f;                        // direction derivative (direction change step)
    public float max_ddirect = 0.032f;                      // max value ddirect

    private int count_step;                                 // number of step to peform jump
    private bool jumping = false;                           // game object peforms jump?

    /// <summary>
    /// Method UpdateDirection pefrom jump
    /// </summary>
    protected override void UpdateDirection()
    {
        if (!jumping) return;

        if (count_step <= 0)
        {
            jumping = false;
            direction.y = 0;
            return;
        }

        direction.y = count_step * count_step * ddirect;            // parabolic change of direction
        count_step--;
    }

    /// <summary>
    /// Method Jump initiate game object jump.
    /// </summary>
    public void Jump()
    {
        jumping = true;
        count_step = Mathf.RoundToInt(max_ddirect / ddirect);
        direction.y = 0;
    }
}
