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
    public GameObject turretsField;                                                 // field for created turrets

    public GameObject blockSample;                                                  // block sample with box colider2d for generating other blocks
    public GameObject[] gamePrefabs;                                                // all game prefabs
    public GameObject[] spawnPrefabs;                                               // all prefabs for spawning game objects

    public string emptyPrefabName = "empty";                                        // empty block prefab name

    private int current_level;                                                      // number current running level

    private List<Sprite> levelBgSprites;                                            // background sprites for current level

    private Vector2 mapCenterPos;                                                   // central map position for current level

    private GameObjSpawner playerSpawner;                                           // spawner of player game object

    private List<SpriteRenderer> bg_rndrs;                                          // list background renderers

    private enum ScrollDirect { Left = -2, Down = -1, Up = 1, Right = 2 }           // type direction for background scrolling functions
    private LinkedList<SpriteRenderer> bg_sorted_byx;                               // sorted on X-axis background objects list for current level
    private LinkedList<SpriteRenderer> bg_sorted_byy;                               // sorted on Y-axis background objects list for current level

    private GenPlayerStrategy gen_player_strtg;                                     // strategy of generation player
    private GenBlockStrategy gen_block_strtg;                                       // strategy of generation block
    private GenSawStrategy gen_saw_strtg;                                           // strategy of generation saw
    private GenBigSawStrategy gen_big_saw_strtg;                                    // strategy of generation big saw
    private GenMovingSawStrategy gen_move_saw_strtg;                                // strategy of generation moving saw
    private GenSpikeStrategy gen_spike_strtg;                                       // strategy of generation spike
    private GenTurretStrategy gen_turret_strtg;                                     // strategy of generation turret

    private BoxCollider2D blocksmpl_collider;
    private BoxCollider2D bgsmpl_collider;

    public int currentLevel
    {
        get { return current_level; }
        set { ConstructLevel(value); }
    }

    void Start()
    {
        // get all strategys of generation game object
        gen_player_strtg = GetComponent<GenPlayerStrategy>();
        gen_block_strtg = GetComponent<GenBlockStrategy>();
        gen_saw_strtg = GetComponent<GenSawStrategy>();
        gen_big_saw_strtg = GetComponent<GenBigSawStrategy>();
        gen_move_saw_strtg = GetComponent<GenMovingSawStrategy>();
        gen_spike_strtg = GetComponent<GenSpikeStrategy>();
        gen_turret_strtg = GetComponent<GenTurretStrategy>();

        blocksmpl_collider = blockSample.GetComponent<BoxCollider2D>();
        bgsmpl_collider = bgSample.GetComponent<BoxCollider2D>();

        bg_rndrs = new List<SpriteRenderer>();
        for (int i = 0; i < bgField.transform.childCount; i++)
            bg_rndrs.Add(bgField.transform.GetChild(i).GetComponent<SpriteRenderer>());

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
        Vector2 blockSmplSize = SizeScripts.sizeObjBy(blocksmpl_collider);
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

        Vector2 blockSmplSize = SizeScripts.sizeObjBy(blocksmpl_collider);                                                  // get block sample size

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

        setTurretsTarget();                                                                                                 // specifies target for all created turrets
    }

    /// <summary>
    /// Method specifies target(player) for all turrets
    /// </summary>
    private void setTurretsTarget()
    {
        if (playerSpawner)
        {
            for (int i = 0; i < turretsField.transform.childCount; i++)
            {
                Transform turret = turretsField.transform.GetChild(i);
                TurretHandler turret_hnd = turret.GetComponent<TurretHandler>();
                if (turret_hnd) turret_hnd.target = playerSpawner.spawnedObj;                                                                // set player as target for turret
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
                genObj = gen_player_strtg;                                                                                          // select strategy of generation player                                                                                                                                            
                genObj.spwnrPrefab = findGameObjByName(prefab.name, spawnPrefabs, (key_n, val_n) => val_n.Contains(key_n));         // search of spawner prefab by player prefab name
                genObj.spwnrParentField = playerField;                                                                              // select parent fields for generation
                genObj.objParentField = playerField;
                break;
            case "Block":
                genObj = gen_block_strtg;
                genObj.objParentField = blocksField;
                break;
            case "Saw":
                genObj = gen_saw_strtg;
                genObj.objParentField = sawsField;
                break;
            case "BigSaw":
                genObj = gen_big_saw_strtg;
                genObj.objParentField = sawsField;
                break;
            case "Spike":
                genObj = gen_spike_strtg;
                genObj.objParentField = spikesField;
                break;
            case "MovingSaw":
                genObj = gen_move_saw_strtg;
                genObj.objParentField = sawsField;
                break;
            case "Turret":
                genObj = gen_turret_strtg;
                genObj.objParentField = turretsField;
                break;
            default:
                return;
        }

        genObj.objPrefab = prefab;                                                                                                  // set main params for strategy of generation
        genObj.setParams(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize, emptyPrefabName, block_size);
        genObj.setSpwnrPosInMap(row_pos, col_pos);
        genObj.Generate();                                                                                                          // create game object

        if (prefab.tag == "Player")
            playerSpawner = genObj.spwnrHnd;                                                                                        // set playerSpawner  
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

        Vector2 bgSmplSize = SizeScripts.sizeObjBy(bgsmpl_collider);

        // calculate number of backgrounds between main camera and map center
        int cbg_from_center = MathWay.countLinesFitOnBetween(bgSmplSize.y, Camera.main.transform.position.y, mapCenterPos.y);

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

        bg_sorted_byx = getSortedBgList(r => r.transform.position.x);             // get sorted linked list backgrounds objects on X-axis
        bg_sorted_byy = getSortedBgList(r => r.transform.position.y);             // get sorted linked list backgrounds objects on Y-axis

        UpdateLevelBgSpritesInBgObjs();                                           // update sprites in background objects
    }

    /// <summary>
    /// Method for getting sorted background objects list on specified condition.
    /// </summary>
    /// <param name="order_func"> specified condition </param>
    /// <returns> sorted background objects linked list </returns>
    private LinkedList<SpriteRenderer> getSortedBgList(System.Func<SpriteRenderer, float> order_func)
    {
        LinkedList<SpriteRenderer> bg_list = new LinkedList<SpriteRenderer>();

        foreach (SpriteRenderer bg_rndr in bg_rndrs.OrderBy(order_func).ToList())
            bg_list.AddLast(bg_rndr);

        return bg_list;
    }

    /// <summary>
    /// Method scroll background objects depending on main camera position.
    /// </summary>
    private void ScrollLevelBackground()
    {
        if (bg_sorted_byx.Count == 0 || bg_sorted_byy.Count == 0) return;

        Renderer firstRndr;
        Renderer lastRndr;

        firstRndr = bg_sorted_byx.FirstOrDefault();
        lastRndr = bg_sorted_byx.LastOrDefault();

        if (firstRndr.isVisible == false && Camera.main.transform.position.x > lastRndr.transform.position.x)
        {
            ScrollLevelBackground(bg_sorted_byx, ScrollDirect.Right, firstRndr, lastRndr);                      // scroll backgrounds right
        }
        else if (lastRndr.isVisible == false && Camera.main.transform.position.x < firstRndr.transform.position.x)
        {
            ScrollLevelBackground(bg_sorted_byx, ScrollDirect.Left, firstRndr, lastRndr);                       // scroll backgrounds left
        }

        firstRndr = bg_sorted_byy.FirstOrDefault();                                                             // first up background object
        lastRndr = bg_sorted_byy.LastOrDefault();                                                               // last down background object

        bool scrolled_y = false;
        if (firstRndr.isVisible == false && Camera.main.transform.position.y > lastRndr.transform.position.y)
        {
            ScrollLevelBackground(bg_sorted_byy, ScrollDirect.Up, firstRndr, lastRndr);                         // scroll backgrounds up
            scrolled_y = true;
        }
        else if (lastRndr.isVisible == false && Camera.main.transform.position.y < firstRndr.transform.position.y)
        {
            ScrollLevelBackground(bg_sorted_byy, ScrollDirect.Down, firstRndr, lastRndr);                       // scroll backgrounds down
            scrolled_y = true;
        }

        if (scrolled_y)
            UpdateLevelBgSpritesInBgObjs();                                                                     // update sprites of background objects
    }

    /// <summary>
    /// Method is scroll background objects depending on specified direction.
    /// </summary>
    /// <param name="bg_sorted_list"> sorted backgrounds list to scroll </param>
    /// <param name="direct"> scrolling direction </param>
    /// <param name="firstBgRndr"> renderer of first background </param>
    /// <param name="lastBgRndr"> renderer of last background </param>
    private void ScrollLevelBackground(LinkedList<SpriteRenderer> bg_sorted_list, ScrollDirect direct, Renderer firstBgRndr, Renderer lastBgRndr)
    {
        Vector3 firstBgSize = SizeScripts.sizeObjBy(firstBgRndr);
        Vector3 lastBgSize = SizeScripts.sizeObjBy(lastBgRndr);
        Transform firstBg = firstBgRndr.transform;
        Transform lastBg = lastBgRndr.transform;

        List<SpriteRenderer> listBgs = new List<SpriteRenderer>();
        for (int i = 0; i < 2; i++)
        {
            Transform bg;
            SpriteRenderer rndr;
            if (direct > 0)                                                                                      // values of Up and Right more zero
            {
                rndr = bg_sorted_list.FirstOrDefault();
                bg = rndr.transform;
                bg.position = direct == ScrollDirect.Right ? 
                              new Vector3(lastBg.position.x + lastBgSize.x, bg.position.y, bg.position.z) :      // set position current after the last background on X-axis
                              new Vector3(bg.position.x, lastBg.position.y + lastBgSize.y, bg.position.z);
            }
            else
            {
                rndr = bg_sorted_list.LastOrDefault();
                bg = rndr.transform;
                bg.position = direct == ScrollDirect.Left ? 
                              new Vector3(firstBg.position.x - firstBgSize.x, bg.position.y, bg.position.z):
                              new Vector3(bg.position.x, firstBg.position.y - firstBgSize.y, bg.position.z);     // set position current before the first background on Y-axis
            }
            listBgs.Add(rndr);
            bg_sorted_list.Remove(rndr);                                                                         // 1. change background position in sorted backgrounds objects list
        }

        for (int i = 0; i < 2; i++)
        {
            if (direct > 0)
                bg_sorted_list.AddLast(listBgs[i]);                                                              // 2. change background position in sorted backgrounds objects list
            else
                bg_sorted_list.AddFirst(listBgs[i]);
        }
    }

    /// <summary>
    /// Method is update sprites of background objects depending on map center position.
    /// </summary>
    private void UpdateLevelBgSpritesInBgObjs()
    {
        Vector2 bgSmplSize = SizeScripts.sizeObjBy(bgsmpl_collider);

        Transform firstBg = bg_sorted_byy.FirstOrDefault().transform;

        // calculate number of backgrounds between main camera and map center
        int cbg_from_center = MathWay.countLinesFitOnBetween(bgSmplSize.y, firstBg.position.y, mapCenterPos.y);

        // central background sprite displayed in map center position
        int ind_bg = midLevelBgSpriteIndex() + cbg_from_center;                                         // get first background sprite index (which background stage to display)

        int i = 0;
        foreach (SpriteRenderer bg_srndr in bg_sorted_byy)
        {
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
