using UnityEngine;
using System.Collections.Generic;


/*
 * class GameSceneHandler
 * Init all action on game scene (generate level on game scene, etc.)
 */

public class GameSceneHandler : MonoBehaviour
{
    public GameObject blocksField;                  // field for created blocks
    public GameObject[] blockPrefabs;               // block prefab samples
    public Vector2 maxLevelSize;                    // max level height (count blocks)
    public string levelDictPath;                    // path for level dictionary file
    public string levelsPath;                       // path for levels directory
    private int current_level = 1;                  // index current running level
    private LevelFileParser fileParser;             // parser level files

    public int currentLevel
    {
        get { return currentLevel; }
        set 
        { 
            ConstructLevel(value); 
        }
    }

    void Start()
    {
        fileParser = GetComponent<LevelFileParser>();
        currentLevel = 1;
    }

    // Function create level by level files and level dictionary file
    public void ConstructLevel(int level)
    {
        current_level = level;
        fileParser.parseLevelDict(levelDictPath);                                                     // get level dict
        fileParser.parseLevelFile(levelsPath, level, (int)maxLevelSize.y, (int)maxLevelSize.x);       // get level map
        GenerateLevelObjects(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize);

        // ========================= Debug ================================
        foreach (KeyValuePair<char, string> pair in fileParser.levelDict)
        {
            Debug.Log(pair.Key.ToString() + " : " +  pair.Value.ToString());
        }

        if (fileParser.levelMap == null) return;

        Debug.Log(fileParser.levelBackground);

        for (int i = 0; i < (int)fileParser.mapSize.y; i++)
        {
            string answer = "";
            for (int j = 0; j < (int)fileParser.mapSize.x; j++)
            {
                answer += fileParser.levelMap[i, j].ToString();
            }
            Debug.Log(answer);
        }

        Debug.Log("size: (" + fileParser.mapSize.x.ToString() + "; " + fileParser.mapSize.y.ToString() + ")");
        // ========================= Debug ================================
    }

    private void GenerateLevelObjects(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size)
    {
        if (blockPrefabs.Length <= 0) return;
        BoxCollider2D blockBox = blockPrefabs[0].GetComponent<BoxCollider2D>();
        if (blockBox == null) return;

        for (int i = (int)map_size.y - 1; i >= 0; i--)
        {
            for (int j = (int)map_size.x - 1; j >= 0; j--)
            {
                string prefabName;
                if (!level_dict.TryGetValue(level_map[i, j], out prefabName)) continue;

                float x = ((int)map_size.x - 1 - j) * blockBox.size.x / 2;
                float y = ((int)map_size.y - 1 - i) * blockBox.size.y / 2;

                GameObject block = null;
                foreach (GameObject blockPrefab in blockPrefabs)
                {
                    if (blockPrefab.name == prefabName)
                    {
                        block = blockPrefab;
                        break;
                    }
                }
                if (block == null) continue;

                GameObject blockObject =
                    Instantiate(block, new Vector3(x, y, block.transform.position.z), Quaternion.identity) as GameObject;
                blockObject.transform.parent = blocksField.transform;
            }
        }
    }
}
