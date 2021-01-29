using UnityEngine;


public class GameMenuControl : MonoBehaviour
{
    public GameObject gameMenu;
    public GameObject subMenu;
    public GameObject loadingMenu;

    public SubMenuControl subMenuCntrl;

    public void PauseMenu()
    {
        CallMenu(false, true, false);
        subMenuCntrl.PauseMenu();
    }

    public void WinMenu()
    {
        CallMenu(false, true, false);
        subMenuCntrl.WinMenu();
    }

    public void LoseMenu()
    {
        CallMenu(false, true, false);
        subMenuCntrl.LoseMenu();
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
}
