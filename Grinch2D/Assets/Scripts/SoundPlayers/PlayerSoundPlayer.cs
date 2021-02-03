using UnityEngine;


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

    void Start()
    {
        if (playerHealthHnd)
        {
            if (dieSound)
                playerHealthHnd.OnDied += () => PlaySound(dieSound, transform.position);                // add playing of sound in callback
        }

        if (playerStarHnd)
        {
            if (takeStarSound)
                playerStarHnd.OnTaked += () => PlaySound(takeStarSound, transform.position);           // add playing of sound in callback
        }

        if (playerGiftHnd)
        {
            if (takeGiftSound)
                playerGiftHnd.OnTaked += () => PlaySound(takeGiftSound, transform.position);
        }
    }
}
