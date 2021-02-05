using UnityEngine;
using static CoroutineScripts;



/// <summary>
/// MainMenuControl is class for control main menu
/// </summary>
public class MainMenuControl : MonoBehaviour
{
    public AudioPlayer audioPlayer;                                     // Audio player in menu
    public SelectMenuControl selectMenuCntrl;                           // Select menu control

    // *************** All menu objects ******************
    public GameObject mainMenu;                                         // main menu
    public GameObject settingsMenu;                                     // settings menu
    public GameObject selectLevelMenu;                                  // select level menu
    public GameObject loadingMenu;                                      // loading menu
    // ***************************************************

    // *************** All dialog objects ******************
    public GameObject resetProgressDialog;                              // reset game progress dialog
    public GameObject notifyProgressDialog;                             // notify if game progress reseted
    // ***************************************************

    public float waitForSecNotify = 1f;                                 // duration of notification

    void Start()
    {
        if (audioPlayer) audioPlayer.Play(Random.Range(0, audioPlayer.musicList.Length - 1));                        // start music list playing

        selectMenuCntrl.OnResetProgress += delegate                                                                  // add notification in callback
        {
            notifyProgressDialog.SetActive(true);                                                                    // show notification
            StartCoroutine(ExecWithWait(() => notifyProgressDialog.SetActive(false), waitForSecNotify));             // close notification
        };

        MainMenu();                                                                                                  // open main menu
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        CallMenu(mainMenu, settingsMenu, selectLevelMenu, loadingMenu);
    }

    public void SettingMenu()
    {
        CallMenu(settingsMenu, mainMenu, selectLevelMenu, loadingMenu, resetProgressDialog, notifyProgressDialog);
    }

    public void SelectLevelMenu()
    {
        CallMenu(selectLevelMenu, settingsMenu, mainMenu, loadingMenu);
        selectMenuCntrl.UpdateMenuPage();
    }

    public void LoadingMenu()
    {
        CallMenu(loadingMenu, selectLevelMenu, settingsMenu, mainMenu);
    }

    public void CallMenu(GameObject open_menu, params GameObject[] close_menus)
    {
        open_menu.SetActive(true);
        foreach (GameObject close_menu in close_menus)
            close_menu.SetActive(false);
    }
}
