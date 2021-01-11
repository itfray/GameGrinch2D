using UnityEngine;
using System.Collections.Generic;



/// <summary>
/// class GameSceneHandler
/// is init all action on game scene (generate level on game scene, etc.)
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    public GameObject bgField;                              // field for created backgrounds
    public GameObject bgSample;                             // background sample with box colider2d for generating other backgrounds
    public GameObject[] bgPrefabs;                                  // background prefab samples

    public GameObject blocksField;                                  // field for created blocks
    public GameObject blockSample;                                  // block sample with box colider2d for generating other blocks
    public GameObject[] blockPrefabs;                               // block prefab samples
    public Vector2 maxLevelSize;                                    // max level height (count blocks)

    public string levelDictPath;                                    // path for level dictionary file
    public string bgLevelDictPath;                                  // path for background level dictionary file
    public string levelsPath;                                       // path for levels directory
    private LevelFileParser fileParser;                             // parser level files

    private int current_level = 1;                                  // index current running level

    public int currentLevel
    {
        get { return currentLevel; }
        set { ConstructLevel(value); }
    }

    void Start()
    {
        fileParser = GetComponent<LevelFileParser>();
        currentLevel = 1;
    }

    /// <summary>
    /// Method create level in gamescene by level files and level dictionary file
    /// </summary>
    /// <param name="level"> level number </param>
    public void ConstructLevel(int level)
    {
        current_level = level;
        fileParser.parseLevelDict(levelDictPath);
        fileParser.parseBgLevelDict(bgLevelDictPath);
        fileParser.parseLevelFile(levelsPath, level, (int)maxLevelSize.y, (int)maxLevelSize.x);
        CreateLevelObjsByMap(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize);
        CreateLevelBackground(fileParser.backgroundDict, fileParser.levelBackground, fileParser.mapSize);
    }

    /// <summary>
    /// Method is create background for level.
    /// 
    /// </summary>
    /// <param name="bg_dict"> background dictionary </param>
    /// <param name="bg_sign"> background symbol </param>
    /// <param name="map_blocks_size"> map size in number of blocks </param>
    private void CreateLevelBackground(Dictionary<char, string> bg_dict, char bg_sign, Vector2 map_blocks_size)
    {
        string bgName;
        if (!bg_dict.TryGetValue(bg_sign, out bgName))                                                  // get background prefab name from background dictionary
            Debug.LogError("Uncorrect level background symbol!!!");

        Vector2 blockSmplSize = sizeObjByBoxCollider2D(blockSample);                                    // get size sample prefabs by BoxCollider2D component
        Vector2 bgSmplSize = sizeObjByBoxCollider2D(bgSample);

        float mapCenterY = blockSmplSize.y * (map_blocks_size.y - 1) / 2;                               // calculate map center 'Y' position

        float fcbg_from_center = (Camera.main.transform.position.y - mapCenterY) / bgSmplSize.y;        // calculate offset current background in number of backgrounds between main camera and central background
        int cbg_from_center = fcbg_from_center >= 0 ?                                                   // int offset current background in number of backgrounds
                              Mathf.CeilToInt(fcbg_from_center) : Mathf.FloorToInt(fcbg_from_center);

        List<GameObject> level_bgs = new List<GameObject>();                                            // get list backgrounds on this level
        foreach (GameObject bg_prefab in bgPrefabs)
            if (bg_prefab.name.Contains(bgName))
                level_bgs.Add(bg_prefab);

        int ind_mid_bg = level_bgs.Count / 2;                                                           // get central background index in list
        if (level_bgs.Count % 2 == 0) ind_mid_bg -= 1;

        int ind_bg = ind_mid_bg + cbg_from_center;                                                      // get current background index

        int istart = -1;
        int iend = 2;

        if (fcbg_from_center > 0)
            iend -= 1;
        else
            istart += 1;

        // Create big background ( 2 x 2 backgrounds in front of the main camera )
        for (int i = istart; i < iend; i++)
        {
            int ind_bgi = ind_bg + i;
            GameObject prefab;
            if (ind_bgi < 0)                                    // if (background index < 0) then repeat background first
                prefab = level_bgs[0];
            else if (ind_bgi >= level_bgs.Count)                // if (background index >= count backgrounds) then repeat background last
                prefab = level_bgs[level_bgs.Count - 1];
            else
                prefab = level_bgs[ind_bgi];

            // Create 2 backgrounds in a line
            for (int j = -1; j < 1; j++)
            {
                // Create background on game scene
                GameObject bg = Instantiate(prefab,
                    new Vector3(Camera.main.transform.position.x + j * bgSmplSize.x + bgSmplSize.x / 2,
                                mapCenterY + (cbg_from_center + i) * bgSmplSize.y, 0), Quaternion.identity) as GameObject;
                bg.transform.parent = bgField.transform;
                Destroy(bg.GetComponent<BoxCollider2D>());
            }
        }
    }

    /// <summary>
    /// Method create level objects by level dictionary and level map
    /// </summary>
    /// <param name="level_dict"> level dictionary </param>
    /// <param name="level_map"> level map </param>
    /// <param name="map_size"> level map size </param>
    private void CreateLevelObjsByMap(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size)
    {
        if (blockPrefabs.Length <= 0) return;

        Vector2 blockSmplSize = sizeObjByBoxCollider2D(blockSample);

        for (int i = (int)map_size.y - 1; i >= 0; i--)                                      // reverse step, because file with map was readed top down
        {
            for (int j = 0; j < (int)map_size.x; j++)
            {
                string prefabName;
                if (!level_dict.TryGetValue(level_map[i, j], out prefabName)) continue;     // get prefab name of level dictionary

                float x = j * blockSmplSize.x;                                              // calculate position for instantiate prefab object
                float y = ((int)map_size.y - 1 - i) * blockSmplSize.y;

                GameObject block = null;                                                    // search prefab object by name
                foreach (GameObject blockPrefab in blockPrefabs)
                {
                    if (blockPrefab.name == "empty") break;
                    if (blockPrefab.name == prefabName)
                    {
                        block = blockPrefab;
                        break;
                    }
                }
                if (block == null) continue;
                                                                 
                // Create block and add it in blocks field object.
                GameObject blockObject =                                                
                    Instantiate(block, new Vector3(x, y, block.transform.position.z), Quaternion.identity) as GameObject;
                blockObject.transform.parent = blocksField.transform;
            }
        }
    }

    /// <summary>
    /// Function is calculate size game object by BoxCollider2D component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns></returns>
    private static Vector2 sizeObjByBoxCollider2D(GameObject obj)
    {
        BoxCollider2D objBox = obj.GetComponent<BoxCollider2D>();
        return new Vector2(objBox.size.x * obj.transform.localScale.x,
                           objBox.size.y * obj.transform.localScale.y);
    }
}
