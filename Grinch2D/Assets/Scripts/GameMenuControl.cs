using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameMenuControl : MonoBehaviour
{
    public GameSceneHandler gameScnHnd;

    public Sprite[] starBarSprites;

    // *************** All menu objects ******************
    public GameObject gamePlayMenu;                                     // game play menu
    public GameObject gameUnplayMenu;                                   // game unplay menu
    public GameObject loadingMenu;                                      // loading menu
    // ***************************************************

    // ************* Game unplay menu elements ***********
    // ********* { WinMenu, LoseMenu, PauseMenu } ********
    public Text gmUnplHeader;                                           // game unplay menu header
    public Text gmUnplTimeBar;                                          // game unplay menu time bar
    public Text gmUnplLevelBar;                                         // game unplay menu level bar
    public Image gmUnplStarBar;                                         // game unplay menu star bar
    public Button gmUnplBt;                                             // game unplay menu button
    public Text gmUnplBtText;                                           // game unplay menu button text

    public const string pauseHdrText = "Pause";                         // game unplay menu header text
    public const string winHdrText = "You Win";
    public const string loseHdrText = "You Lose";

    public const string pauseBtText = "Resume";                         // game unplay menu button text
    public const string winBtText = "Next";
    // ***************************************************

    public const string timeBarText = "Time ";

    // ************* Game play menu elements ***********
    public Image gameStarBar;                                           // game play menu star bar
    public Text gameTimeBar;                                            // game play menu time bar
    // ***************************************************

    public float time = 0f;

    private GameSceneHandler.GameSceneEventHnd on_construct_level = null;

    void Start()
    {
        LoadingMenu();                                                                                  // open loading menu

        gameScnHnd.OnInited += () => gameScnHnd.ConstructLevel(PlayerPrefs.GetInt("level", 5));
        gameScnHnd.OnConstructedLevel += StartGame;

        gameScnHnd.Init();
    }

    void Update()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started)                                     // update game play info
        {
            UpdateStarBar(gameStarBar);
            UpdateTimeBar(gameTimeBar);
        }
    }

    public void StartGame()
    {
        GameMenu();
        gameScnHnd.StartGame();
    }

    public void PauseGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started)
        {
            PauseMenu();
            gameScnHnd.StopGame(true);
        }
    }

    public void LoseGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started)
        {
            LoseMenu();
            gameScnHnd.StopGame(true);
        }
    }

    public void WinGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started)
        {
            WinMenu();
            gameScnHnd.StopGame(true);
        }
    }

    public void ResumeGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Stoped)
        {
            GameMenu();
            gameScnHnd.StopGame(false);
        }
    }

    public void NextLevel()
    {
        int level = gameScnHnd.CurrentLevel + 1;
        if (level > gameScnHnd.CountLevels) return;

        LoadingMenu();

        if (on_construct_level != null) 
            gameScnHnd.OnDeconstructedLevel -= on_construct_level;

        on_construct_level = () => gameScnHnd.ConstructLevel(level);

        gameScnHnd.OnDeconstructedLevel += on_construct_level;

        gameScnHnd.DeconstructLevel();
    }

    public void UpdateStarBar(Image starBar)
    {
        int count_stars = gameScnHnd.CountStars - 1;
        if (count_stars < 0)
        {
            starBar.gameObject.SetActive(false);
        }
        else
        {
            starBar.gameObject.SetActive(true);
            if (count_stars >= starBarSprites.Length)
                count_stars = starBarSprites.Length - 1;
            starBar.sprite = starBarSprites[count_stars];
        }
    }

    public void UpdateTimeBar(Text timeBar)
    {
        Debug.Log(SecondsToTimeStr(time));
        timeBar.text = timeBarText + SecondsToTimeStr(gameScnHnd.GameTime);
    }

    public void PauseMenu()
    {
        CallMenu(false, true, false);
        CallSubMenu(pauseHdrText, pauseBtText, true, ResumeGame);
    }

    public void WinMenu()
    {
        CallMenu(false, true, false);
        CallSubMenu(winHdrText, winBtText, true, NextLevel);
    }

    public void LoseMenu()
    {
        CallMenu(false, true, false);
        CallSubMenu(loseHdrText, null, false, null);
    }

    public void GameMenu()
    {
        CallMenu(true, false, false);
    }

    public void LoadingMenu()
    {
        CallMenu(false, false, true);
    }

    public void CallMenu(bool gameMenuActive, bool subMenuActive, bool loadMenuActive)
    {
        gamePlayMenu.SetActive(gameMenuActive);
        gameUnplayMenu.SetActive(subMenuActive);
        loadingMenu.SetActive(loadMenuActive);
    }

    public void CallSubMenu(string hdrText, string upBtText, bool upBtActive, UnityEngine.Events.UnityAction btAction)
    {
        gmUnplHeader.text = hdrText;

        gmUnplBt.gameObject.SetActive(upBtActive);
        if (upBtActive)
        {
            gmUnplBtText.text = upBtText;

            gmUnplBt.onClick.RemoveAllListeners();
            gmUnplBt.onClick.AddListener(btAction);
        }

        UpdateStarBar(gmUnplStarBar);
        UpdateTimeBar(gmUnplTimeBar);
    }

    public static string SecondsToTimeStr(float time)
    {
        int min = (int)time / 60;
        int sec = (int)time % 60;

        if (min > 99)
        {
            min = 99;
            sec = 59;
        }

        string NumTo2DigStr(int num)
        {
            return num < 10 ? "0" + num : num.ToString();
        }

        return NumTo2DigStr(min) + ":" + NumTo2DigStr(sec);
    }
}
