using UnityEngine;

public class JumpHandler : MoveHandler
{
    public float ddirect = 0.1f;
    public int max_count_step = 1;

    private bool flag_jump = false;
    private int count_step;

    protected override void UpdateDirection()
    {
        if (!flag_jump) return;

        if (count_step == -(max_count_step - 1))
        {
            flag_jump = false;
            direction.y = 0;
            Debug.Log("flag_jump: " + flag_jump.ToString());
            Debug.Log("count_step: " + count_step.ToString());
            return;
        }

        count_step--;
        int sign = count_step > 0 ? 1 : -1;
        direction.y = sign * count_step * count_step * ddirect;

        Debug.Log("flag_jump: " + flag_jump.ToString());
        Debug.Log("count_step: " + count_step.ToString());
        Debug.Log("direction.y: " + direction.y.ToString());
    }

    public void Jump()
    {
        flag_jump = true;
        count_step = max_count_step;
        direction.y = 0;
    }

    public bool Jumping()
    {
        return flag_jump;
    }
}
