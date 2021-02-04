using UnityEngine;
using UnityEngine.UI;
using static MenuControlHelper;

public class SelectMenuControl : MonoBehaviour
{
    public LevelFileParser fileParser;

    public Button[] levelBoxButtons;
    public Text[] levelBoxTexts;
    public Image[] levelBoxLocks;
    public Image[] levelBoxStarBars;
    public Text[] levelBoxTimeBars;

    public Sprite[] starBarSprites;

    public Button nextButton;
    public Button prevButton;

    public const string levelPrefixPref = "level_";
    public const string levelStarSufixPref = "_stars";
    public const string levelTimeSufixPref = "_times";
    public const string levelOpenedPref = "level_opened";

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
            int level = ilvbox + start_with_level;
            bool active_box = level <= count_levels;
            levelBoxButtons[ilvbox].gameObject.SetActive(active_box);
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
}
