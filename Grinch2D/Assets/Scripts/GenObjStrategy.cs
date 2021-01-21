using System.Collections.Generic;
using UnityEngine;

public abstract class GenObjStrategy : MonoBehaviour
{
    public GameObject objPrefab { set; get; }
    public GameObject objParentField { set; get; }

    public string emptyPrefabName { get; set; }

    public GameObject spwnrPrefab { set; get; }
    public GameObject spwnrParentField { set; get; }
    public Vector2 spwnrSize { set; get; }

    public Dictionary<char, string> levelDict { set; get; }
    public char[,] levelMap { set; get; }
    public Vector2 mapSize { set; get; }

    protected Vector2 map_spwnr_pos;
    protected Vector2 spwnr_pos;

    public void Init(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size, string empty_pref_name, Vector2 spwnr_size)
    {
        setMapParams(level_dict, level_map, map_size, empty_pref_name);
        spwnrSize = spwnr_size;
    }

    public abstract void Generate();

    public void setMapParams(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size, string empty_pref_name)
    {
        levelDict = level_dict;
        levelMap = level_map;
        mapSize = map_size;
        emptyPrefabName = empty_pref_name;
    }

    public void setSpwnrPosInMap(int row_pos, int col_pos)
    {
        setSpwnrPosInMap(new Vector2(col_pos, row_pos));
    }


    public void setSpwnrPosInMap(Vector2 map_pos)
    {
        map_spwnr_pos = map_pos;
        spwnr_pos = new Vector2(map_spwnr_pos.x * spwnrSize.x, ((int)mapSize.y - 1 - map_spwnr_pos.y) * spwnrSize.y);            // calculate position for instantiate prefab object
    }
}
