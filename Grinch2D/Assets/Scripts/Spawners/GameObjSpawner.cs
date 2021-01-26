using UnityEngine;

public abstract class GameObjSpawner : MonoBehaviour
{
    public GameObject parentField;
    public GameObject prefab;
    public Vector2 spawnPosition;

    protected GameObject spawned_obj;

    public void InitSpawner(GameObject obj_prefab, GameObject parent_field, Vector2 spawn_position)
    {
        prefab = obj_prefab;
        parentField = parent_field;
        spawnPosition = spawn_position;
    }

    public virtual void Spawn()
    {
        spawned_obj = Instantiate(prefab, new Vector3(spawnPosition.x, spawnPosition.y, prefab.transform.position.z), Quaternion.identity) as GameObject;
        spawned_obj.transform.parent = parentField.transform;

        SpawnAddition();
    }

    public virtual void Respawn()
    {
        if (spawned_obj != null)
            Destroy(spawned_obj);
        Spawn();
    }

    public virtual bool isSpawned()
    {
        return spawned_obj != null;
    }

    protected abstract void SpawnAddition();
}
