using UnityEngine;

public class GenSawStrategy : GenObjStrategy
{
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||
            objPrefab == null || objParentField == null)
            return;

        int row_pos = (int)map_spwnr_pos.y;
        int col_pos = (int)map_spwnr_pos.x;

        /* checks nearby blocks in the following way:
         *  x 
         * x x
         *  x 
         * x - checked block
         */

        for (int cofst = -1; cofst < 2; cofst++)
        {
            int rofst_start = 0;
            int rofst_end = 1;

            if (cofst == 0)
            {
                rofst_start = -1;
                rofst_end = 2;
            }

            for (int rofst = rofst_start; rofst < rofst_end; rofst += rofst_end)
            {
                string next_prefname;
                int rpos = row_pos + rofst;
                int cpos = col_pos + cofst;

                if (rpos < 0 || rpos >= mapSize.y || cpos < 0 || cpos >= mapSize.x) continue;

                if (!levelDict.TryGetValue(levelMap[rpos, cpos], out next_prefname))                                      // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname == emptyPrefabName || next_prefname == objPrefab.name) continue;

                Vector2 spawn_pos = new Vector2(spwnr_pos.x + cofst * spwnrSize.x, spwnr_pos.y - rofst * spwnrSize.y);
                GameObject saw = Instantiate(objPrefab,                                                                        // create block game object
                                         new Vector3(spawn_pos.x, spawn_pos.y, objPrefab.transform.position.z),
                                         Quaternion.identity) as GameObject;
                saw.transform.parent = objParentField.transform;
            }
        }
    }
}
