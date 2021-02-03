using UnityEngine;

/// <summary>
/// AudioPlayer is class implements the logic of the music list player
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    public AudioSource[] musicList;                                                 // list of audio compositions
    private int audio_ind = 0;                                                      // index of last playing audio
    private bool is_playing = false;                                                // Is palyer play music in this moment?

    public delegate void AudioPlayerEvent();                                        // type handler of events of AudioPlayer
    public event AudioPlayerEvent OnPlay;                                           // invoke when AudioPlayer execute "Play"
    public event AudioPlayerEvent OnStop;
    public event AudioPlayerEvent OnNext;
    public event AudioPlayerEvent OnPrev;
    public event AudioPlayerEvent OnPause;
    public event AudioPlayerEvent OnUnPause;

    public bool isPlaying { get { return is_playing; } }
    public int audioIndex { get { return audio_ind; } }

    void Update()
    {
        if (is_playing && !musicList[audio_ind].isPlaying)
            Next();
    }

    /// <summary>
    /// Method starts playing music list
    /// </summary>
    public void Play()
    {
        Play(0);
    }

    /// <summary>
    /// Method starts playing music list start with specified index
    /// </summary>
    /// <param name="start_ind"> audio index </param>
    public void Play(int start_ind)
    {
        musicList[audio_ind].Stop();                                            // stop old audio
        audio_ind = validAudioIndex(start_ind);                                 // transform index to valid state
        musicList[audio_ind].Play();                                            // play new audio
        is_playing = true;

        OnPlay?.Invoke();
    }

    /// <summary>
    /// Method starts playing music list start with next audio index
    /// </summary>
    public void Next()
    {
        musicList[audio_ind].Stop();
        audio_ind++;
        if (audio_ind >= musicList.Length)
            audio_ind = 0;
        musicList[audio_ind].Play();
        is_playing = true;

        OnNext?.Invoke();
    }

    /// <summary>
    /// Method starts playing music list start with prev audio index
    /// </summary>
    public void Prev()
    {
        musicList[audio_ind].Stop();
        audio_ind--;
        if (audio_ind < 0)
            audio_ind = musicList.Length - 1;
        musicList[audio_ind].Play();
        is_playing = true;

        OnPrev?.Invoke();
    }

    /// <summary>
    /// Method stops playing music list
    /// </summary>
    public void Stop()
    {
        musicList[audio_ind].Stop();
        is_playing = false;

        OnStop?.Invoke();
    }

    /// <summary>
    /// Method pauses playing music list
    /// </summary>
    public void Pause()
    {
        musicList[audio_ind].Pause();
        is_playing = false;

        OnPause?.Invoke();
    }

    /// <summary>
    /// Method unpauses playing music list
    /// </summary>
    public void UnPause()
    {
        musicList[audio_ind].UnPause();
        is_playing = true;

        OnUnPause?.Invoke();
    }

    /// <summary>
    /// Method for transformation any audio index to valid state
    /// </summary>
    /// <param name="index"> audio index </param>
    /// <returns> valid audio index </returns>
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
