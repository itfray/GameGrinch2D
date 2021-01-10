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
        CreateLevelBackground(fileParser.backgroundDict, fileParser.levelBackground);
    }

    private void CreateLevelBackground(Dictionary<char, string> bg_dict, char bg_sign)
    {
        string bgName;
        if (!bg_dict.TryGetValue(bg_sign, out bgName)) Debug.LogError("Uncorrect level background symbol!!!");

        Vector2 bgSmplSize = sizeObjByBoxCollider2D(bgSample);

        int count_bgs = (int)(Mathf.Abs(Camera.main.transform.position.y) / bgSmplSize.y);

        List<GameObject> rightBgs = new List<GameObject>();
        foreach (GameObject bgObj in bgPrefabs)
            if (bgObj.name.Contains(bgName))
                rightBgs.Add(bgObj);

        for (int i = -1; i < 2; i++)
        {
            int count_bgsi = count_bgs - i;
            if (count_bgsi < 0) continue;
            else if (count_bgsi >= rightBgs.Count) count_bgsi = rightBgs.Count - 1;

            GameObject bg = Instantiate(rightBgs[count_bgsi], new Vector3(Camera.main.transform.position.x, count_bgsi * bgSmplSize.y, 0), Quaternion.identity) as GameObject;
            bg.transform.parent = bgField.transform;
            Destroy(bg.GetComponent<BoxCollider2D>());
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

    private static Vector2 sizeObjByBoxCollider2D(GameObject obj)
    {
        BoxCollider2D objBox = obj.GetComponent<BoxCollider2D>();
        return new Vector2(objBox.size.x * obj.transform.localScale.x,
                           objBox.size.y * obj.transform.localScale.y);
    }
}
