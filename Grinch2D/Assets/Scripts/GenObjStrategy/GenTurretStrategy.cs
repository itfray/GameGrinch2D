using UnityEngine;


/// <summary>
/// GenTurretStrategy is strategy of generation turret
/// </summary>
public class GenTurretStrategy : GenObjStrategy
{
    public override void Generate()
    {
        if (objPrefab == null || objParentField == null)                                                            // checks needed params
            throw new System.ArgumentNullException("objPrefab || objParentField");

        GameObject turret = Instantiate(objPrefab,                                                                  // create turret game object
                               new Vector3(spwnr_pos.x, spwnr_pos.y, objPrefab.transform.position.z),
                               Quaternion.identity) as GameObject;
        turret.transform.parent = objParentField.transform;                                                         // places turret in turrets field
    }
}
