using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// GenObjStrategy is strategy of generation game object
/// </summary>
public abstract class GenObjStrategy : MonoBehaviour
{
    public GameObject objPrefab { set; get; }                       // main prefab for generation of object
    public GameObject objParentField { set; get; }                  // parent field for generation of object

    protected List<GameObject> created_objs = new List<GameObject>();

    public List<GameObject> createdObjs { get { return created_objs; } }

    public string emptyPrefabName { get; set; }                     // text name of empty prefab

    public GameObject spwnrPrefab { set; get; }                     // spawner prefab for generation of spawner object for generation of main game object
    public GameObject spwnrParentField { set; get; }                // parent field for generation spawner object

    protected List<GameObject> created_spwnrs = new List<GameObject>();

    public List<GameObject> createdSpwnrs { get { return created_spwnrs; } }

    public Vector2 spwnrSize { set; get; }                          // size spawner block

    public Dictionary<char, string> levelDict { set; get; }         // reference on level dictionary
    public char[,] levelMap { set; get; }                           // reference on level map
    public Vector2 mapSize { set; get; }                            // level map size 

    protected Vector2 map_spwnr_pos;                                // object spawner position on map
    protected Vector2 spwnr_pos;                                    // object spawner position on game scene

    /// <summary>
    /// Set main params for work of strategy
    /// </summary>
    /// <param name="level_dict"> level dictionary </param>
    /// <param name="level_map"> level map </param>
    /// <param name="map_size"> level map size </param>
    /// <param name="empty_pref_name"> text name of empty prefab </param>
    /// <param name="spwnr_size"> size spawner block </param>
    public void setParams(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size, string empty_pref_name, Vector2 spwnr_size)
    {
        setMapParams(level_dict, level_map, map_size, empty_pref_name);
        spwnrSize = spwnr_size;
    }

    /// <summary>
    /// Method generates game object by prefab and spawner prefab.
    /// </summary>
    public virtual void Generate()
    {
        created_objs.Clear();
        created_spwnrs.Clear();
    }

    public void setMapParams(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size, string empty_pref_name)
    {
        levelDict = level_dict;
        levelMap = level_map;
        mapSize = map_size;
        emptyPrefabName = empty_pref_name;
    }

    /// <summary>
    /// Set position spawner block on level map.
    /// </summary>
    /// <param name="row_pos"> row index in level map </param>
    /// <param name="col_pos"> column inde in level map </param>
    public void setSpwnrPosInMap(int row_pos, int col_pos)
    {
        setSpwnrPosInMap(new Vector2(col_pos, row_pos));
    }

    /// <summary>
    /// Set position spawner block on level map.
    /// </summary>
    /// <param name="map_pos"> Vector(column index in level map, row index in level map) </param>
    public void setSpwnrPosInMap(Vector2 map_pos)
    {
        map_spwnr_pos = map_pos;
        spwnr_pos = new Vector2(map_spwnr_pos.x * spwnrSize.x, map_spwnr_pos.y * spwnrSize.y);            // calculate position on game scene
    }
}
