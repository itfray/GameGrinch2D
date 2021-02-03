﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameMenuControl : MonoBehaviour
{
    public GameSceneHandler gameScnHnd;                                 // Game Scene Handler
    public float waitForSecWin = 0.5f;                                  // wait for seconds of win
    public float waitForSecLose = 1f;                                   // wait for seconds of lose
    public float waitForSecMainMenu = 1f;                               // wait for secinds of transition to main menu

    public Sprite[] starBarSprites;                                     // sprites for star bar

    // *************** All menu objects ******************
    public GameObject gamePlayMenu;                                     // game play menu
    public GameObject gameUnplayMenu;                                   // game unplay menu
    public GameObject loadingMenu;                                      // loading menu
    public GameObject settingMenu;                                      // setting menu
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
    public const string levelBarText = "Level ";

    public const string levelPrefName = "level";                        // name preference that store number of level

    // ************* Game play menu elements ***********
    public Image gameStarBar;                                           // game play menu star bar
    public Text gameTimeBar;                                            // game play menu time bar
    // ***************************************************

    public const string mainMenuSceneName = "MainMenuScene";

    public enum GameMenuState { Loading, Pause, Lose, Win, Game };      // game menu state
    private GameMenuState menu_state;                                   // menu state
    public GameMenuState MenuState { get { return menu_state; } }


    private GameSceneHandler.GameSceneEventHnd ConstructLevel = null;       // pointer on ConstructLevel method
    void Start()
    {
        LoadingMenu();                                                                                  // open loading menu

        gameScnHnd.OnInited += () => gameScnHnd.ConstructLevel(PlayerPrefs.GetInt(levelPrefName, 1));   // add callback after initialization of game scene handler

        gameScnHnd.OnConstructedLevel += () =>
        {
            gameScnHnd.OnWined += () => 
            {
                menu_state = GameMenuState.Win;
                StartCoroutine(ExecWithWait(WinGame, waitForSecWin));                                   // add callback after how player became winner
            };

            gameScnHnd.OnLosed += () => 
            {
                menu_state = GameMenuState.Lose;
                StartCoroutine(ExecWithWait(LoseGame, waitForSecLose));                                 // add callback after how player became losser
            };           
        };

        gameScnHnd.OnConstructedLevel += StartGame;                                                     // add callback after construction of level

        gameScnHnd.Init();                                                                              // run game scene handler initialization
    }

    void Update()
    {
        HandleInput();                                                                                  // handles input

        if (gameScnHnd.State == GameSceneHandler.GameState.Started)                                     // update game play info
        {
            UpdateStarBar(gameStarBar, true);                                                           // player star bar
            UpdateTimeBar(gameTimeBar);                                                                 // player time bar
        }
    }

    /// <summary>
    /// Method handles menu input
    /// </summary>
    public void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu_state == GameMenuState.Game)
                PauseGame();
            else if (menu_state == GameMenuState.Pause)
                ResumeGame();
        }
    }

    /// <summary>
    /// Method for start/restart game
    /// </summary>
    public void StartGame()
    {
        GameMenu();                                                                     // open game menu
        gameScnHnd.StartGame();
    }

    /// <summary>
    /// Method for pause game
    /// </summary>
    public void PauseGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started &&                   // if game scene handler started
            (menu_state == GameMenuState.Game || menu_state == GameMenuState.Pause))
        {
            PauseMenu();                                                                // open pause menu
            gameScnHnd.StopGame(true);                                                  // stop game
        }
    }

    /// <summary>
    /// Method for lose game
    /// </summary>
    public void LoseGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started &&
            (menu_state == GameMenuState.Game || menu_state == GameMenuState.Lose))
        {
            LoseMenu();
            gameScnHnd.StopGame(true);
        }
    }

    /// <summary>
    /// Method for win game
    /// </summary>
    public void WinGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Started &&
            (menu_state == GameMenuState.Game || menu_state == GameMenuState.Win))
        {
            WinMenu();
            gameScnHnd.StopGame(true);
        }
    }

    /// <summary>
    /// Method for resume game
    /// </summary>
    public void ResumeGame()
    {
        if (gameScnHnd.State == GameSceneHandler.GameState.Stoped &&
            menu_state != GameMenuState.Game)
        {
            GameMenu();                                                                    // run game menu
            gameScnHnd.StopGame(false);                                                    // start game
        }
    }

    /// <summary>
    /// Method turns on next level in game
    /// </summary>
    public void NextLevel()
    {
        int level = gameScnHnd.CurrentLevel + 1;
        if (level > gameScnHnd.CountLevels) return;                                         // is there next level in game?

        LoadingMenu();                                                                      // open loading menu

        if (ConstructLevel != null) 
            gameScnHnd.OnDeconstructedLevel -= ConstructLevel;                              // delete old handler

        ConstructLevel = () => gameScnHnd.ConstructLevel(level);                            // create ConstructLevel handler

        gameScnHnd.OnDeconstructedLevel += ConstructLevel;                                  // add callback after deconstruction of level

        gameScnHnd.DeconstructLevel();                                                      // run deconstruction of level
    }

    /// <summary>
    /// Method for updating image in star bar
    /// </summary>
    /// <param name="starBar"> image of star bar </param>
    public void UpdateStarBar(Image starBar, bool active)
    {
        if (!active)
        {
            starBar.gameObject.SetActive(false);
            return;
        }

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
            starBar.sprite = starBarSprites[count_stars];                                   // set sprite from sprites list for image of star bar
        }
    }

    /// <summary>
    /// Method for updating text in time bar
    /// </summary>
    /// <param name="timeBar"> text of time bar </param>
    public void UpdateTimeBar(Text timeBar)
    {
        timeBar.text = timeBarText + SecondsToTimeStr(gameScnHnd.GameTime);
    }

    /// <summary>
    /// Method for updating text in level bar
    /// </summary>
    /// <param name="levelBar"> text component of level bar </param>
    public void UpdateLevelBar(Text levelBar)
    {
        levelBar.text = levelBarText + " " + gameScnHnd.CurrentLevel;
    }

    /// <summary>
    /// Method opens pause menu
    /// </summary>
    public void PauseMenu()
    {
        CallMenu(GameMenuState.Pause, gameUnplayMenu, gamePlayMenu, loadingMenu, settingMenu);
        CallGmUnplMenu(pauseHdrText, true, pauseBtText, true, ResumeGame);
    }

    /// <summary>
    /// Method opens win menu
    /// </summary>
    public void WinMenu()
    {   
        CallMenu(GameMenuState.Win, gameUnplayMenu, gamePlayMenu, loadingMenu, settingMenu);

        int level = gameScnHnd.CurrentLevel + 1;
        if (level <= gameScnHnd.CountLevels)
            CallGmUnplMenu(winHdrText, true, winBtText, true, NextLevel);
        else
            CallGmUnplMenu(winHdrText, true, null, false, null);

    }

    /// <summary>
    /// Method opens lose menu
    /// </summary>
    public void LoseMenu()
    {
        CallMenu(GameMenuState.Lose, gameUnplayMenu, gamePlayMenu, loadingMenu, settingMenu);
        CallGmUnplMenu(loseHdrText, false, null, false, null);
    }

    /// <summary>
    /// Method opens game menu
    /// </summary>
    public void GameMenu()
    {
        CallMenu(GameMenuState.Game, gamePlayMenu, loadingMenu, gameUnplayMenu, settingMenu);
    }

    /// <summary>
    /// Method opens loading menu
    /// </summary>
    public void LoadingMenu()
    {
        CallMenu(GameMenuState.Loading, loadingMenu, gamePlayMenu, gameUnplayMenu, settingMenu);
    }

    /// <summary>
    /// Method opens setting menu
    /// </summary>
    public void SettingMenu()
    {
        CallMenu(menu_state, settingMenu, loadingMenu, gamePlayMenu, gameUnplayMenu);
    }

    /// <summary>
    /// Method opens main menu
    /// </summary>
    public void MainMenu()
    {
        LoadingMenu();
        StartCoroutine(ExecWithWait(() => SceneManager.LoadScene(mainMenuSceneName), waitForSecMainMenu));
    }

    /// <summary>
    /// Method closes sub menu of game menu
    /// </summary>
    public void CloseSubGameMenu()
    {
        switch (menu_state)
        {
            case GameMenuState.Loading:
                LoadingMenu();
                return;
            case GameMenuState.Game:
                GameMenu();
                return;
            case GameMenuState.Pause:
                PauseMenu();
                return;
            case GameMenuState.Lose:
                LoseMenu();
                return;
            case GameMenuState.Win:
                WinMenu();
                return;
        }
    }

    /// <summary>
    /// Method opens specified menu
    /// </summary>
    /// <param name="state"> menu state </param>
    /// <param name="open_menu"> menu, that will open </param>
    /// <param name="close_menus"> all menus, that will close </param>
    public void CallMenu(GameMenuState state, GameObject open_menu, params GameObject[] close_menus)
    {
        open_menu.SetActive(true);
        foreach (GameObject close_menu in close_menus)
            close_menu.SetActive(false);
        menu_state = state;
    }

    /// <summary>
    /// Method opens specified game unplay menu
    /// </summary>
    /// <param name="hdrText"> text of header of menu </param>
    /// <param name="btText"> text of button of menu </param>
    /// <param name="btActive"> flag of activation of button of menu</param>
    /// <param name="btAction"> handler for button of menu </param>
    public void CallGmUnplMenu(string hdrText, bool starBarActive, string btText, bool btActive, UnityEngine.Events.UnityAction btAction)
    {
        gmUnplHeader.text = hdrText;                                                        // change text of header

        gmUnplBt.gameObject.SetActive(btActive); 
        if (btActive)
        {
            gmUnplBtText.text = btText;                                                     // change text of button

            gmUnplBt.onClick.RemoveAllListeners();                                          // clear all handlers
            gmUnplBt.onClick.AddListener(btAction);                                         // set new handler
        }

        UpdateStarBar(gmUnplStarBar, starBarActive);                                        // updating star bar
        UpdateTimeBar(gmUnplTimeBar);                                                       // updating time bar
        UpdateLevelBar(gmUnplLevelBar);                                                     // updating level bar
    }

    /// <summary>
    /// Method transform float seconds value in time string
    /// </summary>
    /// <param name="time"> time in seconds </param>
    /// <returns> time string format: "mm:ss" </returns>
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

    /// <summary>
    /// Method executes specified method after specified number of seconds
    /// </summary>
    /// <param name="action"> invoked method </param>
    /// <param name="seconds"> wait for seconds </param>
    /// <returns> IEnumerator </returns>
    public static IEnumerator ExecWithWait(System.Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        action?.Invoke();
    }
}
