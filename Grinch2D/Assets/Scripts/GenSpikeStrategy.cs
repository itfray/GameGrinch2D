﻿using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// GenSpikeStrategy is strategy of generation spike
/// </summary>
public class GenSpikeStrategy : GenBy4Strategy
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

        Vector2 spike_size = SizeScripts.sizeObjByBoxCollider2D(objPrefab);
        int row_pos = (int)map_spwnr_pos.y;
        int col_pos = (int)map_spwnr_pos.x;

        /* checks nearby blocks in the following way:
         *  x 
         * x x
         *  x 
         * x - checked block
         */
        foreach (Vector2 busy_pos in getBusyPositions(objPrefab.name))
        {
            Vector2 spawn_pos = Vector2.zero;
            spawn_pos.x = busy_pos.x * spwnrSize.x;
            spawn_pos.x += (col_pos - busy_pos.x) * (spwnrSize.x / 2 + spike_size.y / 2);
            spawn_pos.y = ((int)mapSize.y - 1 - busy_pos.y) * spwnrSize.y;
            spawn_pos.y -= (row_pos - busy_pos.y) * (spwnrSize.y / 2 + spike_size.y / 2);

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
