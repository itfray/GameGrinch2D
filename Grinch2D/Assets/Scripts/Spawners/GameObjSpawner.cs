using UnityEngine;

/// <summary>
/// GameObjSpawner is class of spawner of game objects.
/// </summary>
public class GameObjSpawner : MonoBehaviour
{
    public GameObject parentField;                                                                      // parent filed of generated game object
    public GameObject prefab;                                                                           // prefab of generated game object
    public Vector3 spawnPosition;                                                                       // spawn position of generated game object

    protected GameObject spawned_obj;                                                                   // generated game object

    public GameObject spawnedObj { get { return spawned_obj; } }                                        // generated game object

    /// <summary>
    /// Method for initialization spawner
    /// </summary>
    /// <param name="obj_prefab"> prefab of generated game object </param>
    /// <param name="parent_field"> parent filed of generated game object </param>
    /// <param name="spawn_position"> spawn position of generated game object </param>
    public void InitSpawner(GameObject obj_prefab, GameObject parent_field, Vector2 spawn_position)
    {
        prefab = obj_prefab;
        parentField = parent_field;
        spawnPosition = new Vector3(spawn_position.x, spawn_position.y, prefab.transform.position.z);
    }

    /// <summary>
    /// Method for creating game object 
    /// </summary>
    public virtual void Create()
    {
        spawned_obj = Instantiate(prefab, new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z),
                                          Quaternion.identity, parentField.transform) as GameObject;
    }

    /// <summary>
    /// Method for spawning game object 
    /// </summary>
    public virtual void Spawn()
    {
        spawned_obj.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
    }
}
