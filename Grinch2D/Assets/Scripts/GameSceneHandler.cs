using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// class GameSceneHandler
/// is init all action on game scene (generate level on game scene, etc.)
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    public GameObject bgField;                                      // field for store 4 childs (background game objects)
    public GameObject bgSample;                                     // background sample with box colider2d for getting size background game object
    public Sprite[] bgSprites;                                      // all background sprites

    public GameObject blocksField;                                  // field for created blocks
    public GameObject blockSample;                                  // block sample with box colider2d for generating other blocks
    public GameObject[] blockPrefabs;                               // all block prefabs
    public Vector2 maxLevelSize;                                    // max level height (count blocks)

    public string levelDictPath;                                    // path for level dictionary file
    public string bgDictPath;                                       // path for background level dictionary file
    public string levelsPath;                                       // path for levels directory
    private LevelFileParser fileParser;                             // parser level files

    public string emptyBlockName = "empty";                         // empty block prefab name

    private int current_level;                                      // number current running level

    // ====================================================================
    private LinkedList<Transform> bg_sorted_byx = new LinkedList<Transform>();
    private LinkedList<Transform> bg_sorted_byy = new LinkedList<Transform>();
    private delegate bool CondScrollBg();
    // ====================================================================

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
        CreateLevelBackground(fileParser.backgroundDict, fileParser.levelBackground, fileParser.mapSize);   // generate level backgrounds by level file data about background
    }

    /// <summary>
    /// Function is calculate size game object by BoxCollider2D component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns></returns>
    private static Vector2 sizeObjByBoxCollider2D(GameObject obj)
    {
        BoxCollider2D objBox = obj.GetComponent<BoxCollider2D>();
        return new Vector2(objBox.size.x * obj.transform.localScale.x,
                           objBox.size.y * obj.transform.localScale.y);
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
    }

    /// <summary>
    /// Method is create background for level.
    /// </summary>
    /// <param name="bg_dict"> background dictionary </param>
    /// <param name="bg_sign"> background symbol </param>
    /// <param name="map_blocks_size"> map size in number of blocks </param>
    private void CreateLevelBackground(Dictionary<char, string> bg_dict, char bg_sign, Vector2 map_blocks_size)
    {
        string bgName;
        if (!bg_dict.TryGetValue(bg_sign, out bgName))                                                  // get background prefab name from background dictionary
            Debug.LogError("Uncorrect level background symbol!!!", this);

        Vector2 blockSmplSize = sizeObjByBoxCollider2D(blockSample);                                    // get size sample prefabs by BoxCollider2D component
        Vector2 bgSmplSize = sizeObjByBoxCollider2D(bgSample);

        float mapCenterY = blockSmplSize.y * (map_blocks_size.y - 1) / 2;                               // calculate map center 'Y' position

        float fcbg_from_center = (Camera.main.transform.position.y - mapCenterY) / bgSmplSize.y;        // calculate offset current background in number of backgrounds between main camera and central background
        int cbg_from_center = fcbg_from_center >= 0 ?                                                   // int offset current background in number of backgrounds
                              Mathf.CeilToInt(fcbg_from_center) : Mathf.FloorToInt(fcbg_from_center);

        List<Sprite> level_bgs = new List<Sprite>();                                                    // get list background sprites on this level
        foreach (Sprite bg in bgSprites)
            if (bg.name.Contains(bgName))
                level_bgs.Add(bg);

        int ind_mid_bg = level_bgs.Count / 2;                                                           // get central background index in list sprites
        if (level_bgs.Count % 2 == 0) ind_mid_bg -= 1;

        int ind_bg = ind_mid_bg + cbg_from_center;                                                      // get current background index

        int istart = -1;                                                                                // offset bottom background from current background
        int iend = 1;                                                                                   // offset top background from current background

        if (fcbg_from_center > 0)                                                                       // check which side (top or bottom) there is no background in front of the camera
            iend -= 1;                                                                                  // need a background lower than the current one
        else
            istart += 1;                                                                                // need a background upper than the current one

        int ind_in_bgfield = 0;                                                                         // index background game object in background field

        // Create big background ( 2 x 2 backgrounds in front of the main camera )
        for (int i = istart; i <= iend; i++)
        {
            int ind_bgi = ind_bg + i;                           // index in background sprite list
            Sprite bg_sprite;
            if (ind_bgi < 0)                                    // if (background index < 0) then repeat background first sprite
                bg_sprite = level_bgs[0];
            else if (ind_bgi >= level_bgs.Count)                // if (background index >= count backgrounds) then repeat background last sprite
                bg_sprite = level_bgs[level_bgs.Count - 1];
            else
                bg_sprite = level_bgs[ind_bgi];

            // Create 2 backgrounds in a line
            for (int j = -1; j < 1; j++)
            {
                Transform bg_trnsfm = bgField.transform.GetChild(ind_in_bgfield);
                SpriteRenderer bg_srndr = bg_trnsfm.gameObject.GetComponent<SpriteRenderer>();
                bg_srndr.sprite = bg_sprite;                                                                                        // change background sprite
                bg_trnsfm.position = new Vector3(Camera.main.transform.position.x + j * bgSmplSize.x + bgSmplSize.x / 2,            // change position background game object
                                                 mapCenterY + (cbg_from_center + i) * bgSmplSize.y, 0);
                ind_in_bgfield++;                                                                                                   // next background game object
            }
        }

        List<Transform> bg_byx = new List<Transform>();
        List<Transform> bg_byy = new List<Transform>();
        for (int i = 0; i < bgField.transform.childCount; i++)
        {
            bg_byx.Add(bgField.transform.GetChild(i));
            bg_byy.Add(bgField.transform.GetChild(i));
        }
        bg_byx = bg_byx.OrderBy(t => t.position.x).ToList();
        bg_byy = bg_byy.OrderBy(t => t.position.y).ToList();

        for (int i = 0; i < bgField.transform.childCount; i++)
        {
            bg_sorted_byx.AddLast(bg_byx[i]);
            bg_sorted_byy.AddLast(bg_byy[i]);
        }
    }

    private void ScrollLevelBackground()
    {
        if (bg_sorted_byx.Count == 0) return;

        Transform firstChild = bg_sorted_byx.FirstOrDefault();
        Renderer firstRenderer = firstChild.GetComponent<Renderer>();
        Vector3 firstSize = (firstRenderer.bounds.max - firstRenderer.bounds.min);

        Transform lastChild = bg_sorted_byx.LastOrDefault();
        Renderer lastRenderer = lastChild.GetComponent<Renderer>();
        Vector3 lastSize = (lastRenderer.bounds.max - lastRenderer.bounds.min);

        if (firstRenderer.isVisible == false && Camera.main.transform.position.x > lastChild.position.x)
        {
            bg_sorted_byx.Remove(firstChild);
            Transform secondChild = bg_sorted_byx.FirstOrDefault();
            bg_sorted_byx.Remove(secondChild);
            bg_sorted_byx.AddLast(firstChild);
            bg_sorted_byx.AddLast(secondChild);

            firstChild.position = new Vector3(lastChild.position.x + lastSize.x, firstChild.position.y, firstChild.position.z);
            secondChild.position = new Vector3(lastChild.position.x + lastSize.x, secondChild.position.y, secondChild.position.z);
        }
        else if (lastRenderer.isVisible == false && Camera.main.transform.position.x < firstChild.position.x)
        {
            bg_sorted_byx.Remove(lastChild);
            Transform prelastChild = bg_sorted_byx.LastOrDefault();
            bg_sorted_byx.Remove(prelastChild);
            bg_sorted_byx.AddFirst(prelastChild);
            bg_sorted_byx.AddFirst(lastChild);

            lastChild.position = new Vector3(firstChild.position.x - firstSize.x, lastChild.position.y, lastChild.position.z);
            prelastChild.position = new Vector3(firstChild.position.x - firstSize.x, prelastChild.position.y, prelastChild.position.z);
        }
    }
}
