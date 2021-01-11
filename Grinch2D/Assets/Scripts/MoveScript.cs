using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    Vector2 speed = new Vector2(10, 10);

    void Update()
    {
        Vector3 movement = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (Input.GetKey(KeyCode.LeftArrow))
            movement.x -= speed.x * Time.deltaTime;

        if (Input.GetKey(KeyCode.RightArrow))
            movement.x += speed.x * Time.deltaTime;

        if (Input.GetKey(KeyCode.DownArrow))
            movement.y -= speed.y * Time.deltaTime;

        if (Input.GetKey(KeyCode.UpArrow))
            movement.y += speed.y * Time.deltaTime;

        transform.position = movement;
    }
}
