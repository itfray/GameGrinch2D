using UnityEngine;

public class JumpHandler : MoveHandler
{
    public float ddirect = 0.00025f;
    public float max_ddirect = 0.03f;

    private int count_step;
    private bool jumping = false;

    protected override void UpdateDirection()
    {
        if (!jumping) return;

        if (count_step <= 0)
        {
            jumping = false;
            direction.y = 0;
            return;
        }

        direction.y = count_step * count_step * ddirect;
        count_step--;
    }

    public void Jump()
    {
        jumping = true;
        count_step = Mathf.RoundToInt(max_ddirect / ddirect);
        direction.y = 0;
    }
}
