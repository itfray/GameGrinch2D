using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LevelFileParser : MonoBehaviour
{
    private Dictionary<char, string> level_dict;
    private char[,] level_map;
    private Vector2 map_size;

    public Dictionary<char, string> levelDict { get { return level_dict; } }
    public char[,] levelMap { get { return level_map; } }
    public Vector2 mapSize { get { return map_size; } }

    void Start()
    {
        level_dict = new Dictionary<char, string>();
        level_map = null;
        map_size = new Vector2(0, 0);
    }

    // Function parse file with level dictionary
    // example answer: {'i': 'ice_block', 's': 'stone_block', ... }
    public void parseLevelDict(string levelDictPath)
    {
        level_dict.Clear();
        if (!File.Exists(levelDictPath)) return;                                    // check file exists

        using (StreamReader sr = File.OpenText(levelDictPath))                      // open text file with level dictionary
        {
            string line;
            while ((line = sr.ReadLine()) != null)                                  // load data of file
            {
                string[] pair = line.Split(':');
                if (pair[0].Length == 0) continue;
                level_dict.Add(pair[0][0], pair[1].Split(' ')[1]);
            }
        }
    }

    // Function parse file with level map
    /* example answer: 
     * ssssssssssssssssssssssssssssssssssss
     * s                                  s
     * s                  p               s
     * s                  s               s
     * s                  ss              s
     * ssssssssssssssssssssssssssssssssssss
     */
    public void parseLevelFile(string levelsPath, int ind_level, int max_h, int max_w)
    {
        level_map = null;
        map_size = new Vector2(0, 0);
        if (max_h <= 0 || max_w <= 0) return;

        string levelPath = levelsPath + "/" + ind_level.ToString() + ".txt";
        if (!File.Exists(levelPath)) return;                                    // check file exists

        level_map = new char[max_h, max_w];                                     // create map array

        using (StreamReader sr = File.OpenText(levelPath))                      // open text file with level dictionary
        {
            string line;
            int row = 0;
            int max_len_line = -1;                                              // max line length in file
            while ((line = sr.ReadLine()) != null && row < max_h)               // load map of file
            {
                for (int col = 0; col < line.Length && col < max_w; col++)      // load map line of file line
                    level_map[row, col] = line[col];
                row++;
                if (max_len_line < line.Length)
                    max_len_line = line.Length;
            }
            map_size.y = row;                                                   // store fact map size
            map_size.x = max_len_line;                  
        }
    }
}
