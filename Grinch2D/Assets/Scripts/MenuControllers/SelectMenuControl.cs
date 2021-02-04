using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static MenuControlHelper;
using static CoroutineScripts;

public class SelectMenuControl : MonoBehaviour
{
    public LevelFileParser fileParser;
    public EventSystem eventSystem;
    public MainMenuControl mainMenuControl;

    public Button[] levelBoxButtons;
    public Text[] levelBoxTexts;
    public Image[] levelBoxLocks;
    public Image[] levelBoxStarBars;
    public Text[] levelBoxTimeBars;

    public Sprite[] starBarSprites;

    public Button nextButton;
    public Button prevButton;

    // ************ preference names **********************
    public const string levelPref = "level";                            // preference name that store number of level

    public const string levelPrefixPref = "level_";
    public const string levelStarSufixPref = "_stars";
    public const string levelTimeSufixPref = "_times";
    public const string levelOpenedPref = "level_opened";
    // ****************************************************

    public const string gameSceneName = "GameScene";

    public float waitForSecGame = 0.5f;                               // wait for seconds of transition to game

    private int start_with_level = 1;
    private int selected_level = 0;
    private int count_levels = 0;

    void Start()
    {
        if (fileParser)
            count_levels = fileParser.countLevels();

        UpdateMenuPage();
    }

    public void UpdateMenuPage()
    {
        int opened_level = PlayerPrefs.GetInt(levelOpenedPref, 1);

        for (int ilvbox = 0; ilvbox < levelBoxButtons.Length; ilvbox++)
        {
            Button levelbox = levelBoxButtons[ilvbox];
            int level = ilvbox + start_with_level;
            bool active_box = level <= count_levels;
            levelbox.gameObject.SetActive(active_box);
            if (!active_box) continue;

            levelBoxTexts[ilvbox].text = level.ToString();

            levelBoxLocks[ilvbox].gameObject.SetActive(level > opened_level);

            int count_stars = PlayerPrefs.GetInt(levelPrefixPref + level + levelStarSufixPref, 0);
            UpdateSpriteStarBar(count_stars, levelBoxStarBars[ilvbox], starBarSprites);

            float game_time = PlayerPrefs.GetFloat(levelPrefixPref + level + levelTimeSufixPref, -1f);
            bool active_timebar = game_time >= 0;
            if (active_timebar)
                levelBoxTimeBars[ilvbox].text = SecondsToTimeStr(game_time);
            levelBoxTimeBars[ilvbox].gameObject.SetActive(active_timebar);

            levelbox.onClick.RemoveAllListeners();
            levelbox.onClick.AddListener(() => LevelBoxSelect(levelbox, level));
        }

        if (nextButton) nextButton.gameObject.SetActive(start_with_level + levelBoxButtons.Length <= count_levels);
        if (prevButton) prevButton.gameObject.SetActive(start_with_level - levelBoxButtons.Length > 0);
    }

    public void NextMenuPage()
    {
        int level = start_with_level + levelBoxButtons.Length;
        if (level <= count_levels)
            start_with_level = level;
        UpdateMenuPage();
    }

    public void PrevMenuPage()
    {
        int level = start_with_level - levelBoxButtons.Length;
        if (level > 0)
            start_with_level = level;
        UpdateMenuPage();
    }

    public void LevelBoxSelect(Button levelBox, int level)
    {
        int opened_level = PlayerPrefs.GetInt(levelOpenedPref, 1);

        if (level > opened_level)
        {
            selected_level = 0;
            if (eventSystem) eventSystem.SetSelectedGameObject(null);
        }
        else
        {
            selected_level = level;
        }
    }

    public void PlaySelectedLevel()
    {
        if (selected_level <= 0) return;

        if (mainMenuControl) 
            mainMenuControl.LoadingMenu();

        PlayerPrefs.SetInt(levelPref, selected_level);

        StartCoroutine(ExecWithWait(() => SceneManager.LoadScene(gameSceneName), waitForSecGame));
    }
}
