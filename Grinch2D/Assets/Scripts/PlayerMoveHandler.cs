using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveHandler : JumpHandler
{
    protected override void UpdateDirection()
    {
        float inputx = Input.GetAxis("Horizontal");
        direction.x = inputx;
        /*float inputy = Input.GetAxis("Vertical");
        direction.y = inputy;*/

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        base.UpdateDirection();
    }

    protected override void UpdatePosition()
    {
        Rigidbody2D rgbody = GetComponent<Rigidbody2D>();
        rgbody.MovePosition(rgbody.position + speed * Time.deltaTime);
    }
}
