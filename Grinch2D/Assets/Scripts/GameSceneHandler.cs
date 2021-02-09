using UnityEngine;
using System.Collections.Generic;
using System.Collections;


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
    public GameObject disappBlocksField;                                            // field for created disappearing blocks
    public GameObject sawsField;                                                    // field for created saws
    public GameObject spikesField;                                                  // field for created spikes
    public GameObject turretsField;                                                 // field for created turrets
    public GameObject starsField;                                                   // field for created stars
    public GameObject giftsField;                                                   // field for created gifts
    public GameObject keysField;                                                    // field for created keys

    public const string playerTag = "Player";
    public const string blockTag = "Block";
    public const string disappBlockTag = "DisappBlock";
    public const string movingBlockTag = "MovingBlock";
    public const string sawTag = "Saw";
    public const string bigSawTag = "BigSaw";
    public const string movingSawTag = "MovingSaw";
    public const string spikeTag = "Spike";
    public const string turretTag = "Turret";
    public const string starTag = "Star";
    public const string giftTag = "Gift";
    public const string keyTag = "Key";

    public GameObject[] gamePrefabs;                                                // all game prefabs
    public GameObject[] spawnPrefabs;                                               // all prefabs for spawning game objects

    public string emptyPrefabName = "empty";                                        // empty block prefab name

    public BoxCollider2D blocksmpl_collider;                                        // sample block collider
    public BoxCollider2D bgsmpl_collider;                                           // sample bg collider

    public GenPlayerStrategy gen_player_strtg;                                     // strategy of generation player
    public GenBlockStrategy gen_block_strtg;                                       // strategy of generation block
    public GenSawStrategy gen_saw_strtg;                                           // strategy of generation saw
    public GenBigSawStrategy gen_big_saw_strtg;                                    // strategy of generation big saw
    public GenMovingBlockStrategy gen_move_block_strtg;                            // strategy of generation moving block
    public GenSpikeStrategy gen_spike_strtg;                                       // strategy of generation spike
    public GenKeyStrategy gen_key_strtg;                                           // strategy of generation key

    public delegate void GameSceneEventHnd();                                       // type handler of events of GameSceneHandler
    public event GameSceneEventHnd OnInited;                                        // invoke when game scene handler inited
    public event GameSceneEventHnd OnConstructedLevel;                              // invoke when game scene handler constructed
    public event GameSceneEventHnd OnDeconstructedLevel;                            // invoke when game scene gandler deconstructed

    public event GameSceneEventHnd OnWined
    {
        add
        {

            if (playerSpawner != null && playerSpawner?.PlayerGiftHnd != null)
                playerSpawner.PlayerGiftHnd.OnTaked += () => value?.Invoke();       // invoke callback of win handler when player is take gift 
        }

        remove {}
    }

    public event GameSceneEventHnd OnLosed  
    {
        add
        {
            if (playerSpawner != null && playerSpawner?.PlayerHealthHnd != null)
                playerSpawner.PlayerHealthHnd.OnDied += (obj) => value?.Invoke();     // invoke callback of lose handler when player is died 
        }

        remove {}
    }

    private List<Sprite> levelBgSprites = new List<Sprite>();                       // background sprites for current level

    private Vector2 mapCenterPos;                                                   // central map position for current level

    private PlayerSpawner playerSpawner;                                            // spawner of player game object

    private List<TurretHandler> turretHnds = new List<TurretHandler>();             // handlers of turrets

    private List<DisappBlockHnd> disappBlockHnds = new List<DisappBlockHnd>();      // disappearing blocks

    private List<KeyHandler> keyHnds = new List<KeyHandler>();                      // handlers of key objects

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
            StartCoroutine(InitGameSceneHnd());                                    // start initialization
        }
    }

    /// <summary>
    /// Method for initializtion game scene handler
    /// </summary>
    /// <returns> null </returns>
    private IEnumerator<object> InitGameSceneHnd()
    {
        count_levels = SelectMenuControl.countLevels;                               // get data of number of levels

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
        
        StartCoroutine(CreateLevelObjsByMap());                                                              // generate level game objects by level file data about map    
    }

    /// <summary>
    /// Method destoy level objects in gamescene
    /// </summary>
    public void DeconstructLevel()
    {
        if (state == GameState.Initing || state == GameState.Deconstructing || state == GameState.Constructing)
            return;

        state = GameState.Deconstructing;

        StartCoroutine(DeleteLevelObjs());                                                                // destroy level game objects
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

        List<Transform> fields = new List<Transform>();
        fields.Add(starsField.transform);                                                                   // restore all stars
        fields.Add(giftsField.transform);                                                                   // restore all gifts

        foreach (Transform field in fields)
            foreach (Transform obj in field)
                obj.gameObject.SetActive(true);

        List<DisappearHandler> disappHnds = new List<DisappearHandler>();
        disappHnds.AddRange(disappBlockHnds);
        disappHnds.AddRange(keyHnds);

        foreach (DisappearHandler disapp_hnd in disappHnds)                                                 // restore all diapearing blocks
            disapp_hnd.Appear(true);

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
        keysField.SetActive(!value);

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
        {
            Debug.LogError("Uncorrect level background symbol!!!", this);
            return;
        }

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
    /// <returns> void </returns>
    private IEnumerator CreateLevelObjsByMap()
    {
        if (gamePrefabs.Length <= 0) yield break;

        Dictionary<char, string> level_dict = fileParser.levelDict;
        char[,] level_map = fileParser.levelMap;
        Vector2 map_size = fileParser.mapSize;

        Vector2 blockSmplSize = SizeScripts.sizeObjBy(blocksmpl_collider);                                                  // get block sample size

        for (int i = 0; i < (int)map_size.y; i++)                                                                           
        {
            for (int j = 0; j < (int)map_size.x; j++)
            {
                string prefabName;
                if (!level_dict.TryGetValue(level_map[i, j], out prefabName)) continue;                                     // get prefab name of level dictionary

                if (prefabName == emptyPrefabName) continue;                                                                // check empty prefab name

                GameObject prefab = findGameObjByName(prefabName, gamePrefabs, (key_n, val_n) => key_n == val_n);           // get game prefab by name
                if (prefab == null) continue;

                generateGameObj(prefab, i, j, blockSmplSize);                                                               // generate game object
                yield return null;
            }
        }

        if (playerSpawner?.spawnedObj)
            setTurretsTarget(playerSpawner.spawnedObj);                                                                     // specifies target for all created turrets and set player as target for turret

        yield return null;

        foreach (object step in whileKeysNotCompleted())                                                                    // executes while have keys that not created all locked blocks
            yield return null;

        state = GameState.Constructed;
        OnConstructedLevel?.Invoke();
    }

    /// <summary>
    /// Method destoru level objects
    /// </summary>
    private IEnumerator DeleteLevelObjs()
    {
        disappBlockHnds.Clear();
        turretHnds.Clear();
        keyHnds.Clear();

        List<Transform> fields = new List<Transform>();
        fields.Add(playerField.transform);                                          // get all fields
        fields.Add(blocksField.transform);
        fields.Add(sawsField.transform);
        fields.Add(spikesField.transform);
        fields.Add(turretsField.transform);
        fields.Add(starsField.transform);
        fields.Add(giftsField.transform);
        fields.Add(disappBlocksField.transform);
        fields.Add(keysField.transform);

        List<Transform> objs = new List<Transform>();
        foreach (Transform field in fields)
        {
            foreach (Transform obj in field)
            {
                objs.Add(obj);
            }
        }

        foreach (Transform obj in objs)
        {
            Destroy(obj.gameObject);                                            // destoy object of game
            yield return null;
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
            case playerTag:
                genObj = gen_player_strtg;                                                                                          // select strategy of generation player                                                                                                                                            
                genObj.spwnrPrefab = findGameObjByName(prefab.name, spawnPrefabs, (key_n, val_n) => val_n.Contains(key_n));         // search of spawner prefab by player prefab name
                genObj.spwnrParentField = playerField;                                                                              // select parent fields for generation
                genObj.objParentField = playerField;
                break;
            case blockTag:
                genObj = gen_block_strtg;
                genObj.objParentField = blocksField;
                break;
            case disappBlockTag:
                genObj = gen_block_strtg;
                genObj.objParentField = disappBlocksField;
                break;
            case movingBlockTag:
                genObj = gen_move_block_strtg;
                genObj.objParentField = blocksField;
                break;
            case sawTag:
                genObj = gen_saw_strtg;
                genObj.objParentField = sawsField;
                break;
            case bigSawTag:
                genObj = gen_big_saw_strtg;
                genObj.objParentField = sawsField;
                break;
            case spikeTag:
                genObj = gen_spike_strtg;
                genObj.objParentField = spikesField;
                break;
            case movingSawTag:
                genObj = gen_move_block_strtg;
                genObj.objParentField = sawsField;
                break;
            case turretTag:
                genObj = gen_block_strtg;
                genObj.objParentField = turretsField;
                break;
            case keyTag:
                genObj = gen_key_strtg;
                genObj.objParentField = blocksField;
                genObj.spwnrParentField = keysField;
                genObj.spwnrPrefab = prefab;
                break;
            case starTag:
                genObj = gen_block_strtg;
                genObj.objParentField = starsField;
                break;
            case giftTag:
                genObj = gen_block_strtg;
                genObj.objParentField = giftsField;
                break;
            default:
                return;
        }

        genObj.objPrefab = prefab;                                                                                                  // set main params for strategy of generation
        genObj.setParams(fileParser.levelDict, fileParser.levelMap, fileParser.mapSize, emptyPrefabName, block_size);
        genObj.setSpwnrPosInMap(row_pos, col_pos);
        genObj.Generate();                                                                                                          // create game object

        switch (prefab.tag)
        {
            case playerTag:
                playerSpawner = genObj.createdSpwnrs[0].GetComponent<PlayerSpawner>();                                             // get playerSpawner
                break;
            case disappBlockTag:
                DisappBlockHnd block_hnd = genObj.createdObjs[0].GetComponent<DisappBlockHnd>();
                disappBlockHnds.Add(block_hnd);
                break;
            case turretTag:
                TurretHandler turret_hnd = genObj.createdObjs[0].GetComponent<TurretHandler>();
                turretHnds.Add(turret_hnd);
                break;
            case keyTag:
                KeyHandler key_hnd = genObj.createdSpwnrs[0].GetComponent<KeyHandler>();
                keyHnds.Add(key_hnd);
                break;
        }
    }

    /// <summary>
    /// Method specifies target for all turrets
    /// </summary>
    private void setTurretsTarget(GameObject target)
    {
        foreach (TurretHandler turret_hnd in turretHnds)
        {
            if (turret_hnd) turret_hnd.target = target;
        }
    }

    /// <summary>
    /// Calculate count completed key objects that created all locked blocks
    /// </summary>
    /// <returns> count </returns>
    private int countCompletedKeys()
    {
        int count_keys = 0;
        foreach (KeyHandler key_hnd in keyHnds)
        {
            if (key_hnd.isLockBlocksCreated)
                count_keys++;
        }
        return count_keys;
    }

    /// <summary>
    /// Executes while have keys that not created all locked blocks
    /// </summary>
    /// <returns> null </returns>
    private IEnumerable whileKeysNotCompleted()
    {
        int count_keys;                                                                                                     // checks that all locked blocks created
        do
        {
            count_keys = countCompletedKeys();
            yield return null;
        }
        while (count_keys != keyHnds.Count);
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
