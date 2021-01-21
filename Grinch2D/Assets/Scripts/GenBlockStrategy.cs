using UnityEngine;

public class GenBlockStrategy : GenObjStrategy
{
    public override void Generate()
    {
        if (objPrefab == null || objParentField == null) return;

        GameObject block = Instantiate(objPrefab,                                                                  // create block game object
                               new Vector3(spwnr_pos.x, spwnr_pos.y, objPrefab.transform.position.z),
                               Quaternion.identity) as GameObject;
        block.transform.parent = objParentField.transform;                                                         // places block in blocks field
    }
}
