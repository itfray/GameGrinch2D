using UnityEngine;

/// <summary>
/// AudioPlayer is class implements the logic of the music list player
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    public AudioSource[] musicList;                                                 // list of audio compositions
    private int audio_ind = 0;                                                      // index of last playing audio
    private bool is_playing = false;                                                // Is palyer play music in this moment?

    public string audioIndexPrefName = "";

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

    private void PlayWith(int index)
    {
        musicList[audio_ind].Stop();                                // stop old audio
        audio_ind = index;                                          // transform index to valid state
        musicList[audio_ind].Play();                                // play new audio
        is_playing = true;

        if (audioIndexPrefName.Length > 0)
            PlayerPrefs.SetInt(audioIndexPrefName, audio_ind);      // store audio index as preference
    }

    /// <summary>
    /// Method starts playing music list
    /// </summary>
    public void Play()
    {
        Play(PlayerPrefs.GetInt(audioIndexPrefName, 0));
    }

    /// <summary>
    /// Method starts playing music list start with specified index
    /// </summary>
    /// <param name="start_ind"> audio index </param>
    public void Play(int start_ind)
    {
        PlayWith(ValidAudioIndex(start_ind));
        OnPlay?.Invoke();
    }

    /// <summary>
    /// Method starts playing music list start with next audio index
    /// </summary>
    public void Next()
    {
        PlayWith(NextAudioIndex());
        OnNext?.Invoke();
    }

    /// <summary>
    /// Method starts playing music list start with prev audio index
    /// </summary>
    public void Prev()
    {
        PlayWith(PrevAudioIndex());
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
    /// Method for transformation any audio index to valid audio index
    /// </summary>
    /// <param name="index"> audio index </param>
    /// <returns> valid audio index </returns>
    public int ValidAudioIndex(int index)
    {
        if (index < 0)
            return 0;
        else if (index >= musicList.Length)
            return musicList.Length - 1;
        else
            return index;
    }

    /// <summary>
    /// Method calculates next audio index  in list of music
    /// </summary>
    /// <returns> next audio index </returns>
    public int NextAudioIndex()
    {
        int index = audio_ind + 1;
        if (index >= musicList.Length)
            index = 0;
        return index;
    }

    /// <summary>
    /// Method calculates prev audio index  in list of music
    /// </summary>
    /// <returns> prev audio index </returns>
    public int PrevAudioIndex()
    {
        int index = audio_ind - 1;
        if (index < 0)
            index = musicList.Length - 1;
        return index;
    }
}
