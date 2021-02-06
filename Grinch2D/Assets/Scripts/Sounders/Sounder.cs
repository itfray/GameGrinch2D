using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Sounder is class for playing object action sounds
/// </summary>
public abstract class Sounder : MonoBehaviour
{
    protected List<AudioSource> audios = new List<AudioSource>();               // all sounds of object

    void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        foreach (AudioSource audio in audios)
        {
            if (audio) audio.transform.parent = null;                           // replace in root hierarchy
        }
    }

    void OnDestroy()
    {
        foreach (AudioSource audio in audios)
        {
            if (audio) Destroy(audio.gameObject);                              // destroy all sounds of game object
        }
    }
}
