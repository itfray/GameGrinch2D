﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// class GameSceneHandler
/// is init all action on game scene (generate level on game scene, etc.)
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    public LevelFileParser fileParser;                                              // parser level files

    public GameObject bgField;                                                      // field for store 4 childs (background game objects)
    public GameObject bgSample;                                                     // background sample with box colider2d for getting size background game object
    public Sprite[] bgSprites;                                                      // all background sprites

    public GameObject playerField;                                                  // field for created palyer
    public GameObject blocksField;                                                  // field for created blocks
    public GameObject sawsField;                                                    // field for created saws
    public GameObject spikesField;                                                  // field for created spikes

    public GameObject blockSample;                                                  // block sample with box colider2d for generating other blocks
    public GameObject[] gamePrefabs;                                                // all game prefabs
    public GameObject[] spawnPrefabs;                                               // all prefabs for spawning game objects

    public string emptyPrefabName = "empty";                                         // empty block prefab name

    private int current_level;                                                      // number current running level

    private List<Sprite> levelBgSprites;                                            // background sprites for current level

    private Vector2 mapCenterPos;                                                   // central map position for current level

    private enum ScrollDirect { Left = -2, Down = -1, Up = 1, Right = 2 }           // type direction for background scrolling functions
    private LinkedList<Transform> bg_sorted_byx;                                    // sorted on X-axis background objects list for current level
    private LinkedList<Transform> bg_sorted_byy;                                    // sorted on Y-axis background objects list for current level

    public int currentLevel
    {
        get { return current_level; }
        set { ConstructLevel(value); }
    }

    void Start()
    {
        fileParser.parseLevelDict();                                                // parse data of level dictionary file
        fileParser.parseBgLevelDict();                                              // parse data of background dictionary file

        currentLevel = 5;
    }

    void Update()
    {
        ScrollLevelBackground();
    }

    /// <summary>
    /// Method create level in gamescene by level files and level dictionary file
    /// </summary>
    /// <param name="level"> level number </param>
    public void ConstructLevel(int level)
    {
        current_level = level;
        fileParser.parseLevelFile(level);                                                                   // parse data of level map file

        CalcMapCenterPos();
        CreateLevelObjsByMap();                                                                             // generate level game objects by level file data about map    
        CreateLevelBackground();                                                                            // generate level backgrounds by level file data about background
    }


    /// <summary>
    /// Method for calculating position of map center in current level
    /// </summary>
    void CalcMapCenterPos()
    {
        Vector2 map_size = fileParser.mapSize;
        Vector2 blockSmplSize = SizeScripts.sizeObjByBoxCollider2D(blockSample);
        mapCenterPos = new Vector2(blockSmplSize.x * (map_size.x - 1) / 2,                                  // calculate map center
                                   blockSmplSize.y * (map_size.y - 1) / 2);
    }

    /// <summary>
    /// Method create level objects by level dictionary and level map
    /// </summary>
    private void CreateLevelObjsByMap()
    {
        if (gamePrefabs.Length <= 0) return;

        Dictionary<char, string> level_dict = fileParser.levelDict;
        char[,] level_map = fileParser.levelMap;
        Vector2 map_size = fileParser.mapSize;

        Vector2 blockSmplSize = SizeScripts.sizeObjByBoxCollider2D(blockSample);                                            // get block sample size

        for (int i = (int)map_size.y - 1; i >= 0; i--)                                                                      // reverse step, because file with map was readed top down
        {
            for (int j = 0; j < (int)map_size.x; j++)
            {
                string prefabName;
                if (!level_dict.TryGetValue(level_map[i, j], out prefabName)) continue;                                     // get prefab name of level dictionary

                if (prefabName == emptyPrefabName) continue;                                                                // check empty prefab name

                GameObject prefab = findGameObjByName(prefabName, gamePrefabs, (key_n, val_n) => key_n == val_n);           // get game prefab by name
                if (prefab == null) continue;

                generateGameObj(prefab, i, j, blockSmplSize);                                                               // generate game object
            }
        }
    }


    /// <summary>
    /// Method generates game object by prefab
    /// </summary>
    /// <param name="prefab"> game object prefab </param>
    /// <param name="row_pos"> row position in level map </param>
    /// <param name="col_pos"> col position in level map </param>
    /// <param name="block_size"> size of one block </param>
    private void generateGameObj(GameObject prefab, int row_pos, int col_pos, Vector2 block_size)
    {
        GenObjStrategy genObj;                                                                                                      // strategy of generation game object

        switch (prefab.tag)
        {
            case "Player":
                genObj = GetComponent<GenPlayerStrategy>();                                                                         // select strategy of generation player
                genObj.spwnrPrefab = findGameObjByName(prefab.name, spawnPrefabs, (key_n, val_n) => val_n.Contains(key_n));         // search of spawner prefab by player prefab name
                genObj.spwnrParentField = playerField;                                                                              // select parent fields for generation
                genObj.objParentField = playerField;
                break;
            case "Block":
                genObj = GetComponent<GenBlockStrategy>();
                genObj.objParentField = blocksField;
                break;
            case "Saw":
                genObj = GetComponent<GenSawStrategy>();
                genObj.objParentField = sawsField;
                break;
            case "BigSaw":
                genObj = GetComponent<GenBigSawStrategy>();
                genObj.objParentField = sawsField;
                break;
            case "Spike":
                genObj = GetComponent<GenSpikeStrategy>();
                genObj.objParentField = spikesField;
                break;
            default:
                return;
        }

        genObj.objPrefab = prefab;                                                                                                  // set main params for strategy of generation
        genObj.setParams(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize, emptyPrefabName, block_size);
        genObj.setSpwnrPosInMap(row_pos, col_pos);
        genObj.Generate();                                                                                                          // create game object
    }

    /// <summary>
    /// Method is create background for level.
    /// Create big background ( 2 x 2 backgrounds in front of the main camera ).
    /// set the backgrounds position something like this:
    /// -------------------------
    /// |           |           |
    /// |    bg1    |    bg2    |
    /// |      -----------      |
    /// |      |    |    |      |
    /// |------|-- cam --|------|
    /// |      |    |    |      |
    /// |      -----------      |
    /// |    bg3    |    bg4    |
    /// |           |           |
    /// -------------------------
    /// </summary>
    private void CreateLevelBackground()
    {
        Dictionary<char, string> bg_dict = fileParser.backgroundDict;
        char bg_sign = fileParser.levelBackground;

        levelBgSprites = findLevelBgSprites(bg_dict, bg_sign);                                          // get list background sprites on level
        if (levelBgSprites.Count == 0)
            Debug.LogError("Empty list with level background sprites!!!", this);

        Vector2 bgSmplSize = SizeScripts.sizeObjByBoxCollider2D(bgSample);

        // calculate number of backgrounds between main camera and map center
        int cbg_from_center = countLinesFitOnBetween(bgSmplSize.y, Camera.main.transform.position.y, mapCenterPos.y);

        int istart = -1;                                                                                // offset bottom background from current background
        int iend = 1;                                                                                   // offset top background from current background

        if (cbg_from_center > 0)                                                                        // check which side (top or bottom) there is no background in front of the camera
            iend -= 1;                                                                                  // need a background lower than the current one
        else
            istart += 1;                                                                                // need a background upper than the current one

        int ind_bg_obj = 0;                                                                             // index background game object in background field

        // Create big background ( 2 x 2 backgrounds in front of the main camera )
        for (int i = istart; i <= iend; i++)
        {
            // Create 2 backgrounds in a line
            for (int j = -1; j < 1; j++)
            {
                // change bg position in front of the camera
                Transform bg_trnsfm = bgField.transform.GetChild(ind_bg_obj);
                bg_trnsfm.position = new Vector3(Camera.main.transform.position.x + j * bgSmplSize.x + bgSmplSize.x / 2,
                                                 mapCenterPos.y + (cbg_from_center + i) * bgSmplSize.y, 0);
                ind_bg_obj++;
            }
        }

        bg_sorted_byx = getSortedBgList(t => t.position.x);             // get sorted linked list backgrounds objects on X-axis
        bg_sorted_byy = getSortedBgList(t => t.position.y);             // get sorted linked list backgrounds objects on Y-axis
        UpdateLevelBgSpritesInBgObjs();                                 // update sprites in background objects
    }

    /// <summary>
    /// Method for getting sorted background objects list on specified condition.
    /// </summary>
    /// <param name="order_func"> specified condition </param>
    /// <returns> sorted background objects linked list </returns>
    private LinkedList<Transform> getSortedBgList(System.Func<Transform, float> order_func)
    {
        LinkedList<Transform> bg_list = new LinkedList<Transform>();
        List<Transform> bgs = new List<Transform>();
        for (int i = 0; i < bgField.transform.childCount; i++)
            bgs.Add(bgField.transform.GetChild(i));

        bgs = bgs.OrderBy(order_func).ToList();

        for (int i = 0; i < bgs.Count; i++)
            bg_list.AddLast(bgs[i]);
        return bg_list;
    }

    /// <summary>
    /// Method scroll background objects depending on main camera position.
    /// </summary>
    private void ScrollLevelBackground()
    {
        if (bg_sorted_byx.Count == 0 || bg_sorted_byy.Count == 0) return;

        Transform firstBg;
        Renderer firstRndr;

        Transform lastBg;
        Renderer lastRndr;

        firstBg = bg_sorted_byx.FirstOrDefault();
        firstRndr = firstBg.GetComponent<Renderer>();

        lastBg = bg_sorted_byx.LastOrDefault();
        lastRndr = lastBg.GetComponent<Renderer>();

        if (firstRndr.isVisible == false && Camera.main.transform.position.x > lastBg.position.x)
        {
            ScrollLevelBackground(bg_sorted_byx, ScrollDirect.Right);                                       // scroll backgrounds left
        }
        else if (lastRndr.isVisible == false && Camera.main.transform.position.x < firstBg.position.x)
        {
            ScrollLevelBackground(bg_sorted_byx, ScrollDirect.Left);                                        // scroll backgrounds right
        }

        firstBg = bg_sorted_byy.FirstOrDefault();                                                           // first up background object
        firstRndr = firstBg.GetComponent<Renderer>();

        lastBg = bg_sorted_byy.LastOrDefault();                                                             // last down background object
        lastRndr = lastBg.GetComponent<Renderer>();

        bool scrolled_y = false;
        if (firstRndr.isVisible == false && Camera.main.transform.position.y > lastBg.position.y)
        {
            ScrollLevelBackground(bg_sorted_byy, ScrollDirect.Up);                                           // scroll backgrounds up
            scrolled_y = true;
        }
        else if (lastRndr.isVisible == false && Camera.main.transform.position.y < firstBg.position.y)
        {
            ScrollLevelBackground(bg_sorted_byy, ScrollDirect.Down);                                         // scroll backgrounds down
            scrolled_y = true;
        }

        if (scrolled_y)
            UpdateLevelBgSpritesInBgObjs();                                                                  // update sprites of background objects
    }

    /// <summary>
    /// Method is scroll background objects depending on specified direction.
    /// </summary>
    /// <param name="bg_sorted_list"> sorted backgrounds list to scroll </param>
    /// <param name="direct"> scrolling direction </param>
    private void ScrollLevelBackground(LinkedList<Transform> bg_sorted_list, ScrollDirect direct)
    {
        Transform firstBg = bg_sorted_list.FirstOrDefault();
        Vector3 firstBgSize = SizeScripts.sizeObjByRenderer(firstBg.gameObject);
        
        Transform lastBg = bg_sorted_list.LastOrDefault();
        Vector3 lastBgSize = SizeScripts.sizeObjByRenderer(lastBg.gameObject);

        List<Transform> listBgs = new List<Transform>();
        for (int i = 0; i < 2; i++)
        {
            Transform bg;
            if (direct > 0)                                                                                      // values of Up and Right more zero
            {
                bg = bg_sorted_list.FirstOrDefault();
                bg.position = direct == ScrollDirect.Right ? 
                              new Vector3(lastBg.position.x + lastBgSize.x, bg.position.y, bg.position.z) :      // set position current after the last background on X-axis
                              new Vector3(bg.position.x, lastBg.position.y + lastBgSize.y, bg.position.z);
            }
            else
            {
                bg = bg_sorted_list.LastOrDefault();
                bg.position = direct == ScrollDirect.Left ? 
                              new Vector3(firstBg.position.x - firstBgSize.x, bg.position.y, bg.position.z):
                              new Vector3(bg.position.x, firstBg.position.y - firstBgSize.y, bg.position.z);     // set position current before the first background on Y-axis
            }
            listBgs.Add(bg);
            bg_sorted_list.Remove(bg);                                                                           // 1. change background position in sorted backgrounds objects list
        }

        for (int i = 0; i < 2; i++)
        {
            if (direct > 0)
                bg_sorted_list.AddLast(listBgs[i]);                                                             // 2. change background position in sorted backgrounds objects list
            else
                bg_sorted_list.AddFirst(listBgs[i]);
        }
    }

    /// <summary>
    /// Method is update sprites of background objects depending on map center position.
    /// </summary>
    private void UpdateLevelBgSpritesInBgObjs()
    {
        Vector2 bgSmplSize = SizeScripts.sizeObjByBoxCollider2D(bgSample);

        Transform firstBg = bg_sorted_byy.FirstOrDefault();

        // calculate number of backgrounds between main camera and map center
        int cbg_from_center = countLinesFitOnBetween(bgSmplSize.y, firstBg.position.y, mapCenterPos.y);

        // central background sprite displayed in map center position
        int ind_bg = midLevelBgSpriteIndex() + cbg_from_center;                                         // get first background sprite index (which background stage to display)

        int i = 0;
        foreach (Transform bg_trnsfm in bg_sorted_byy)
        {
            SpriteRenderer bg_srndr = bg_trnsfm.gameObject.GetComponent<SpriteRenderer>();
            bg_srndr.sprite = getLevelBgSprite(ind_bg + i / 2);                                       // set right background sprite
            i++;
        }
    }

    /// <summary>
    /// Method is find all background sprites for level by background dictionary and background symbol
    /// </summary>
    /// <param name="bg_dict"> background dictionary with backround sprite names </param>
    /// <param name="bg_sign"> backround symbol for level</param>
    /// <returns> list of background sprites for level </returns>
    private List<Sprite> findLevelBgSprites(Dictionary<char, string> bg_dict, char bg_sign)
    {
        string bgName;
        if (!bg_dict.TryGetValue(bg_sign, out bgName))                                                  // get background sprite name from background dictionary
            Debug.LogError("Uncorrect level background symbol!!!", this);

        List<Sprite> level_bgs = new List<Sprite>();                                                    // get list background sprites for level
        foreach (Sprite bg in bgSprites)
            if (bg.name.Contains(bgName))
                level_bgs.Add(bg);
        return level_bgs;
    }

    /// <summary>
    /// Method is return background sprite from levelBgSprites by index.
    /// </summary>
    /// <param name="sprt_ind"> background sprite index </param>
    /// <returns> background sprite </returns>
    private Sprite getLevelBgSprite(int sprt_ind)
    {
        Sprite bg_sprite;
        if (sprt_ind < 0)                                           // if (background index < 0) then repeat background first sprite
            bg_sprite = levelBgSprites[0];
        else if (sprt_ind >= levelBgSprites.Count)                  // if (background index >= count backgrounds) then repeat background last sprite
            bg_sprite = levelBgSprites[levelBgSprites.Count - 1];
        else
            bg_sprite = levelBgSprites[sprt_ind];
        return bg_sprite;
    }

    /// <summary>
    /// Get central background sprite index in list background sprites
    /// </summary>
    /// <returns> mid sprite index in levelBgSprites </returns>
    private int midLevelBgSpriteIndex()
    {
        int ind_mid_bg = levelBgSprites.Count / 2;
        if (levelBgSprites.Count % 2 == 0) ind_mid_bg -= 1;
        return ind_mid_bg;
    }

    /// <summary>
    /// Function is calculate how many lines with specified length can fit between two positions.
    /// Example: 
    /// pos1 = -7.5, pos2 = 0, len = 4, result = -2
    /// pos1 = 7.5, pos2 = 0, len = 4, result = 2
    /// </summary>
    /// <param name="line_len"> Checked line length </param>
    /// <param name="pos1"> Position 1 </param>
    /// <param name="pos2"> Position 2  </param>
    /// <returns> Signed count </returns>
    private static int countLinesFitOnBetween(float line_len, float pos1, float pos2)
    {
        float fc = (pos1 - pos2) / line_len;
        int fcsign = fc > 0 ? 1 : -1;
        int c = fcsign * Mathf.RoundToInt(Mathf.Abs(fc));
        return c;
    }

    /// <summary>
    /// Function for searching object in objects list by name
    /// </summary>
    /// <param name="obj_name"> name object of search </param>
    /// <param name="objs"> list objects </param>
    /// <param name="cond"> condition for check object names </param>
    /// <returns> game object </returns>
    private static GameObject findGameObjByName(string obj_name, GameObject[] objs, System.Func<string, string, bool> cond)
    {
        GameObject find_obj = null;                                                           // search prefab object by prefab name
        foreach (GameObject obj in objs)
        {
            if (cond(obj_name, obj.name))
            {
                find_obj = obj;
                break;
            }
        }
        return find_obj;
    }
}
