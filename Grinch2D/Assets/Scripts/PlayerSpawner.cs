using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private GameObject playerField;
    private GameObject playerPrefab;
    private GameObject playerObj;
    private Vector2 spawnPosition;

    void InitSpawn(GameObject player_prefab, GameObject player_field, Vector2 spawn_position)
    {
        playerPrefab = player_prefab;
        playerField = player_field;
        spawnPosition = spawn_position;
    }

    void Spawn()
    {
        playerObj = Instantiate(playerPrefab, new Vector3(spawnPosition.x, spawnPosition.y, playerPrefab.transform.position.z), Quaternion.identity) as GameObject;
        playerObj.transform.parent = playerField.transform;

        MoveCameraHandler hmove_cam = Camera.main.transform.GetComponent<MoveCameraHandler>();
        if (hmove_cam)
            hmove_cam.following = playerObj.transform;
    }
}
