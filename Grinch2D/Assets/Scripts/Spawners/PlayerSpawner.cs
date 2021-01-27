using UnityEngine;

/// <summary>
/// GameObjSpawner is class of spawner of player.
/// </summary>
public class PlayerSpawner : GameObjSpawner
{
    private MoveCameraHandler move_cam_hnd;                             // handler of camera moving

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
            Spawn();
    }

    /// <summary>
    /// Method for spawning player
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        if (move_cam_hnd == null)
            move_cam_hnd = Camera.main.transform.GetComponent<MoveCameraHandler>();
        move_cam_hnd.following = spawned_obj.transform;                                 // do that main camera follow on the player
    }
}
