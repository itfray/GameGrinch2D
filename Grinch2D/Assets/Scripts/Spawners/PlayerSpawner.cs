using UnityEngine;

public class PlayerSpawner : GameObjSpawner
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
            Spawn();
    }

    protected override void SpawnAddition()
    {
        MoveCameraHandler hmove_cam = Camera.main.transform.GetComponent<MoveCameraHandler>();
        if (hmove_cam)
            hmove_cam.following = spawned_obj.transform;
    }
}
