using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHandler : MonoBehaviour
{
    public GameObject lockBlockPrefab;
    public GameObject lockBlockParent;

    private List<GameObject> lock_blocks = new List<GameObject>();

    public void CreateLockBlocks(List<Vector2> positions)
    {
        if (lockBlockPrefab == null) return;

        foreach (Vector2 pos in positions)
        {
            lock_blocks.Add(Instantiate(lockBlockPrefab, pos, Quaternion.identity, lockBlockParent.transform));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
