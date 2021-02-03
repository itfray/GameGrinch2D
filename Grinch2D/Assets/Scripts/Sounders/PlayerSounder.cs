using UnityEngine;
using System.Collections.Generic;
using static CoroutineScripts;


/// <summary>
/// PlayerSounder is class for playing player action sounds
/// </summary>
public class PlayerSounder : MonoBehaviour
{
    public AudioSource takeStarSound;                                                                   // sound getting of star
    public AudioSource takeGiftSound;                                                                   // sound getting of gift
    public AudioSource dieSound;                                                                        // sound of player death

    public HealthHandler playerHealthHnd;                                                               // handler of player health
    public StarHandler playerStarHnd;                                                                   // handler stars
    public GiftHandler playerGiftHnd;                                                                   // handler gifts

    public AudioSource dieFromSawSound;                                                                 // sound of player death from saw
    public GameObject sawObject;                                                                        // example saw
    public GameObject bigSawObject;                                                                     // example big saw
    public GameObject movingSawObject;                                                                  // example moving saw

    Dictionary<string, AudioSource> diedSoundDict = new Dictionary<string, AudioSource>();              // dicitionary of audio clips for appointing sounds for death ways


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

        foreach (KeyValuePair<string, AudioSource> pair in diedSoundDict)
            pair.Value.transform.parent = null;
    }

    /// <summary>
    /// Method for addition sounds for player death event
    /// </summary>
    void AddDiedSounds()
    {
        if (playerHealthHnd == null) return;

        if (dieSound)                                                                                                    // add death sound for death event
        {
            dieSound.transform.parent = null;
            playerHealthHnd.OnDied += obj => dieSound.Play();                                                            // add playing of sound in callback
        }

        FillDiedSoundDict();                                                                                             // fill diedSoundDict

        if (diedSoundDict.Count > 0)
        {
            playerHealthHnd.OnDied += delegate (GameObject killer)
            {
                AudioSource kill_sound;
                if (diedSoundDict.TryGetValue(killer.tag, out kill_sound))                                              // search of death sound of killer object by tag
                    kill_sound.Play();
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
        {
            takeGiftSound.transform.parent = null;
            playerGiftHnd.OnTaked += () => takeGiftSound.Play();
        }
    }

    /// <summary>
    /// Method for addition sounds for event of star taking
    /// </summary>
    void AddStarSounds()
    {
        if (playerStarHnd == null) return;

        if (takeStarSound)
        {
            takeStarSound.transform.parent = null;
            playerStarHnd.OnTaked += () => takeStarSound.Play();                                                        // add playing of sound in callback
        }
    }

    void OnDestroy()
    {
        if (takeGiftSound) Destroy(takeGiftSound.gameObject);                                                           // destroy all of used sounds
        if (takeStarSound) Destroy(takeStarSound.gameObject);
        if (dieSound) Destroy(dieSound.gameObject);

        foreach (KeyValuePair<string, AudioSource> pair in diedSoundDict)
            Destroy(pair.Value.gameObject);
    }
}
