﻿using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// class GameSceneHandler
/// is init all action on game scene (generate level on game scene, etc.)
/// </summary>
public class GameSceneHandler : MonoBehaviour
{
    public LevelFileParser fileParser;                                              // parser level files

    public Sprite[] bgSprites;                                                      // all background sprites
    public SpriteRenderer[] bgRndrs;                                                // list background renderers on game scene

    public GameObject playerField;                                                  // field for created palyer
    public GameObject blocksField;                                                  // field for created blocks
    public GameObject sawsField;                                                    // field for created saws
    public GameObject spikesField;                                                  // field for created spikes
    public GameObject turretsField;                                                 // field for created turrets
    public GameObject starsField;                                                   // field for created stars

    public GameObject[] gamePrefabs;                                                // all game prefabs
    public GameObject[] spawnPrefabs;                                               // all prefabs for spawning game objects

    public string emptyPrefabName = "empty";                                        // empty block prefab name

    public BoxCollider2D blocksmpl_collider;                                        // sample block collider
    public BoxCollider2D bgsmpl_collider;                                           // sample bg collider

    public GenPlayerStrategy gen_player_strtg;                                     // strategy of generation player
    public GenBlockStrategy gen_block_strtg;                                       // strategy of generation block
    public GenSawStrategy gen_saw_strtg;                                           // strategy of generation saw
    public GenBigSawStrategy gen_big_saw_strtg;                                    // strategy of generation big saw
    public GenMovingSawStrategy gen_move_saw_strtg;                                // strategy of generation moving saw
    public GenSpikeStrategy gen_spike_strtg;                                       // strategy of generation spike

    public delegate void GameSceneEventHnd();
    public event GameSceneEventHnd OnInited = null;
    public event GameSceneEventHnd OnConstructedLevel = null;
    public event GameSceneEventHnd OnDeconstructedLevel = null;

    private List<Sprite> levelBgSprites = new List<Sprite>();                       // background sprites for current level

    private Vector2 mapCenterPos;                                                   // central map position for current level

    private PlayerSpawner playerSpawner;                                            // spawner of player game object

    public enum GameState { Uninited, Initing, Inited, Constructing, Constructed, Deconstructing, Deconstructed, Started, Stoped }
    private GameState state = GameState.Uninited;
    public GameState State { get { return state; } }

    private float gameTime = 0f;                                                    // time value player in game
    public float GameTime { get { return gameTime; } }

    private int current_level;                                                      // number current running level
    public int CurrentLevel { get { return current_level; } }

    private int count_levels;                                                       // number of levels in game
    public int CountLevels { get { return count_levels; } }

    public int CountStars                                                           // number of taked stars
    {
        get 
        {
            if (playerSpawner)
            {
                StarHandler starHnd = playerSpawner?.PlayerStarHnd;
                if (starHnd) return starHnd.count_stars;
            }

            return 0;
        } 
    }


    public void Init()
    {
        if (state == GameState.Uninited)
        {
            state = GameState.Initing;
            StartCoroutine(InitGameSceneHnd());                                             // start initialization
        }
    }

    /// <summary>
    /// Method for initializtion game scene handler
    /// </summary>
    /// <returns> null </returns>
    private IEnumerator<object> InitGameSceneHnd()
    {
        count_levels = fileParser.countLevels();                                    // parse data of number of levels
        fileParser.parseBgLevelDict();                                              // parse data of background dictionary file
        yield return null;

        fileParser.parseLevelDict();                                                // parse data of level dictionary file

        state = GameState.Inited;                                                   // set flag intialized
        OnInited?.Invoke();                                                         // callback OnInited handler
    }

    void Update()
    {
        if (state == GameState.Started || state == GameState.Stoped)
        {
            UpdateBackgroundLevel();                                                    // update background sprites and background postions
            if (state == GameState.Started) 
                gameTime += Time.deltaTime;                                             // count game time
        }
    }

    /// <summary>
    /// Method create level objects in gamescene by level files and level dictionary file
    /// </summary>
    /// <param name="level"> level number </param>
    public void ConstructLevel(int level)
    {
        if (state == GameState.Initing || state == GameState.Deconstructing || state == GameState.Constructing)
            return;

        if (level < 1 || level > count_levels)                                                               // check level value on valid
            return;             

        state = GameState.Constructing;

        current_level = level;
        fileParser.parseLevelFile(level);                                                                    // parse data of level map file

        CalcMapCenterPos();                                                                                  // calculate map center
        LoadLevelBgSprites();                                                                                // generate level backgrounds by level file data about background
        
        StartCoroutine(CreateLevelObjsByMap(10));                                                            // generate level game objects by level file data about map    
    }

    /// <summary>
    /// Method destoy level objects in gamescene
    /// </summary>
    public void DeconstructLevel()
    {
        if (state == GameState.Initing || state == GameState.Deconstructing || state == GameState.Constructing)
            return;

        state = GameState.Deconstructing;

        StartCoroutine(DeleteLevelObjs(10));                                                                // destroy level game objects
    }

    /// <summary>
    /// Method runs constructed game from the beginning
    /// </summary>
    public void StartGame()
    {
        if (!(state == GameState.Constructed || state == GameState.Started || state == GameState.Stoped))
            return;
            
        if (state == GameState.Stoped)                                                                      // if game was stoped
            StopGame(false);                                                     
        else
            state = GameState.Started;

        for (int istar = 0; istar < starsField.transform.childCount; istar++)                                // restore all stars
        {
            Transform star_trnsfm = starsField.transform.GetChild(istar);
            star_trnsfm.gameObject.SetActive(true);
        }

        if (playerSpawner) playerSpawner.Spawn();                                                           // spawn player

        gameTime = 0f;                                                                                      // reset game time
    }

    /// <summary>
    /// Method stops or resume constructed game
    /// </summary>
    /// <param name="value"> true: stop, false: resume </param>
    public void StopGame(bool value)
    {
        if (!(state == GameState.Constructed || state == GameState.Started || state == GameState.Stoped))
            return;

        playerField.SetActive(!value);                                                                      // stop player field activity
        sawsField.SetActive(!value);                                                                        // stop saws field activity
        turretsField.SetActive(!value);                                                                     // stop turrets field activity

        state = value ? GameState.Stoped : GameState.Started;
    }

    /// <summary>
    /// Method is find all background sprites for level by background dictionary and background symbol
    /// </summary>
    private void LoadLevelBgSprites()
    {
        Dictionary<char, string> bg_dict = fileParser.backgroundDict;
        char bg_sign = fileParser.levelBackground;

        string bgName;
        if (!bg_dict.TryGetValue(bg_sign, out bgName))                                                  // get background sprite name from background dictionary
            Debug.LogError("Uncorrect level background symbol!!!", this);

        levelBgSprites = new List<Sprite>();                                                            // get list background sprites for level
        foreach (Sprite bg in bgSprites)
            if (bg.name.Contains(bgName))
                levelBgSprites.Add(bg);

        if (levelBgSprites.Count == 0)
            Debug.LogError("Empty list with level background sprites!!!", this);
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
    /// <param name="maxc_gen_per_frame"> max number of generated objects per frame </param>
    /// <returns> void </returns>
    private IEnumerator<object> CreateLevelObjsByMap(int maxc_gen_per_frame)
    {
        if (gamePrefabs.Length <= 0) yield break;

        Dictionary<char, string> level_dict = fileParser.levelDict;
        char[,] level_map = fileParser.levelMap;
        Vector2 map_size = fileParser.mapSize;

        Vector2 blockSmplSize = SizeScripts.sizeObjBy(blocksmpl_collider);                                                  // get block sample size

        int count_gen_objs = 0;                                                                                             // count generated objects per frame
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

                count_gen_objs++;
                if (count_gen_objs >= maxc_gen_per_frame)
                {
                    count_gen_objs = 0;
                    yield return null;
                }
            }
        }

        if (playerSpawner.spawnedObj)
        {
            setTurretsTarget(playerSpawner.spawnedObj);                                                                     // specifies target for all created turrets and set player as target for turret
        }

        state = GameState.Constructed;
        OnConstructedLevel?.Invoke();
    }

    /// <summary>
    /// Method destoru level objects
    /// </summary>
    /// <param name="maxc_del_per_frame">  max number of destroyed objects per frame </param>
    /// <returns></returns>
    private IEnumerator<object> DeleteLevelObjs(int maxc_del_per_frame)
    {
        List<Transform> fields = new List<Transform>();

        fields.Add(playerField.transform);                                          // get all fields
        fields.Add(blocksField.transform);
        fields.Add(sawsField.transform);
        fields.Add(spikesField.transform);
        fields.Add(turretsField.transform);
        fields.Add(starsField.transform);

        int count_del_objs = 0;                                                     // number of destroyed objects per frame
        foreach (Transform field in fields)
        {
            foreach (Transform obj in field)
            {
                Destroy(obj.gameObject);                                            // destoy object of game
                count_del_objs++;
                if (count_del_objs >= maxc_del_per_frame)
                {
                    count_del_objs = 0;
                    yield return null;
                }
            }
        }

        state = GameState.Deconstructed;
        OnDeconstructedLevel?.Invoke();
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
                genObj = gen_block_strtg;
                genObj.objParentField = turretsField;
                break;
            case "Star":
                genObj = gen_block_strtg;
                genObj.objParentField = starsField;
                break;
            default:
                return;
        }

        genObj.objPrefab = prefab;                                                                                                  // set main params for strategy of generation
        genObj.setParams(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize, emptyPrefabName, block_size);
        genObj.setSpwnrPosInMap(row_pos, col_pos);
        genObj.Generate();                                                                                                          // create game object

        if (prefab.tag == "Player")
            playerSpawner = genObj.spwnrHnd as PlayerSpawner;                                                                       // get playerSpawner
    }

    /// <summary>
    /// Method specifies target for all turrets
    /// </summary>
    private void setTurretsTarget(GameObject target)
    {
        for (int i = 0; i < turretsField.transform.childCount; i++)
        {
            Transform turret = turretsField.transform.GetChild(i);
            TurretHandler turret_hnd = turret.GetComponent<TurretHandler>();
            if (turret_hnd) turret_hnd.target = target;
        }
    }

    /// <summary>
    /// Method updates background on current level.
    /// Update big background ( 2 x 2 backgrounds in front of the main camera ).
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
    private void UpdateBackgroundLevel()
    {
        Vector2 bgSmplSize = SizeScripts.sizeObjBy(bgsmpl_collider);

        // calculate number of backgrounds between main camera and map center
        int cbgx_frm_cntr = MathWay.countLinesFitOnBetween(bgSmplSize.x, Camera.main.transform.position.x, mapCenterPos.x);
        int cbgy_frm_cntr = MathWay.countLinesFitOnBetween(bgSmplSize.y, Camera.main.transform.position.y, mapCenterPos.y);

        int istart = -1;                                                                                // offset bottom background from current background
        int iend = 1;                                                                                   // offset top background from current background
        int jstart = -1;
        int jend = 1;

        if (cbgy_frm_cntr > 0)                                                                           // check which side (top or bottom) there is no background in front of the camera
            iend -= 1;                                                                                   // need a background lower than the current one
        else
            istart += 1;                                                                                 // need a background upper than the current one

        if (cbgx_frm_cntr > 0)
            jend -= 1;
        else
            jstart += 1;

        int imid_sprt = midLevelBgSpriteIndex();                                                         // central background sprite displayed in map center position

        int iobj = 0;
        // Place big background ( 2 x 2 backgrounds in front of the main camera )
        for (int i = istart; i <= iend; i++)
        {
            // Place 2 backgrounds in a line
            for (int j = jstart; j <= jend; j++)
            {
                // change bg position in front of the camera
                bgRndrs[iobj].transform.position = new Vector3(mapCenterPos.x + (cbgx_frm_cntr + j) * bgSmplSize.x,
                                                                mapCenterPos.y + (cbgy_frm_cntr + i) * bgSmplSize.y, 
                                                                bgRndrs[iobj].transform.position.z);
                bgRndrs[iobj].sprite = getLevelBgSprite(imid_sprt + cbgy_frm_cntr + i);
                iobj++;
            }
        }
    }

    /// <summary>
    /// Method is return background sprite from levelBgSprites by index.
    /// </summary>
    /// <param name="sprt_ind"> background sprite index </param>
    /// <returns> background sprite </returns>
    private Sprite getLevelBgSprite(int sprt_ind)
    {
        Sprite bg_sprite = null;

        if (levelBgSprites.Count == 0) 
            return bg_sprite;

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
        if (levelBgSprites.Count % 2 == 0) 
            ind_mid_bg -= 1;
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
