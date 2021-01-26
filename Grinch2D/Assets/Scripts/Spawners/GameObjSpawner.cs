using UnityEngine;

public abstract class GameObjSpawner : MonoBehaviour
{
    public GameObject parentField;
    public GameObject prefab;
    public Vector2 spawnPosition;

    protected GameObject spawned_obj;

    public GameObject spawnedObj { get { return spawned_obj; } }

    public void InitSpawner(GameObject obj_prefab, GameObject parent_field, Vector2 spawn_position)
    {
        prefab = obj_prefab;
        parentField = parent_field;
        spawnPosition = spawn_position;
    }

    public virtual void Spawn()
    {
        Vector3 spawn_pos = new Vector3(spawnPosition.x, spawnPosition.y, prefab.transform.position.z);
        if (spawned_obj == null)
            spawned_obj = Instantiate(prefab, spawn_pos, Quaternion.identity, parentField.transform) as GameObject;
        else
            spawned_obj.transform.position = spawn_pos;

        SpawnAddition();
    }

    protected abstract void SpawnAddition();
}
