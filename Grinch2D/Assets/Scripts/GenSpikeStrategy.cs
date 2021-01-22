using UnityEngine;


/// <summary>
/// GenSpikeStrategy is strategy of generation spike
/// </summary>
public class GenSpikeStrategy : GenObjStrategy
{
    /// <summary>
    /// Method generates spike by prefab.
    /// </summary>
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||
            objPrefab == null || objParentField == null)
            throw new System.ArgumentNullException("levelDict || levelMap || " +
                                                   "objPrefab || objParentField");

        int row_pos = (int)map_spwnr_pos.y;
        int col_pos = (int)map_spwnr_pos.x;

        Vector2 spike_size = SizeScripts.sizeObjByBoxCollider2D(objPrefab);

        /* checks nearby blocks in the following way:
         *  x 
         * x x
         *  x 
         * x - checked block
         */
        for (int cofst = -1; cofst < 2; cofst++)
        {
            int rofst_start = 0;                                                                                               // interval boundaries
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

                if (rpos < 0 || rpos >= mapSize.y || cpos < 0 || cpos >= mapSize.x) continue;                                   // checks validity of coords

                if (!levelDict.TryGetValue(levelMap[rpos, cpos], out next_prefname))                                            // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname == emptyPrefabName || next_prefname == objPrefab.name) continue;                              // checks busy block

                Vector2 spawn_pos = Vector2.zero;
                spawn_pos.x = spwnr_pos.x + cofst * (spwnrSize.x / 2 - spike_size.y / 2);
                spawn_pos.y = spwnr_pos.y - rofst * (spwnrSize.y / 2 - spike_size.y / 2);

                GameObject spike = Instantiate(objPrefab,                                                                         // creates saw game object
                                               new Vector3(spawn_pos.x, spawn_pos.y, objPrefab.transform.position.z),
                                               Quaternion.identity) as GameObject;
                spike.transform.parent = objParentField.transform;

                int angle = 0;
                if (spawn_pos.x > spwnr_pos.x)
                    angle = 90;
                else if (spawn_pos.x < spwnr_pos.x)
                    angle = -90;
                else if (spawn_pos.y > spwnr_pos.y)
                    angle = 180;
                spike.transform.eulerAngles = Vector3.forward * angle;                                                              // correctly rotate spike
            }
        }
    }
}
