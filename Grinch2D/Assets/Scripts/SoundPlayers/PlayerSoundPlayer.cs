using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// PlayerSoundPlayer is class for playing player action sounds
/// </summary>
public class PlayerSoundPlayer : SoundPlayer
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

    private Dictionary<string, AudioClip> diedSoundDict = new Dictionary<string, AudioClip>();


    void Start()
    {
        AddDiedSounds();
        AddStarSounds();
        AddGiftSounds();
    }

    void FillDiedSoundDict()
    {
        if (dieFromSawSound)
        {
            if (sawObject) diedSoundDict.Add(sawObject.tag, dieFromSawSound);
            if (bigSawObject) diedSoundDict.Add(bigSawObject.tag, dieFromSawSound);
            if (movingSawObject) diedSoundDict.Add(movingSawObject.tag, dieFromSawSound);
        }
    }

    void AddDiedSounds()
    {
        if (playerHealthHnd == null) return;

        if (dieSound)
            playerHealthHnd.OnDied += (obj) => PlaySound(dieSound, transform.position);                // add playing of sound in callback

        FillDiedSoundDict();

        if (diedSoundDict.Count > 0)
        {
            playerHealthHnd.OnDied += delegate (GameObject killer)
            {
                AudioClip kill_sound;
                if (diedSoundDict.TryGetValue(killer.tag, out kill_sound))
                    PlaySound(kill_sound, killer.transform.position);
            };
        }
    }

    void AddGiftSounds()
    {
        if (playerGiftHnd == null) return;

        if (takeGiftSound)
            playerGiftHnd.OnTaked += () => PlaySound(takeGiftSound, transform.position);
    }

    void AddStarSounds()
    {
        if (playerStarHnd == null) return;

        if (takeStarSound)
            playerStarHnd.OnTaked += () => PlaySound(takeStarSound, transform.position);           // add playing of sound in callback
    }
}
