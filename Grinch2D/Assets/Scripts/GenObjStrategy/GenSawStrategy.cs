using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// GenSawStrategy is strategy of generation little saw
/// </summary>
public class GenSawStrategy : GenBy4Strategy
{
    /// <summary>
    /// Method generates saw by prefab.
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
        foreach (Vector2 busy_pos in getBusyPositions(objPrefab.name))
        {
            Vector2 spawn_pos = new Vector2(busy_pos.x * spwnrSize.x, busy_pos.y * spwnrSize.y);
            GameObject saw = Instantiate(objPrefab,                                                                         // creates saw game object
                                         new Vector3(spawn_pos.x, spawn_pos.y, objPrefab.transform.position.z),
                                         Quaternion.identity) as GameObject;
            saw.transform.parent = objParentField.transform;

            created_objs.Add(saw);
        }
    }
}
