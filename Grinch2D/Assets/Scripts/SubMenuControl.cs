using UnityEngine;
using UnityEngine.UI;

public class SubMenuControl : MonoBehaviour
{
    public GameMenuControl gameMenuCntrl;
    public GameSceneHandler gameSceneHnd;

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

    public void PauseMenu()
    {
        CallMenu(pauseHdrText, pauseBtText, true, true, Resume);
    }

    public void WinMenu()
    {
        CallMenu(winHdrText, winBtText, true, true, Next);
    }

    public void LoseMenu()
    {
        CallMenu(loseHdrText, null, false, false, null);
    }

    public void CallMenu(string hdrText, string upBtText, bool upBtActive, bool starBarActive, UnityEngine.Events.UnityAction btAction)
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

    public void Resume()
    {
        gameMenuCntrl.GameMenu();
    }

    public void Next()
    {
    }
}
