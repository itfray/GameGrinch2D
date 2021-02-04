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
    // ***************************************************

    // ************ preference names **********************
    public const string levelPref = "level";                            // preference name that store number of level
    // ****************************************************

    public const string gameSceneName = "GameScene";

    void Start()
    {
        if (audioPlayer) audioPlayer.Play();                                                           // start music list playing
    }
}
