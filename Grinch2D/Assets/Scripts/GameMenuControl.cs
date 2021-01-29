using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameMenuControl : MonoBehaviour
{
    public GameSceneHandler gameScnHnd;

    public Sprite[] starBarSprites;

    // !------------------------ All menus -----------------!
    public GameObject gameMenu;
    public GameObject subMenu;
    public GameObject loadingMenu;
    // !-----------------------------------------------------!

    // !--------------- Sub menu elements ------------------!
    // !------- { WinMenu, LoseMenu, PauseMenu } -----------!
    public Text subMenuheader;
    public Text subTimeBar;
    public Text subLevelBar;
    public GameObject subStarBarObj;
    public Image subStarBar;
    public GameObject subUpBtObj;
    public Button subUpBt;
    public Text subUpBtText;

    public const string pauseHdrText = "Pause";
    public const string winHdrText = "You Win";
    public const string loseHdrText = "You Lose";
    public const string pauseBtText = "Resume";
    public const string winBtText = "Next";
    // !----------------------------------------------------!

    // !------------------ Game menu elements --------------!
    public GameObject gameStarBarObj;
    public Image gameStarBar;
    public Text gameTimeBar;
    // !----------------------------------------------------!

    public const string timeBarText = "Time ";

    public enum GameState { Loading, Game, Pause, Win, Lose };
    private GameState state;
    public GameState State { get { return state; } }

    void Start()
    {
        LoadingMenu();
        StartCoroutine(ConstructLevel());
        StartCoroutine(StartLevel());
    }

    void Update()
    {
        if (state == GameState.Game)
        {
            UpdateStarAndTimeBars(gameStarBarObj, gameStarBar, gameTimeBar);
        }
    }

    IEnumerator<object> ConstructLevel()
    {
        if (state != GameState.Loading) yield break;

        while (!gameScnHnd.Inited)
            yield return null;
        gameScnHnd.ConstructLevel(PlayerPrefs.GetInt("level", 5));
    }

    IEnumerator<object> StartLevel()
    {
        while (!gameScnHnd.Constructed)
            yield return null;
        StartGame();
    }

    public void StartGame()
    {
        GameMenu();
        gameScnHnd.StartGame();
    }

    public void PauseGame()
    {
        if (state == GameState.Game)
        {
            PauseMenu();
            gameScnHnd.StopGame(true);
        }
    }

    public void LoseGame()
    {
        if (state == GameState.Game)
        {
            LoseMenu();
            gameScnHnd.StopGame(true);
        }
    }

    public void WinGame()
    {
        if (state == GameState.Game)
        {
            WinMenu();
            gameScnHnd.StopGame(true);
        }
    }

    public void ResumeGame()
    {
        if (state == GameState.Pause)
        {
            GameMenu();
            gameScnHnd.StopGame(false);
        }
    }

    public void NextGame()
    {
        if (state == GameState.Win)
        {
            LoadingMenu();
            gameScnHnd.NextLevel();
            StartCoroutine(StartLevel());
        }
    }

    public void UpdateStarAndTimeBars(GameObject starBarObj, Image starBar, Text timeBar)
    {
        int count_stars = gameScnHnd.CountStars - 1;
        if (count_stars < 0)
        {
            starBarObj.SetActive(false);
        }
        else
        {
            starBarObj.SetActive(true);
            if (count_stars >= starBarSprites.Length)
                count_stars = starBarSprites.Length - 1;
            starBar.sprite = starBarSprites[count_stars];
        }

        float time = gameScnHnd.GameTime;
        int min = Mathf.RoundToInt(time / 60);
        int sec = Mathf.RoundToInt(time - min * 60);
        if (min > 99) min = 99;
        timeBar.text = timeBarText + min + ":" + sec;
    }

    public void PauseMenu()
    {
        CallMenu(false, true, false, GameState.Pause);
        CallSubMenu(pauseHdrText, pauseBtText, true, ResumeGame);
    }

    public void WinMenu()
    {
        CallMenu(false, true, false, GameState.Win);
        CallSubMenu(winHdrText, winBtText, true, NextGame);
    }

    public void LoseMenu()
    {
        CallMenu(false, true, false, GameState.Lose);
        CallSubMenu(loseHdrText, null, false, null);
    }

    public void GameMenu()
    {
        CallMenu(true, false, false, GameState.Game);
    }

    public void LoadingMenu()
    {
        CallMenu(false, false, true, GameState.Loading);
    }

    public void CallMenu(bool gameMenuActive, bool subMenuActive, bool loadMenuActive, GameState state)
    {
        gameMenu.SetActive(gameMenuActive);
        subMenu.SetActive(subMenuActive);
        loadingMenu.SetActive(loadMenuActive);
        this.state = state;
    }

    public void CallSubMenu(string hdrText, string upBtText, bool upBtActive, UnityEngine.Events.UnityAction btAction)
    {
        subMenuheader.text = hdrText;

        subUpBtObj.SetActive(upBtActive);
        if (upBtActive)
        {
            subUpBtText.text = upBtText;

            subUpBt.onClick.RemoveAllListeners();
            subUpBt.onClick.AddListener(btAction);
        }

        UpdateStarAndTimeBars(subStarBarObj, subStarBar, subTimeBar);
    }
}
