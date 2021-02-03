using System.Collections.Generic;
using UnityEngine;


public class AudioPlayer : MonoBehaviour
{
    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    private int icurr_audio = 0;

    void Update()
    {
    }

    public void Play()
    {
        icurr_audio = 0;
    }

    public void Play(int istart_audio)
    {
    }

    public void Stop()
    {
    }
}
