using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsMenuController : MonoBehaviour
{
    public AudioMixerGroup mixer;

    public float maxVolume = 0;                                                                 // max value volume in mixer
    public float minVolume = -80;                                                               // min value volume in mixer

    public const string musicVolParamName = "MusicVolume";
    public const string effectsVolParamName = "EffectsVolume";
    public const string masterVolParamName = "MasterVolume";

    public Image buttonMute;
    public Sprite onMuteSprite;
    public Sprite offMuteSprite;

    public Slider volumeSlider;
    public Toggle effectsToggle;
    public Toggle musicToggle;

    private bool mute = false;

    void Start()
    {
        float effectVolume = PlayerPrefs.GetFloat(effectsVolParamName, maxVolume);              // load volume preferences
        float musicVolume = PlayerPrefs.GetFloat(musicVolParamName, maxVolume);
        float masterVolume = PlayerPrefs.GetFloat(masterVolParamName, maxVolume);

        mixer.audioMixer.SetFloat(effectsVolParamName, effectVolume);                           // set volume preferences
        mixer.audioMixer.SetFloat(musicVolParamName, musicVolume);
        mixer.audioMixer.SetFloat(masterVolParamName, masterVolume);

        UpdateEffectsToggle(effectVolume);                                                      // update sound components view
        UpdateMusicToggle(musicVolume);
        UpdateVolumeSlider(masterVolume);
        UpdateMuteToggle(masterVolume);
    }

    /// <summary>
    /// Method toggles activity of effects mixer
    /// </summary>
    /// <param name="enabled"> { true: enable, false: disable } </param>
    public void EffectsToggle(bool enabled)
    {
        float volume = enabled? maxVolume: minVolume;                               // calculate volume
        mixer.audioMixer.SetFloat(effectsVolParamName, volume);                     // change volume in mixer
        PlayerPrefs.SetFloat(effectsVolParamName, volume);                          // store preference
    }

    /// <summary>
    ///  Method toggles activity of music mixer
    /// </summary>
    /// <param name="enabled"> { true: enable, false: disable } </param>
    public void MusicToggle(bool enabled)
    {
        float volume = enabled ? maxVolume : minVolume;
        mixer.audioMixer.SetFloat(musicVolParamName, volume);
        PlayerPrefs.SetFloat(musicVolParamName, volume);
    }

    /// <summary>
    /// Method changes volume of master mixer
    /// </summary>
    /// <param name="volume"> volume value </param>
    public void ChangeVolume(float volume)
    {
        volume = Mathf.Lerp(minVolume, maxVolume, volume);
        mixer.audioMixer.SetFloat(masterVolParamName, volume);
        PlayerPrefs.SetFloat(masterVolParamName, volume);

        UpdateMuteToggle(volume);
    }

    /// <summary>
    ///  Method toggles activity of master mixer
    /// </summary>
    public void MuteToggle()
    {
        float volume = !mute ? minVolume : maxVolume;
        mixer.audioMixer.SetFloat(masterVolParamName, volume);
        PlayerPrefs.SetFloat(masterVolParamName, volume);

        UpdateMuteToggle(volume);                                                       // update view of mute toggler
        UpdateVolumeSlider(volume);                                                     // update view of volume slider                               
    }

    /// <summary>
    /// Method updates view of effects toggler
    /// </summary>
    /// <param name="volume"> value volume </param>
    public void UpdateEffectsToggle(float volume)
    {;
        if (effectsToggle)
            effectsToggle.isOn = volume >= midVolume();
    }

    /// <summary>
    /// Method updates view of music toggler
    /// </summary>
    /// <param name="volume"> value volume </param>
    public void UpdateMusicToggle(float volume)
    {
        if (musicToggle)
            musicToggle.isOn = volume >= midVolume();
    }

    /// <summary>
    /// Method updates view of volume slider
    /// </summary>
    /// <param name="volume"> value volume </param>
    public void UpdateVolumeSlider(float volume)
    {
        if (volumeSlider)
            volumeSlider.value = Mathf.InverseLerp(minVolume, maxVolume, volume);
    }

    /// <summary>
    /// Method updates view of mute toggler
    /// </summary>
    /// <param name="volume"> value volume </param>
    public void UpdateMuteToggle(float volume)
    {
        mute = volume == minVolume;
        if (buttonMute != null && onMuteSprite != null && offMuteSprite != null)
            buttonMute.sprite = mute ? onMuteSprite : offMuteSprite;
    }

    private float midVolume()
    {
        return maxVolume - (maxVolume - minVolume) / 2;
    }
}
