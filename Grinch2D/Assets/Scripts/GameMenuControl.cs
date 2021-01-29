using UnityEngine;
using UnityEngine.UI;


public class GameMenuControl : MonoBehaviour
{
    public GameObject gameMenu;
    public GameObject subMenu;
    public GameObject loadingMenu;

    public Text header;
    public Text timeBar;
    public Text levelBar;

    public GameObject startBarObj;
    public Image starBar;

    public GameObject upButtonObj;
    public Button upButton;
    public Text upButtonText;


    public const string pauseHdrText = "Pause";
    public const string winHdrText = "You Win";
    public const string loseHdrText = "You Lose";
    public const string pauseBtText = "Resume";
    public const string winBtText = "Next";

    public GameSceneHandler gameScnHnd;

    public enum GameState { Loading, Game, Pause, Win, Lose };
    private GameState state;
    public GameState State { get { return state; } }

    void Start()
    {
        LoadingMenu();
        gameScnHnd.ConstructLevel(PlayerPrefs.GetInt("level", 5));

        /*
        gameScnHnd.StartGame();
        GameMenu();
        */
    }

    public void PauseMenu()
    {
        CallMenu(false, true, false);
        CallSubMenu(pauseHdrText, pauseBtText, true, true, ResumeHandler);
    }

    public void WinMenu()
    {
        CallMenu(false, true, false);
        CallSubMenu(winHdrText, winBtText, true, true, NextHandler);
    }

    public void LoseMenu()
    {
        CallMenu(false, true, false);
        CallSubMenu(loseHdrText, null, false, false, null);
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
        gameMenu.SetActive(gameMenuActive);
        subMenu.SetActive(subMenuActive);
        loadingMenu.SetActive(loadMenuActive);
    }

    public void CallSubMenu(string hdrText, string upBtText, bool upBtActive, bool starBarActive, UnityEngine.Events.UnityAction btAction)
    {
        header.text = hdrText;

        upButtonObj.SetActive(upBtActive);
        if (upBtActive)
        {
            upButtonText.text = upBtText;

            upButton.onClick.RemoveAllListeners();
            upButton.onClick.AddListener(btAction);
        }

        startBarObj.SetActive(starBarActive);
    }

    public void ResumeHandler()
    {
        GameMenu();
    }

    public void NextHandler()
    {
    }
}
