using UnityEngine;

/// <summary>
/// AudioPlayer
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    public AudioSource[] musicList;
    private int audio_ind = 0;
    private bool is_playing = false;

    public bool isPlaying { get { return is_playing; } }

    void Update()
    {
        if (is_playing && !musicList[audio_ind].isPlaying)
            Next();
    }

    public void Play()
    {
        Play(0);
    }

    public void Play(int start_ind)
    {
        musicList[audio_ind].Stop();
        audio_ind = validAudioIndex(start_ind);
        musicList[audio_ind].Play();
        is_playing = true;
    }

    public void Next()
    {
        musicList[audio_ind].Stop();
        audio_ind++;
        if (audio_ind >= musicList.Length)
            audio_ind = 0;
        musicList[audio_ind].Play();
        is_playing = true;
    }

    public void Prev()
    {
        musicList[audio_ind].Stop();
        audio_ind--;
        if (audio_ind < 0)
            audio_ind = musicList.Length - 1;
        musicList[audio_ind].Play();
        is_playing = true;
    }

    public void Stop()
    {
        musicList[audio_ind].Stop();
        is_playing = false;
    }

    public void Pause()
    {
        musicList[audio_ind].Pause();
        is_playing = false;
    }

    public void UnPause()
    {
        musicList[audio_ind].UnPause();
        is_playing = true;
    }

    private int validAudioIndex(int index)
    {
        if (index < 0)
            return 0;
        else if (index >= musicList.Length)
            return musicList.Length - 1;
        else
            return index;
    }
}
