﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthHnd : HealthHandler
{
    private Animator animator;

    void Update()
    {
        if (blood_spltr)
        {
            blood_spltr.transform.position = new Vector3(transform.position.x, transform.position.y,
                                                         blood_spltr.transform.position.z);
        }
    }

    protected override void Init()
    {
        base.Init();

        animator = GetComponent<Animator>();
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        animator.SetTrigger("Hitting");
    }

    protected override void Die()
    {
/*      MoveCameraHandler mvcam_hnd = Camera.main.GetComponent<MoveCameraHandler>();
        if (mvcam_hnd) mvcam_hnd.following = null;*/
    }
}
