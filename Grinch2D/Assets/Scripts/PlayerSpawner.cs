using UnityEngine;

public class PlayerSpawner : GameObjSpawner
{
    protected override void SpawnAddition()
    {
        MoveCameraHandler hmove_cam = Camera.main.transform.GetComponent<MoveCameraHandler>();
        if (hmove_cam)
            hmove_cam.following = spawned_obj.transform;
    }
}
