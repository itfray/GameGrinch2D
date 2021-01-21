using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/*public abstract class GenObjStrategy
{
    protected GameObject obj_prefab;
    protected GameObject spwnr_prefab;

    protected int row_spwnr_pos;
    protected int col_spwnr_pos;
    protected Vector2 spwnr_pos;
    protected Vector2 spwnr_size;

    protected Dictionary<char, string> level_dict;
    protected char[,] level_map;
    protected Vector2 map_size;

    public GenObjStrategy(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size, Vector2 spwnr_size)
    {
        setMapParams(level_dict, level_map, map_size);
        setSpwnrSize(spwnr_size);
    }

    public abstract void generate();

    public void setMapParams(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size)
    {
        this.level_dict = level_dict;
        this.level_map = level_map;
        this.map_size = map_size;
    }

    public void setSpwnrSize(Vector2 size)
    {
        spwnr_size = size;
    }

    public void setSpwnrPosInMap(int row_pos, int col_pos)
    {
        row_spwnr_pos = row_pos;
        col_spwnr_pos = col_pos;
        spwnr_pos = new Vector2(col_pos * spwnr_size.x, ((int)map_size.y - 1 - row_pos) * spwnr_size.y);            // calculate position for instantiate prefab object
    }

    public void setPrefab(GameObject prefab)
    {
        obj_prefab = prefab;
    }

    public void setSpwnrPrefab(GameObject prefab)
    {
        spwnr_prefab = prefab;
    }
}


public class GenPlayerStrategy: GenObjStrategy
{
    public GenPlayerStrategy(Dictionary<char, string> level_dict, char[,] level_map, Vector2 map_size, Vector2 spwnr_size)
        :base(level_dict, level_map, map_size, spwnr_size)
    {
    }

    public override void generate()
    {
        if (spwnr_prefab == null || level_dict == null || level_map == null) return;

        GameObject spwn_obj = Instantiate(spwnr_prefab,                                                                          // create spawner object
                                          new Vector3(spwnr_pos.x, spwnr_pos.y, spwnr_prefab.transform.position.z),
                                          Quaternion.identity) as GameObject;
        spwn_obj.transform.parent = playerField.transform;

        Vector2 spawn_pos = Vector2.zero;                                                                                       // spawn position of player
        Vector2 player_size = sizeObjByBoxCollider2D(prefab);

        *//* checks nearby blocks in the following way:
         * xxx
         * x x
         * xxx
         * x - checked block
         *//*
        bool find_spwn_pos = false;                                                                         // spawn position was finded?
        for (int cofst = 0; cofst < 3 && !find_spwn_pos; cofst++)
        {
            for (int rofst = -1; rofst < 2; rofst += 2)
            {
                string next_prefname;
                int rpos = row_pos;
                int cpos = col_pos;
                int col_ofst = (cofst < 2 ? cofst : -1);

                cpos += col_ofst;
                if (cpos < 0 || cpos >= map_size.x) continue;

                if (!level_dict.TryGetValue(level_map[rpos, cpos], out next_prefname))                    // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname != emptyPrefabName && cpos != col_pos) continue;                        // if central block is empty

                rpos += rofst;
                if (rpos < 0 || rpos >= map_size.y) continue;

                if (!level_dict.TryGetValue(level_map[rpos, cpos], out next_prefname))                    // check top/bottom block (middle top/bottom, right top/bottom, left top/bottom)
                    next_prefname = emptyPrefabName;

                if (next_prefname != emptyPrefabName) continue;

                spawn_pos.x = spwnr_pos.x + col_ofst * spwnr_size.x;                                      // calculate spawn position
                spawn_pos.y = spwnr_pos.y + rofst * spwnr_size.y / 2 - rofst * player_size.y / 2;
                find_spwn_pos = true;
                break;
            }
        }
        if (!find_spwn_pos) return;

        PlayerSpawner player_spwnr = spwn_obj.GetComponent<PlayerSpawner>();
        if (player_spwnr == null) return;
        player_spwnr.InitSpawner(prefab, playerField, spawn_pos);                                                   // init player spawner
        player_spwnr.Spawn();                                                                                       // spawn player
    }
}*/


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
        Vector2 blockSmplSize = sizeObjByBoxCollider2D(blockSample);
        // calculate map center
        mapCenterPos = new Vector2(blockSmplSize.x * (map_size.x - 1) / 2,
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

        Vector2 blockSmplSize = sizeObjByBoxCollider2D(blockSample);                                                        // get block sample size

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
        Vector2 map_size = fileParser.mapSize;
        Vector2 block_pos = new Vector2(col_pos * block_size.x, ((int)map_size.y - 1 - row_pos) * block_size.y);        // calculate position for instantiate prefab object

        switch (prefab.tag)
        {
            case "Player":
                generatePlayer(prefab, row_pos, col_pos, block_pos, block_size);
                break;
            case "Block":
                generateBlock(prefab, block_pos);
                break;
            case "Saw":
                generateSaw(prefab, row_pos, col_pos, block_pos, block_size);
                break;
            case "BigSaw":
                generateBigSaw(prefab, row_pos, col_pos, block_pos, block_size);
                break;
            case "Spike":
                //generateSpike(prefab, row_pos, col_pos, block_pos, block_size);
                break;
        }
    }

    private void generateSpike(GameObject prefab, int row_pos, int col_pos, Vector2 spwnr_pos, Vector2 spwnr_size)
    {
    }

    private void generateBigSaw(GameObject prefab, int row_pos, int col_pos, Vector2 spwnr_pos, Vector2 spwnr_size)
    {
        Dictionary<char, string> level_dict = fileParser.levelDict;
        char[,] level_map = fileParser.levelMap;
        Vector2 map_size = fileParser.mapSize;

        List<Vector2> busy_poss = new List<Vector2>();

        /* checks nearby blocks in the following way:
         *  x
         * x x
         *  x 
         * x - checked block
         */
        for (int cofst = -1; cofst < 2; cofst++)
        {
            int rofst_start = 0;
            int rofst_end = 1;

            if (cofst == 0)
            {
                rofst_start = -1;
                rofst_end = 2;
            }

            for (int rofst = rofst_start; rofst < rofst_end; rofst += rofst_end)
            {
                string next_prefname;
                int rpos = row_pos + rofst;
                int cpos = col_pos + cofst;

                if (rpos < 0 || rpos >= map_size.y || cpos < 0 || cpos >= map_size.x) continue;

                if (!level_dict.TryGetValue(level_map[rpos, cpos], out next_prefname))                    // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname == emptyPrefabName) continue;

                busy_poss.Add(new Vector2(cpos, rpos));
            }
        }

        Vector2 spawn_pos = Vector2.zero;

        switch (busy_poss.Count)
        {
            case 0:
                return;
            case 1:
                spawn_pos = busy_poss[0];
                break;
            case 2:
                List<Vector2> check_poss = new List<Vector2>();
                check_poss.Add(new Vector2(busy_poss[0].x, busy_poss[1].y));
                check_poss.Add(new Vector2(busy_poss[1].x, busy_poss[0].y));

                /* checks nearby blocks in the following way:
                 * xb   b    bx    b
                 * b   bx     b    xb 
                 *
                 * x - checked block
                 * b - block
                 */
                foreach (Vector2 check_pos in check_poss)
                {
                    string next_prefname;
                    if (!level_dict.TryGetValue(level_map[(int)check_pos.y, (int)check_pos.x], out next_prefname))                    // check name central block (middle center, right center, left center)
                        next_prefname = emptyPrefabName;
                    if (next_prefname == emptyPrefabName || next_prefname == prefab.name) continue;

                    spawn_pos = check_pos;
                }
                break;

            case 3:

                /* checks nearby blocks in the following way:
                 *   x    x       x        
                 *  x      x     x x   x x 
                 *   x    x             x
                 * x - checked block
                 */
                if (busy_poss[0].x == busy_poss[1].x)
                {
                    spawn_pos = busy_poss[2];
                    break;
                }
                else if (busy_poss[1].x == busy_poss[2].x)
                {
                    spawn_pos = busy_poss[0];
                    break;
                }

                busy_poss = busy_poss.OrderBy(v => v.y).ToList();

                if (busy_poss[0].y == busy_poss[1].y)
                {
                    spawn_pos = busy_poss[2];
                }
                else if (busy_poss[1].y == busy_poss[2].y)
                {
                    spawn_pos = busy_poss[0];
                }
                break;

            case 4:
                spawn_pos = new Vector2(col_pos, row_pos);
                break;

            default:
                return;
        }

        spawn_pos = new Vector2(spawn_pos.x * spwnr_size.x, ((int)map_size.y - 1 - spawn_pos.y) * spwnr_size.y);

        GameObject saw = Instantiate(prefab,                                                                  // create block game object
                                     new Vector3(spawn_pos.x, spawn_pos.y, prefab.transform.position.z),
                                     Quaternion.identity) as GameObject;
        saw.transform.parent = sawsField.transform;
    }

    private void generateSaw(GameObject prefab, int row_pos, int col_pos, Vector2 spwnr_pos, Vector2 spwnr_size)
    {
        Dictionary<char, string> level_dict = fileParser.levelDict;
        char[,] level_map = fileParser.levelMap;
        Vector2 map_size = fileParser.mapSize;

        /* checks nearby blocks in the following way:
         *  x 
         * x x
         *  x 
         * x - checked block
         */

        for (int cofst = -1; cofst < 2; cofst++)
        {
            int rofst_start = 0;
            int rofst_end = 1;

            if (cofst == 0)
            {
                rofst_start = -1;
                rofst_end = 2;
            }

            for (int rofst = rofst_start; rofst < rofst_end; rofst += rofst_end)
            {
                string next_prefname;
                int rpos = row_pos + rofst;
                int cpos = col_pos + cofst;

                if (rpos < 0 || rpos >= map_size.y || cpos < 0 || cpos >= map_size.x) continue;

                if (!level_dict.TryGetValue(level_map[rpos, cpos], out next_prefname))                                      // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname == emptyPrefabName || next_prefname == prefab.name) continue;

                Vector2 spawn_pos = new Vector2(spwnr_pos.x + cofst * spwnr_size.x, spwnr_pos.y - rofst * spwnr_size.y);
                GameObject saw = Instantiate(prefab,                                                                        // create block game object
                                         new Vector3(spawn_pos.x, spawn_pos.y, prefab.transform.position.z),
                                         Quaternion.identity) as GameObject;
                saw.transform.parent = sawsField.transform;
            }
        }
    }

    /// <summary>
    /// Method generates block object by prefab of block
    /// </summary>
    /// <param name="prefab"> block prefab </param>
    /// <param name="block_pos"> block position </param>
    private void generateBlock(GameObject prefab, Vector2 block_pos)
    {
        GameObject block = Instantiate(prefab,                                                                  // create block game object
                                       new Vector3(block_pos.x, block_pos.y, prefab.transform.position.z),
                                       Quaternion.identity) as GameObject;
        block.transform.parent = blocksField.transform;                                                         // places block in blocks field
    }

    /// <summary>
    /// Method generates player spawner block by prefab of player and initiates player creation.
    /// </summary>
    /// <param name="prefab"> prefab of player </param>
    /// <param name="row_pos"> row position in level map </param>
    /// <param name="col_pos"> col position in level map </param>
    /// <param name="spwnr_pos"> position of spawner block </param>
    /// <param name="spwnr_size"> size of spawner block </param>
    private void generatePlayer(GameObject prefab, int row_pos, int col_pos, Vector2 spwnr_pos, Vector2 spwnr_size)
    {
        Dictionary<char, string> level_dict = fileParser.levelDict;
        char[,] level_map = fileParser.levelMap; 
        Vector2 map_size = fileParser.mapSize;

        GameObject spwn_prefab = findGameObjByName(prefab.name, spawnPrefabs, (key_n, val_n) => val_n.Contains(key_n));         // search of spawner prefab by player prefab name
        if (spwn_prefab == null) return;
        GameObject spwn_obj = Instantiate(spwn_prefab,                                                                          // create spawner object
                                          new Vector3(spwnr_pos.x, spwnr_pos.y, spwn_prefab.transform.position.z),
                                          Quaternion.identity) as GameObject;
        spwn_obj.transform.parent = playerField.transform;

        Vector2 spawn_pos = Vector2.zero;                                                                                       // spawn position of player
        Vector2 player_size = sizeObjByBoxCollider2D(prefab);

        /* checks nearby blocks in the following way:
         * xxx
         * x x
         * xxx
         * x - checked block
         */
        bool find_spwn_pos = false;                                                                         // spawn position was finded?
        for (int cofst = 0; cofst < 3 && !find_spwn_pos; cofst++)
        {
            for (int rofst = -1; rofst < 2; rofst += 2)
            {
                string next_prefname;
                int rpos = row_pos;
                int cpos = col_pos;
                int col_ofst = (cofst < 2 ? cofst : -1);

                cpos += col_ofst;
                if (cpos < 0 || cpos >= map_size.x) continue;

                if (!level_dict.TryGetValue(level_map[rpos, cpos], out next_prefname))                    // check name central block (middle center, right center, left center)
                    next_prefname = emptyPrefabName;

                if (next_prefname != emptyPrefabName && cpos != col_pos) continue;                        // if central block is empty

                rpos += rofst;
                if (rpos < 0 || rpos >= map_size.y) continue;

                if (!level_dict.TryGetValue(level_map[rpos, cpos], out next_prefname))                    // check top/bottom block (middle top/bottom, right top/bottom, left top/bottom)
                    next_prefname = emptyPrefabName;

                if (next_prefname != emptyPrefabName) continue;

                spawn_pos.x = spwnr_pos.x + col_ofst * spwnr_size.x;                                      // calculate spawn position
                spawn_pos.y = spwnr_pos.y + rofst * spwnr_size.y / 2 - rofst * player_size.y / 2;
                find_spwn_pos = true;
                break;
            }
        }
        if (!find_spwn_pos) return;

        PlayerSpawner player_spwnr = spwn_obj.GetComponent<PlayerSpawner>();
        if (player_spwnr == null) return;
        player_spwnr.InitSpawner(prefab, playerField, spawn_pos);                                                   // init player spawner
        player_spwnr.Spawn();                                                                                       // spawn player
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

        Vector2 bgSmplSize = sizeObjByBoxCollider2D(bgSample);

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
        Vector3 firstBgSize = sizeObjByRenderer(firstBg.gameObject);
        
        Transform lastBg = bg_sorted_list.LastOrDefault();
        Vector3 lastBgSize = sizeObjByRenderer(lastBg.gameObject);

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
        Vector2 bgSmplSize = sizeObjByBoxCollider2D(bgSample);

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
    /// Function is calculate size game object by BoxCollider2D component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns> size game object </returns>
    private static Vector2 sizeObjByBoxCollider2D(GameObject obj)
    {
        BoxCollider2D objBox = obj.GetComponent<BoxCollider2D>();
        if (objBox == null) return Vector2.zero;
        return new Vector2(Mathf.Abs(objBox.size.x * obj.transform.localScale.x),
                           Mathf.Abs(objBox.size.y * obj.transform.localScale.y));
    }

    /// <summary>
    /// Function is calculate size game object by CapsuleCollider2D component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns> size game object </returns>
    private static Vector2 sizeObjByCapsuleCollider2D(GameObject obj)
    {
        CapsuleCollider2D objBox = obj.GetComponent<CapsuleCollider2D>();
        if (objBox == null) return Vector2.zero;
        return new Vector2(Mathf.Abs(objBox.size.x * obj.transform.localScale.x),
                           Mathf.Abs(objBox.size.y * obj.transform.localScale.y));
    }

    /// <summary>
    /// Function is calculate size game object by Renderer component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns> size game object </returns>
    private static Vector2 sizeObjByRenderer(GameObject obj)
    {
        Renderer rndr = obj.GetComponent<Renderer>();
        return rndr.bounds.max - rndr.bounds.min;
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
