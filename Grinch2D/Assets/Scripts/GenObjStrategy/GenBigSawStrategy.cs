using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// GenBigSawStrategy is strategy of generation big saw
/// </summary>
public class GenBigSawStrategy : GenBy4Strategy
{
    /// <summary>
    /// Method generates big saw by prefab.
    /// </summary>
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||
            objPrefab == null || objParentField == null)
            throw new System.ArgumentNullException("levelDict || levelMap || " +
                                                   "objPrefab || objParentField");

        base.Generate();

        /* checks nearby blocks in the following way:
         *  x
         * x x
         *  x 
         * x - checked block
         */
        List<Vector2> busy_poss = getBusyPositions(objPrefab.name);
        Vector2 spawn_pos = Vector2.zero;

        switch (busy_poss.Count)
        {
            case 0:
                return;
            case 1:
                spawn_pos = busy_poss[0];
                break;
            case 2:
                List<Vector2> check_poss = new List<Vector2>();
                check_poss.Add(new Vector2(busy_poss[0].x, busy_poss[1].y));
                check_poss.Add(new Vector2(busy_poss[1].x, busy_poss[0].y));

                /* checks nearby blocks in the following way:
                 * xb   b    bx    b
                 * b   bx     b    xb 
                 *
                 * x - checked block
                 * b - block
                 */
                foreach (Vector2 check_pos in check_poss)
                {
                    string next_prefname;
                    if (!levelDict.TryGetValue(levelMap[(int)check_pos.y, (int)check_pos.x], out next_prefname))                    // check name central block (middle center, right center, left center)
                        next_prefname = emptyPrefabName;
                    if (next_prefname == emptyPrefabName || next_prefname == objPrefab.name) continue;

                    spawn_pos = check_pos;
                }
                break;

            case 3:

                /* checks nearby blocks in the following way:
                 *   x    x       x        
                 *  x      x     x x   x x 
                 *   x    x             x
                 * x - checked block
                 */
                if (busy_poss[0].x == busy_poss[1].x)
                {
                    spawn_pos = busy_poss[2];
                    break;
                }
                else if (busy_poss[1].x == busy_poss[2].x)
                {
                    spawn_pos = busy_poss[0];
                    break;
                }

                busy_poss = busy_poss.OrderBy(v => v.y).ToList();

                if (busy_poss[0].y == busy_poss[1].y)
                {
                    spawn_pos = busy_poss[2];
                }
                else if (busy_poss[1].y == busy_poss[2].y)
                {
                    spawn_pos = busy_poss[0];
                }
                break;

            case 4:
                spawn_pos = new Vector2((int)map_spwnr_pos.x, (int)map_spwnr_pos.y);
                break;

            default:
                return;
        }

        spawn_pos = new Vector2(spawn_pos.x * spwnrSize.x, spawn_pos.y * spwnrSize.y);

        GameObject saw = Instantiate(objPrefab,                                                                  // create block game object
                                     new Vector3(spawn_pos.x, spawn_pos.y, objPrefab.transform.position.z),
                                     Quaternion.identity) as GameObject;
        saw.transform.parent = objParentField.transform;
        SelfRotator selfRotator = saw.GetComponent<SelfRotator>();
        if (selfRotator == null) return;
        selfRotator.setRandDAngle();                                                                             // set random rotation

        created_objs.Add(saw);
    }
}
