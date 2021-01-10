using System.IO;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// class LevelFileParser 
/// is a parser level files data (level dict, level map and etc.).
/// </summary>
public class LevelFileParser : MonoBehaviour
{
    /* example level dict: {'i': 'ice_block', 's': 'stone_block', ... }
     * example level map, byte matrix:
     * ssssssssssssssssssssssssssssssssssss
     * s                                  s
     * s    sss           p               s
     * s                  s               s
     * s                 sss              s
     * ssssssssssssssssssssssssssssssssssss
     */
    private Dictionary<char, string> level_dict;
    private char[,] level_map;
    private Vector2 map_size;
    private char level_bg;

    public Dictionary<char, string> levelDict { get { return level_dict; } }            // level dictionary
    public char[,] levelMap { get { return level_map; } }                               // level map, blocks matrix
    public Vector2 mapSize { get { return map_size; } }                                 // size map matrix, (x: count columns, y: count rows)
    public char levelBackground { get { return level_bg; } }                            // level background, background key

    /// <summary>
    /// Method parse file with level dictionary
    /// </summary>
    /// <param name="levelDictPath"> path to the file with level dictionary </param>
    public void parseLevelDict(string levelDictPath)
    {
        if (level_dict == null)
            level_dict = new Dictionary<char, string>();
        else
            level_dict.Clear();

        if (!File.Exists(levelDictPath)) return;                                    // check file exists

        using (StreamReader sr = File.OpenText(levelDictPath))                      // open text file with level dictionary
        {
            string line;
            while ((line = sr.ReadLine()) != null)                                  // load line of file
            {
                string[] pair = line.Split(':');
                if (pair[0].Length == 0) continue;
                level_dict.Add(pair[0][0], pair[1].Split(' ')[1]);                  // load pair of file, example: "e : evil_block"
            }
        }
    }

    /// <summary>
    /// Method parse file with level map
    /// </summary>
    /// <param name="levelsPath"> path to the file with level map </param>
    /// <param name="ind_level"> level number </param>
    /// <param name="max_h"> max blocks count in height </param>
    /// <param name="max_w"> max blocks count in width </param>
    public void parseLevelFile(string levelsPath, int ind_level, int max_h, int max_w)
    {
        level_map = null;
        map_size = new Vector2(0, 0);
        if (max_h <= 0 || max_w <= 0) return;                                   // check params correctness

        string levelPath = levelsPath + "/" + ind_level.ToString() + ".txt";
        if (!File.Exists(levelPath)) return;                                    // check file exists

        level_map = new char[max_h, max_w];

        using (StreamReader sr = File.OpenText(levelPath))                      // open text file with level map
        {
            string line;
            if ((line = sr.ReadLine()) == null) return;
            
            if (line.Length > 0) level_bg = line[0];                            // the first byte is a value level background

            int row;
            int file_max_w = 0;                                                 // max level width in file
            for (row = 0; (line = sr.ReadLine()) != null && row < max_h; row++)
            {
                for (int col = 0; col < line.Length && col < max_w; col++)
                    level_map[row, col] = line[col];

                if (file_max_w < line.Length)                                  // find max level width in file
                    file_max_w = line.Length;
            }
            map_size.y = row;                                                  // store fact map size
            map_size.x = file_max_w;                  
        }
    }
}
