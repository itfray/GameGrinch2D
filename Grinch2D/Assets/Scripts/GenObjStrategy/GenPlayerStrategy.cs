using UnityEngine;


/// <summary>
/// GenPlayerStrategy is strategy of generation player
/// </summary>
public class GenPlayerStrategy : GenObjStrategy
{
    /// <summary>
    /// Method generates player spawner block by prefab of player and initiates player creation.
    /// </summary>
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||                                                                           // checks needed params
            spwnrPrefab == null || objPrefab == null ||
            spwnrParentField == null || objParentField == null)
            throw new System.ArgumentNullException("levelDict || levelMap || spwnrPrefab ||" +
                                                  " objPrefab || spwnrParentField || objParentField");

        base.Generate();

        int row_pos = (int)map_spwnr_pos.y;
        int col_pos = (int)map_spwnr_pos.x;

        GameObject spwnr_obj = Instantiate(spwnrPrefab,                                                                          // create spawner object
                                          new Vector3(spwnr_pos.x, spwnr_pos.y, spwnrPrefab.transform.position.z),
                                          Quaternion.identity) as GameObject;
        spwnr_obj.transform.parent = spwnrParentField.transform;
        created_spwnrs.Add(spwnr_obj);

        Vector2 spawn_pos = Vector2.zero;                                                                                        // spawn position of player
        Vector2 player_size = SizeScripts.sizeObjBy(objPrefab.GetComponent<BoxCollider2D>());

        /* checks nearby blocks in the following way:
         * xxx
         * x x
         * xxx
         * x - checked block
         */
        bool found_spwn_pos = false;                                                                         // spawn position was finded?
        for (int cofst = 0; cofst < 3 && !found_spwn_pos; cofst++)
        {
            for (int rofst = -1; rofst < 2; rofst += 2)
            {
                string next_prefname;
                int rpos = row_pos;
                int cpos = col_pos;
                int col_ofst = (cofst < 2 ? cofst : -1);

                cpos += col_ofst;
                if (cpos < 0 || cpos >= mapSize.x) continue;

                if (!levelDict.TryGetValue(levelMap[rpos, cpos], out next_prefname))                          // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname != emptyPrefabName && cpos != col_pos) continue;                             // if central block is empty

                rpos += rofst;
                if (rpos < 0 || rpos >= mapSize.y) continue;

                if (!levelDict.TryGetValue(levelMap[rpos, cpos], out next_prefname))                           // check top/bottom block (middle top/bottom, right top/bottom, left top/bottom)
                    next_prefname = emptyPrefabName;

                if (next_prefname != emptyPrefabName) continue;

                spawn_pos.x = spwnr_pos.x + col_ofst * spwnrSize.x;                                           // calculate spawn position
                spawn_pos.y = spwnr_pos.y - rofst * spwnrSize.y / 2 + rofst * player_size.y / 2;
                found_spwn_pos = true;
                break;
            }
        }
        if (!found_spwn_pos) return;

        PlayerSpawner player_spwnr = spwnr_obj.GetComponent<PlayerSpawner>();
        if (player_spwnr)
        {
            player_spwnr.InitSpawner(objPrefab, objParentField, spawn_pos);                                                   // init player spawner
            player_spwnr.Create();                                                                                            // create player object
            created_objs.Add(player_spwnr.spawnedObj);
        }
    }
}
