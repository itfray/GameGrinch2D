using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KeySounder is class for playing key object action sounds
/// </summary>
public class KeySounder : Sounder
{
    public AudioSource keySound;                                                    // key sound
    public AudioSource lockSound;                                                   // lock sound

    public KeyHandler keyHnd;                                                       // key handler

    protected override void Init()
    {
        if (keyHnd)
        {
            if (keySound)
            {
                keyHnd.OnDisappearKey += () => keySound.Play();                     // add playing sound of key object in callback
                audios.Add(keySound);
            }

            if (lockSound)
            {
                keyHnd.OnDisappearLockBlock += () => lockSound.Play();
                audios.Add(lockSound);
            }
        }

        base.Init();
    }
}
