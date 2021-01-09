using UnityEngine;
using System.Collections.Generic;


/*
 * class GameSceneHandler
 * Init all action on game scene (generate level on game scene, etc.)
 */

public class GameSceneHandler : MonoBehaviour
{
    public Vector2 blockSize;
    public Vector2 maxLevelSize;                    // max level height (count blocks)
    public string levelDictPath;                    // path for level dictionary file
    public string levelsPath;                       // path for levels directory
    private int current_level = 1;                  // index current running level
    private LevelFileParser fileParser;             // parser level files

    public int currentLevel
    {
        get { return currentLevel; }
        set { ConstructLevel(current_level); }
    }

    void Start()
    {
        fileParser = GetComponent<LevelFileParser>();
        currentLevel = 1;
    }

    public void ConstructLevel(int level)
    {
        Debug.Log("Kek");
        current_level = level;
        fileParser.parseLevelDict(levelDictPath);
        fileParser.parseLevelFile(levelsPath, level, (int)maxLevelSize.y, (int)maxLevelSize.x);

        foreach (KeyValuePair<char, string> pair in fileParser.levelDict)
        {
            Debug.Log(pair.Key.ToString() + " : " +  pair.Value.ToString());
        }

        if (fileParser.levelMap == null) return;

        for (int i = 0; i < (int)fileParser.mapSize.y; i++)
        {
            string answer = "";
            for (int j = 0; j < (int)fileParser.mapSize.x; j++)
            {
                answer += fileParser.levelMap[i, j].ToString();
            }
            Debug.Log(answer);
        }
    }
}
