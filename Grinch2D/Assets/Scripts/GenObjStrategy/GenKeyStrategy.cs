using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GenKeyStrategy is strategy of generation key
/// </summary>
public class GenKeyStrategy : GenObjStrategy
{
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||                                                                           // checks needed params
            spwnrPrefab == null || spwnrParentField == null ||
            objParentField == null)
            throw new System.ArgumentNullException("levelDict || levelMap || spwnrPrefab ||" +
                                                  " spwnrParentField || objParentField");

        base.Generate();

        GameObject key_obj = Instantiate(spwnrPrefab,                                                                          // create key
                                         new Vector3(spwnr_pos.x, spwnr_pos.y, spwnrPrefab.transform.position.z),
                                         Quaternion.identity) as GameObject;
        key_obj.transform.parent = spwnrParentField.transform;                                                                 // set parent field for key
        created_spwnrs.Add(key_obj);

        List<Vector2> lock_blocks_poss = new List<Vector2>();                                                                  // search lock block positions
        for (int iy = 0; iy < (int)mapSize.y; iy++)
        {
            for (int jx = 0; jx < (int)mapSize.x; jx++)
            {
                string next_prefname;
                if (!levelDict.TryGetValue(levelMap[iy, jx], out next_prefname))
                    next_prefname = emptyPrefabName;
                if (next_prefname != spwnrPrefab.name && next_prefname.Contains(spwnrPrefab.name))                             // if name is like key name
                    lock_blocks_poss.Add(new Vector2(jx * spwnrSize.x, iy * spwnrSize.y));
            }
        }

        KeyHandler key_hnd = key_obj.GetComponent<KeyHandler>();
        if (key_hnd)
        {
            key_hnd.lockBlockParent = objParentField;                                                                          // set parent field for locked blocks
            key_hnd.OnCreateLockBlocks += delegate
            {
                foreach (GameObject block in key_hnd.lockedBlocks)
                {
                    created_objs.Add(block);
                }
            };
            key_hnd.CreateLockBlocks(lock_blocks_poss);                                                                        // create blocks by positions
        }
    }
}
