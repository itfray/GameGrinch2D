using UnityEngine;
using System.Collections.Generic;



/// <summary>
/// class GameSceneHandler
/// is init all action on game scene (generate level on game scene, etc.)
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    public GameObject blockSample;                  // block sample with box colider2d for generating other blocks
    public GameObject blocksField;                  // field for created blocks
    public GameObject[] blockPrefabs;               // block prefab samples
    public Vector2 maxLevelSize;                    // max level height (count blocks)

    public string levelDictPath;                    // path for level dictionary file
    public string levelsPath;                       // path for levels directory
    private LevelFileParser fileParser;             // parser level files

    private int current_level = 1;                  // index current running level

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
        fileParser.parseLevelFile(levelsPath, level, (int)maxLevelSize.y, (int)maxLevelSize.x);
        CreateLevelObjsByMap(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize);
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

        BoxCollider2D blockBoxSample = blockSample.GetComponent<BoxCollider2D>();
        if (blockBoxSample == null) return;

        for (int i = (int)map_size.y - 1; i >= 0; i--)                                      // reverse step, because file with map was readed top down
        {
            for (int j = 0; j < (int)map_size.x; j++)
            {
                string prefabName;
                if (!level_dict.TryGetValue(level_map[i, j], out prefabName)) continue;     // get prefab name of level dictionary

                float x = j * blockBoxSample.size.x / 2;                                    // calculate position for instantiate prefab object
                float y = ((int)map_size.y - 1 - i) * blockBoxSample.size.y / 2;

                GameObject block = null;                                                    // search prefab object by name
                foreach (GameObject blockPrefab in blockPrefabs)
                {
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
}
