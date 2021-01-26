using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// GenBlockStrategy is strategy of generation game object by 4 nearby blocks
/// </summary>
public abstract class GenBy4Strategy : GenObjStrategy
{
    protected List<Vector2> getBusyPositions(params string[] excldPrefNames)
    {
        List<Vector2> busy_poss = new List<Vector2>();

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
            int rofst_start = 0;                                                                        // interval boundaries
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

                if (rpos < 0 || rpos >= mapSize.y || cpos < 0 || cpos >= mapSize.x) continue;           // checks validity of coords

                if (!levelDict.TryGetValue(levelMap[rpos, cpos], out next_prefname))                    // checks name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname == emptyPrefabName) continue;                                         // checks busy block

                bool next_iter = false;                                                                 // exlude specified prefab names
                foreach (string exld_prefname in excldPrefNames)
                {
                    if (next_prefname == exld_prefname)
                    {
                        next_iter = true;
                    }
                }
                if (next_iter) continue;

                busy_poss.Add(new Vector2(cpos, rpos));                                                 // add in busy list
            }
        }

        return busy_poss;
    }
}
