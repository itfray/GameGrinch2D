using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenKeyStrategy : GenObjStrategy
{
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||                                                                           // checks needed params
            spwnrPrefab == null || spwnrParentField == null ||
            objParentField == null)
            throw new System.ArgumentNullException("levelDict || levelMap || spwnrPrefab ||" +
                                                  " spwnrParentField || objParentField");

        int row_pos = (int)map_spwnr_pos.y;
        int col_pos = (int)map_spwnr_pos.x;

        GameObject spwnr_obj = Instantiate(spwnrPrefab,                                                                          // create spawner object
                                  new Vector3(spwnr_pos.x, spwnr_pos.y, spwnrPrefab.transform.position.z),
                                  Quaternion.identity) as GameObject;
        spwnr_obj.transform.parent = spwnrParentField.transform;

        // some code ...

        List<Vector2> lock_blocks_poss = new List<Vector2>();

        for (int iy = 0; iy < (int)mapSize.y; iy++)
        {
            for (int jx = 0; jx < (int)mapSize.x; jx++)
            {
                string next_prefname;
                if (!levelDict.TryGetValue(levelMap[iy, jx], out next_prefname))                                                // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;
                if (next_prefname != spwnrPrefab.name && next_prefname.Contains(spwnrPrefab.name))
                {
                    lock_blocks_poss.Add(new Vector2(jx * spwnrSize.x, ((int)mapSize.y - 1 - iy) * spwnrSize.y));
                }
            }
        }


    }
}
