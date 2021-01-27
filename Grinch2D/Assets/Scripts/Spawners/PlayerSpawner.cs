using UnityEngine;

/// <summary>
/// GameObjSpawner is class of spawner of player.
/// </summary>
public class PlayerSpawner : GameObjSpawner
{
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

        MoveCameraHandler hmove_cam = Camera.main.transform.GetComponent<MoveCameraHandler>();
        if (hmove_cam) hmove_cam.following = spawned_obj.transform;                                         // do that main camera follow on the player
    }
}
