using UnityEngine;

/// <summary>
/// GameObjSpawner is class of spawner of player.
/// </summary>
public class PlayerSpawner : GameObjSpawner
{
    private MoveCameraHandler move_cam_hnd;                             // handler of camera moving
    private PlayerHealthHnd health_hnd;
    private StarHandler star_hnd;

    public PlayerHealthHnd PlayerHealthHnd { get { return health_hnd;  } }
    public StarHandler PlayerStarHnd { get { return star_hnd; } }

    public override void Create()
    {
        base.Create();

        health_hnd = spawned_obj.GetComponent<PlayerHealthHnd>();
        star_hnd = spawned_obj.GetComponent<StarHandler>();
        move_cam_hnd = Camera.main.transform.GetComponent<MoveCameraHandler>();
        move_cam_hnd.following = spawned_obj.transform;                                 // do that main camera follow on the player
        spawned_obj.SetActive(false);
    }

    /// <summary>
    /// Method for spawning player
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        health_hnd.InitHealth();
        star_hnd.ResetCounter();

        spawned_obj.SetActive(true);
    }
}
