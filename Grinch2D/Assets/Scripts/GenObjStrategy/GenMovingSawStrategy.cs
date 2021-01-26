using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// GenMovingStrategy is strategy of generation moving saw
/// </summary>
public class GenMovingSawStrategy : GenObjStrategy
{
    public override void Generate()
    {
        if (levelDict == null || levelMap == null ||
            objPrefab == null || objParentField == null)
            throw new System.ArgumentNullException("levelDict || levelMap || " +
                                                   "objPrefab || objParentField");

        int row_pos = (int)map_spwnr_pos.y;
        int col_pos = (int)map_spwnr_pos.x;

        Vector2 other_pos = spwnr_pos;

        /*checks nearby blocks in the following way:
         *    x
         *    x
         *    x
         * xxxoxxx
         *    x
         *    x
         *    x
         * x - checked block
        */
        List<Vector2> checked_poss = new List<Vector2>();
        for (int j = col_pos, i = row_pos + 1; i < (int)mapSize.y; i++)                 // check down positions
            checked_poss.Add(new Vector2(j, i));

        for (int j = col_pos, i = row_pos - 1; i >= 0; i--)                             // check up positions
            checked_poss.Add(new Vector2(j, i));

        for (int i = row_pos, j = col_pos + 1; j < (int)mapSize.x; j++)                 // check right positions
            checked_poss.Add(new Vector2(j, i));

        for (int i = row_pos, j = col_pos - 1; j >= 0; j--)                             // check left positions
            checked_poss.Add(new Vector2(j, i));

        bool has_other_pos = false;
        foreach (Vector2 checked_pos in checked_poss)                                   // search other postion
        {
            string next_prefname;
            if (!levelDict.TryGetValue(levelMap[(int)checked_pos.y, (int)checked_pos.x], out next_prefname))                    // check name central block (middle center, right center, left center)
                next_prefname = emptyPrefabName;
            if (next_prefname != objPrefab.name && next_prefname.Contains(objPrefab.name))
            {
                has_other_pos = true;
                other_pos = new Vector2((int)checked_pos.x, (int)checked_pos.y);
                break;
            }
        }

        Vector2 min_pos = spwnr_pos;
        Vector2 max_pos = spwnr_pos;
        Vector2 direct = Vector2.zero;

        if (has_other_pos)              // select min saw postion and max saw postion
        {
            other_pos = new Vector2(other_pos.x * spwnrSize.x, ((int)mapSize.y - 1 - other_pos.y) * spwnrSize.y);
            if (spwnr_pos.y > other_pos.y)
            {
                min_pos = other_pos;
                direct.y = 1;
            }
            else if (spwnr_pos.y < other_pos.y)
            {
                max_pos = other_pos;
                direct.y = 1;
            }
            else if (spwnr_pos.x > other_pos.x)
            {
                min_pos = other_pos;
                direct.x = 1;
            }
            else if (spwnr_pos.x < other_pos.x)
            {
                max_pos = other_pos;
                direct.x = 1;
            }
        }

        GameObject moving_saw = Instantiate(objPrefab,                                                                         // creates moving saw game object
                                            new Vector3(spwnr_pos.x, spwnr_pos.y, objPrefab.transform.position.z),
                                            Quaternion.identity) as GameObject;
        moving_saw.transform.parent = objParentField.transform;

        SelfRotator selfRotator = moving_saw.GetComponent<SelfRotator>();
        selfRotator?.setRandDAngle();                                                       // set random value for angle of rotation

        MoveSawHandler moveSawHnd = moving_saw.GetComponent<MoveSawHandler>();
        if (moveSawHnd == null) return;
        moveSawHnd.minPosition = min_pos;
        moveSawHnd.maxPosition = max_pos;
        moveSawHnd.direction = direct;
        moveSawHnd.setRandSpeed();                                                          // set random value speed of moving
    }
}
