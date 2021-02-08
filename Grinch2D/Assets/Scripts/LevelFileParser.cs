using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// class LevelFileParser 
/// is a parser level files data (level dict, level map and etc.).
/// </summary>
public class LevelFileParser : MonoBehaviour
{
    public string levelDictPath;                                                    // path for level dictionary file
    public string bgDictPath;                                                       // path for background level dictionary file
    public string levelsPath;                                                       // path for levels directory
    public Vector2 maxLevelSize;                                                    // max level height (count blocks)

    /* example level dict: {'i': 'ice_block', 's': 'stone_block', ... }
     * example background level dict: {'0': 'bg0', 'h': 'bg_h', ... }
     * example level map, byte matrix:
     * ssssssssssssssssssssssssssssssssssss
     * s                                  s
     * s    sss           p               s
     * s                  s               s
     * s                 sss              s
     * ssssssssssssssssssssssssssssssssssss
     */
    private Dictionary<char, string> level_dict = new Dictionary<char, string>();
    private Dictionary<char, string> bg_dict = new Dictionary<char, string>();
    private char[,] level_map;
    private Vector2 map_size;
    private char level_bg;

    public Dictionary<char, string> levelDict { get { return level_dict; } }            // level dictionary
    public Dictionary<char, string> backgroundDict { get { return bg_dict; } }          // background level dictionary
    public char[,] levelMap { get { return level_map; } }                               // level map, blocks matrix
    public Vector2 mapSize { get { return map_size; } }                                 // size map matrix, (x: count columns, y: count rows)
    public char levelBackground { get { return level_bg; } }                            // level background, background key

    public const string dictSeparator = ":";

    /// <summary>
    /// Method parse file with level dictionary
    /// </summary>
    public void parseLevelDict()
    {
        parseDict(level_dict, levelDictPath);
    }

    /// <summary>
    /// Method parse file with background level dictionary
    /// </summary>
    public void parseBgLevelDict()
    {
        parseDict(bg_dict, bgDictPath);
    }

    /// <summary>
    /// Method parse text file with dictionary
    /// </summary>
    /// <param name="dictPath"> path to the text file with dictionary </param>
    private void parseDict(Dictionary<char, string> dict, string dictPath)
    {
        dict.Clear();

        TextAsset file = Resources.Load<TextAsset>(dictPath);                       // load dict text file
        if (file == null) return;

        foreach (string line in file.text.ReadLines())                              // load lines of file text
        {
            if (line.Contains(dictSeparator))
            {
                string[] pair = line.Split(new string[] { dictSeparator }, System.StringSplitOptions.None);

                dict.Add(pair[0][0], pair[1].Trim());                               // load pair of file, example: "e : evil_block"
            }
        }

        Resources.UnloadAsset(file);                                                 // free file
    }

    public void parseLevelFile(int ind_level)
    {
        int max_h = (int)maxLevelSize.y;
        int max_w = (int)maxLevelSize.x;

        level_map = null;
        map_size = new Vector2(0, 0);
        if (max_h <= 0 || max_w <= 0) return;                                                       // check params correctness

        TextAsset file = Resources.Load<TextAsset>(levelsPath + "/" + ind_level.ToString());
        if (file == null) return;                                                                   // check file exists

        level_map = new char[max_h, max_w];

        IEnumerable<string> readlines = file.text.ReadLinesEnd();
        int count_lines = file.text.CountLines();

        int row = 0;
        int file_max_w = 0;                                                    // max level width in file
        foreach (string line in readlines)
        {
            if (row >= max_h) 
                break;

            int col;
            for (col = 0; col < line.Length && col < max_w; col++)             // read map line
                level_map[row, col] = line[col];

            if (file_max_w < col)                                               // find max level width in file
                file_max_w = col;

            row++;

            if (row >= count_lines - 1) 
                break;
        }
        map_size.y = row;                                                       // store fact map size
        map_size.x = file_max_w;

        foreach (string line in readlines)
        {
            if (line.Length > 0)
                level_bg = line[0];
        }

        Resources.UnloadAsset(file);                                            // free file
    }
}
