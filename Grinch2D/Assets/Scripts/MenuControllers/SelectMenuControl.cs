using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static MenuControlHelper;
using static CoroutineScripts;

/// <summary>
/// SelectMenuControl is class for control select level menu
/// </summary>
public class SelectMenuControl : MonoBehaviour
{
    public LevelFileParser fileParser;                                  // file parser
    public EventSystem eventSystem;                                     // event system
    public MainMenuControl mainMenuControl;                             // main menu control script

    public Button[] levelBoxButtons;                                    // buttons of levels
    public Text[] levelBoxTexts;                                        // all texts of levels
    public Image[] levelBoxLocks;                                       // all lock images of levels
    public Image[] levelBoxStarBars;                                    // all starbars of levels
    public Text[] levelBoxTimeBars;                                     // all timebars of levels

    public Sprite[] starBarSprites;                                     // all sprites of starbar

    public Button nextButton;                                           // "Next page" button
    public Button prevButton;                                           // "Prev page" button

    // ************ preference names **********************
    public const string levelPref = "level";                            // preference name that store number of level

    public const string levelPrefixPref = "level_";
    public const string levelStarSufixPref = "_stars";
    public const string levelTimeSufixPref = "_times";
    public const string levelOpenedPref = "level_opened";
    // ****************************************************

    public const string gameSceneName = "GameScene";                  // game scene name

    public float waitForSecGame = 0.5f;                               // wait for seconds of transition to game

    private int start_with_level = 1;                                 // page starts with level
    private int selected_level = 0;                                   // selected level
    private int count_levels = 0;                                     // count levels

    void Start()
    {
        if (fileParser)
            count_levels = fileParser.countLevels();                  // count level files

        UpdateMenuPage();
    }

    /// <summary>
    /// Updates elements in page of level select menu
    /// </summary>
    public void UpdateMenuPage()
    {
        int opened_level = PlayerPrefs.GetInt(levelOpenedPref, 1);

        for (int ilvbox = 0; ilvbox < levelBoxButtons.Length; ilvbox++)
        {
            Button levelbox = levelBoxButtons[ilvbox];                                                                  
            int level = ilvbox + start_with_level;
            bool active_box = level <= count_levels;                                                                            // if level exists than activate level button
            levelbox.gameObject.SetActive(active_box);
            if (!active_box) continue;

            levelBoxTexts[ilvbox].text = level.ToString();                                                                      // set level number as text for button

            levelBoxLocks[ilvbox].gameObject.SetActive(level > opened_level);                                                   // if level not opened than activate lock image 

            int count_stars = PlayerPrefs.GetInt(levelPrefixPref + level + levelStarSufixPref, 0);
            UpdateSpriteStarBar(count_stars, levelBoxStarBars[ilvbox], starBarSprites);                                         // update strabar for level

            float game_time = PlayerPrefs.GetFloat(levelPrefixPref + level + levelTimeSufixPref, -1f);
            bool active_timebar = game_time >= 0;
            if (active_timebar)
                levelBoxTimeBars[ilvbox].text = SecondsToTimeStr(game_time);                                                    // update timebar for level
            levelBoxTimeBars[ilvbox].gameObject.SetActive(active_timebar);

            levelbox.onClick.RemoveAllListeners();
            levelbox.onClick.AddListener(() => LevelBoxSelect(levelbox, level));                                                // add callback that handle level select
        }

        if (nextButton) nextButton.gameObject.SetActive(start_with_level + levelBoxButtons.Length <= count_levels);             // if next page exists that activate "Next page" button
        if (prevButton) prevButton.gameObject.SetActive(start_with_level - levelBoxButtons.Length > 0);
    }

    /// <summary>
    /// Toggle on next page
    /// </summary>
    public void NextMenuPage()
    {
        int level = start_with_level + levelBoxButtons.Length;
        if (level <= count_levels)
            start_with_level = level;
        UpdateMenuPage();
    }

    /// <summary>
    /// Toggle on prev page
    /// </summary>
    public void PrevMenuPage()
    {
        int level = start_with_level - levelBoxButtons.Length;
        if (level > 0)
            start_with_level = level;
        UpdateMenuPage();
    }

    /// <summary>
    /// Handle level select
    /// </summary>
    /// <param name="levelBox"> level button </param>
    /// <param name="level"> level number </param>
    public void LevelBoxSelect(Button levelBox, int level)
    {
        int opened_level = PlayerPrefs.GetInt(levelOpenedPref, 1);

        if (level > opened_level)
        {
            selected_level = 0;
            if (eventSystem) eventSystem.SetSelectedGameObject(null);                   // remove selection
        }
        else
        {
            selected_level = level;
        }
    }

    /// <summary>
    /// Play selected level
    /// </summary>
    public void PlaySelectedLevel()
    {
        if (selected_level <= 0) return;                                                                // if level selected

        if (mainMenuControl) 
            mainMenuControl.LoadingMenu();                                                              // open loading menu

        PlayerPrefs.SetInt(levelPref, selected_level);                                                  // set level for playing with level

        StartCoroutine(ExecWithWait(() => SceneManager.LoadScene(gameSceneName), waitForSecGame));      // load game scene
    }
}
