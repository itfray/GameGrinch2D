﻿using UnityEngine;



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
    public GameObject loadingMenu;                                      // loading menu
    // ***************************************************

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
        CallMenu(mainMenu, settingsMenu, selectLevelMenu, loadingMenu);
    }

    public void SettingMenu()
    {
        CallMenu(settingsMenu, mainMenu, selectLevelMenu, loadingMenu);
    }

    public void SelectLevelMenu()
    {
        CallMenu(selectLevelMenu, settingsMenu, mainMenu, loadingMenu);
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
