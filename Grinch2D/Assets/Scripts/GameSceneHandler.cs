using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


/// <summary>
/// class GameSceneHandler
/// is init all action on game scene (generate level on game scene, etc.)
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    public GameObject bgField;                                                      // field for store 4 childs (background game objects)
    public GameObject bgSample;                                                     // background sample with box colider2d for getting size background game object
    public Sprite[] bgSprites;                                                      // all background sprites

    public GameObject blocksField;                                                  // field for created blocks
    public GameObject blockSample;                                                  // block sample with box colider2d for generating other blocks
    public GameObject[] blockPrefabs;                                               // all block prefabs
    public Vector2 maxLevelSize;                                                    // max level height (count blocks)

    public string levelDictPath;                                                    // path for level dictionary file
    public string bgDictPath;                                                       // path for background level dictionary file
    public string levelsPath;                                                       // path for levels directory
    public LevelFileParser fileParser;                                              // parser level files

    public string emptyBlockName = "empty";                                         // empty block prefab name

    private int current_level;                                                      // number current running level
    private List<Sprite> levelBgSprites;                                            // background sprites for current level
    private Vector2 mapCenterPos;                                                   // central map position for current level
    private LinkedList<Transform> bg_sorted_byx;                                    // sorted on X-axis background objects list for current level
    private LinkedList<Transform> bg_sorted_byy;                                    // sorted on Y-axis background objects list for current level

    private enum ScrollDirect { Left = -2, Down = -1, Up = 1, Right = 2 }         // type direction for background scrolling functions

    public int currentLevel
    {
        get { return currentLevel; }
        set { ConstructLevel(value); }
    }

    void Start()
    {
        currentLevel = 1;
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
        fileParser.parseLevelDict(levelDictPath);                                                           // parse data of level dictionary file
        fileParser.parseBgLevelDict(bgDictPath);                                                            // parse data of background dictionary file
        fileParser.parseLevelFile(levelsPath, level, (int)maxLevelSize.y, (int)maxLevelSize.x);             // parse data of level map file
        CreateLevelObjsByMap(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize);                // generate level game objects by level file data about map    
        CreateLevelBackground(fileParser.backgroundDict, fileParser.levelBackground);                       // generate level backgrounds by level file data about background
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

        Vector2 blockSmplSize = sizeObjByBoxCollider2D(blockSample);                        // get block sample size

        for (int i = (int)map_size.y - 1; i >= 0; i--)                                      // reverse step, because file with map was readed top down
        {
            for (int j = 0; j < (int)map_size.x; j++)
            {
                string prefabName;
                if (!level_dict.TryGetValue(level_map[i, j], out prefabName)) continue;     // get prefab name of level dictionary

                if (prefabName == emptyBlockName) continue;                                 // check empty block

                GameObject block = null;                                                    // search prefab object by prefab name
                foreach (GameObject blockPrefab in blockPrefabs)
                {
                    if (prefabName == blockPrefab.name)
                    {
                        block = blockPrefab;
                        break;
                    }
                }
                if (block == null) continue;

                float x = j * blockSmplSize.x;                                              // calculate position for instantiate prefab object
                float y = ((int)map_size.y - 1 - i) * blockSmplSize.y;

                // create block game object
                GameObject blockObject =
                    Instantiate(block, new Vector3(x, y, block.transform.position.z), Quaternion.identity) as GameObject;

                // add block in blocks field
                blockObject.transform.parent = blocksField.transform;
            }
        }

        mapCenterPos = new Vector2(blockSmplSize.x * (map_size.x - 1) / 2,
                                   blockSmplSize.y * (map_size.y - 1) / 2);
    }

    /// <summary>
    /// Method is create background for level.
    /// </summary>
    /// <param name="bg_dict"> background dictionary </param>
    /// <param name="bg_sign"> background symbol </param>
    /// <param name="map_blocks_size"> map size in number of blocks </param>
    private void CreateLevelBackground(Dictionary<char, string> bg_dict, char bg_sign)
    {
        levelBgSprites = findLevelBgSprites(bg_dict, bg_sign);                                          // get list background sprites on this level
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
                Transform bg_trnsfm = bgField.transform.GetChild(ind_bg_obj);
                bg_trnsfm.position = new Vector3(Camera.main.transform.position.x + j * bgSmplSize.x + bgSmplSize.x / 2,            // set right position background game object
                                                 mapCenterPos.y + (cbg_from_center + i) * bgSmplSize.y, 0);
                ind_bg_obj++;
            }
        }

        bg_sorted_byx = getSortedBgList(t => t.position.x);
        bg_sorted_byy = getSortedBgList(t => t.position.y);
        UpdateLevelBgSpritesInBgObjs(true);
    }

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
            ScrollLevelBackground(bg_sorted_byx, ScrollDirect.Right);
        }
        else if (lastRndr.isVisible == false && Camera.main.transform.position.x < firstBg.position.x)
        {
            ScrollLevelBackground(bg_sorted_byx, ScrollDirect.Left);
        }

        firstBg = bg_sorted_byy.FirstOrDefault();
        firstRndr = firstBg.GetComponent<Renderer>();

        lastBg = bg_sorted_byy.LastOrDefault();
        lastRndr = lastBg.GetComponent<Renderer>();

        bool scrolled_y = false;
        if (firstRndr.isVisible == false && Camera.main.transform.position.y > lastBg.position.y)
        {
            ScrollLevelBackground(bg_sorted_byy, ScrollDirect.Up);
            scrolled_y = true;
        }
        else if (lastRndr.isVisible == false && Camera.main.transform.position.y < firstBg.position.y)
        {
            ScrollLevelBackground(bg_sorted_byy, ScrollDirect.Down);
            scrolled_y = true;
        }

        UpdateLevelBgSpritesInBgObjs(scrolled_y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bg_sorted_list"></param>
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
            if (direct > 0)
            {
                bg = bg_sorted_list.FirstOrDefault();
                bg.position = direct == ScrollDirect.Right ? 
                              new Vector3(lastBg.position.x + lastBgSize.x, bg.position.y, bg.position.z): 
                              new Vector3(bg.position.x, lastBg.position.y + lastBgSize.y, bg.position.z);
            }
            else
            {
                bg = bg_sorted_list.LastOrDefault();
                bg.position = direct == ScrollDirect.Left ? 
                              new Vector3(firstBg.position.x - firstBgSize.x, bg.position.y, bg.position.z):
                              new Vector3(bg.position.x, firstBg.position.y - firstBgSize.y, bg.position.z);
            }
            listBgs.Add(bg);
            bg_sorted_list.Remove(bg);
        }

        for (int i = 0; i < 2; i++)
        {
            if (direct > 0)
                bg_sorted_list.AddLast(listBgs[i]);
            else
                bg_sorted_list.AddFirst(listBgs[i]);
        }
    }

    private void UpdateLevelBgSpritesInBgObjs(bool scrolled_y)
    {
        if (scrolled_y)
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
    }

    private List<Sprite> findLevelBgSprites(Dictionary<char, string> bg_dict, char bg_sign)
    {
        string bgName;
        if (!bg_dict.TryGetValue(bg_sign, out bgName))                                                  // get background prefab name from background dictionary
            Debug.LogError("Uncorrect level background symbol!!!", this);

        List<Sprite> level_bgs = new List<Sprite>();                                                    // get list background sprites on this level
        foreach (Sprite bg in bgSprites)
            if (bg.name.Contains(bgName))
                level_bgs.Add(bg);
        return level_bgs;
    }

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
        return new Vector2(objBox.size.x * obj.transform.localScale.x,
                           objBox.size.y * obj.transform.localScale.y);
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
}
