using UnityEngine;



/// <summary>
/// MainMenuControl is class for control main menu
/// </summary>
public class MainMenuControl : MonoBehaviour
{
    public AudioPlayer audioPlayer;                                     // Audio player in menu

    // *************** All menu objects ******************
    public GameObject mainMenu;                                         // main menu
    public GameObject settingsMenu;                                     // settings menu
    public GameObject selectLevelMenu;                                  // select level menu
    // ***************************************************

    // ************ preference names **********************
    public const string levelPref = "level";                            // preference name that store number of level
    // ****************************************************

    public const string gameSceneName = "GameScene";

    void Start()
    {
        if (audioPlayer) audioPlayer.Play();                                                           // start music list playing

        MainMenu();                                                                                    // open main menu
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        CallMenu(mainMenu, settingsMenu, selectLevelMenu);
    }

    public void SettingMenu()
    {
        CallMenu(settingsMenu, mainMenu, selectLevelMenu);
    }

    public void SelectLevelMenu()
    {
        CallMenu(selectLevelMenu, settingsMenu, mainMenu);
    }

    public void CallMenu(GameObject open_menu, params GameObject[] close_menus)
    {
        open_menu.SetActive(true);
        foreach (GameObject close_menu in close_menus)
            close_menu.SetActive(false);
    }
}
