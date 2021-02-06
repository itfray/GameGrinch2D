using UnityEngine;

/// <summary>
/// GenBlockStrategy is strategy of generation block
/// </summary>
public class GenBlockStrategy : GenObjStrategy
{
    /// <summary>
    /// Method generates block by prefab.
    /// </summary>
    public override void Generate()
    {
        if (objPrefab == null || objParentField == null)                                                           // checks needed params
            throw new System.ArgumentNullException("objPrefab || objParentField");

        base.Generate();

        GameObject block = Instantiate(objPrefab,                                                                  // create block game object
                               new Vector3(spwnr_pos.x, spwnr_pos.y, objPrefab.transform.position.z),
                               Quaternion.identity) as GameObject;
        block.transform.parent = objParentField.transform;                                                         // places block in blocks field

        created_objs.Add(block);
    }
}
