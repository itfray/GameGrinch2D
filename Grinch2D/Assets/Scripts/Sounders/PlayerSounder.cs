using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// PlayerSounder is class for playing player action sounds
/// </summary>
public class PlayerSounder : MonoBehaviour
{
    public AudioClip takeStarSound;                                                                     // sound getting of star
    public AudioClip takeGiftSound;                                                                     // sound getting of gift
    public AudioClip dieSound;                                                                          // sound of player death

    public HealthHandler playerHealthHnd;                                                               // handler of player health
    public StarHandler playerStarHnd;                                                                   // handler stars
    public GiftHandler playerGiftHnd;                                                                   // handler gifts

    public AudioClip dieFromSawSound;                                                                   // sound of player death from saw
    public GameObject sawObject;                                                                        // example saw
    public GameObject bigSawObject;                                                                     // example big saw
    public GameObject movingSawObject;                                                                  // example moving saw

    private Dictionary<string, AudioClip> diedSoundDict = new Dictionary<string, AudioClip>();          // dicitionary of audio clips for appointing sounds for death ways


    void Start()
    {
        AddDiedSounds();
        AddStarSounds();
        AddGiftSounds();
    }

    /// <summary>
    /// Method for filling diedSoundDict
    /// </summary>
    void FillDiedSoundDict()
    {
        if (dieFromSawSound)                                                                                            // add sound of death from saw
        {
            if (sawObject) diedSoundDict.Add(sawObject.tag, dieFromSawSound);                                           // add this sound and tag of sawObject if killer is sawObject
            if (bigSawObject) diedSoundDict.Add(bigSawObject.tag, dieFromSawSound);
            if (movingSawObject) diedSoundDict.Add(movingSawObject.tag, dieFromSawSound);                               // add this sound and tag of movingSawObject if killer is movingSawObject
        }
    }

    /// <summary>
    /// Method for addition sounds for player death event
    /// </summary>
    void AddDiedSounds()
    {
        if (playerHealthHnd == null) return;

        if (dieSound)                                                                                                    // add death sound for death event
            playerHealthHnd.OnDied += (obj) => AudioSource.PlayClipAtPoint(dieSound, transform.position);                // add playing of sound in callback

        FillDiedSoundDict();                                                                                             // fill diedSoundDict

        if (diedSoundDict.Count > 0)
        {
            playerHealthHnd.OnDied += delegate (GameObject killer)
            {
                AudioClip kill_sound;
                if (diedSoundDict.TryGetValue(killer.tag, out kill_sound))                                              // search of death sound of killer object by tag
                    AudioSource.PlayClipAtPoint(kill_sound, killer.transform.position);                                 
            };
        }
    }

    /// <summary>
    /// Method for addition sounds for event of gift taking
    /// </summary>
    void AddGiftSounds()
    {
        if (playerGiftHnd == null) return;

        if (takeGiftSound)
            playerGiftHnd.OnTaked += () => AudioSource.PlayClipAtPoint(takeGiftSound, transform.position);
    }

    /// <summary>
    /// Method for addition sounds for event of star taking
    /// </summary>
    void AddStarSounds()
    {
        if (playerStarHnd == null) return;

        if (takeStarSound)
            playerStarHnd.OnTaked += () => AudioSource.PlayClipAtPoint(takeStarSound, transform.position);           // add playing of sound in callback
    }
}
