using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    public string triggerName = "isExploded";
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Explode()
    {
        Explode(new Vector2(transform.position.x, transform.position.y));
    }

    public void Explode(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        animator.SetTrigger(triggerName);
    }
}
