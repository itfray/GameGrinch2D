using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsMenuController : MonoBehaviour
{
    public AudioMixerGroup mixer;

    public float maxVolume = 0;
    public float minVolume = -80;

    public const string musicVolParamName = "MusicVolume";
    public const string soundVolParamName = "EffectsVolume";
    public const string masterVolParamName = "MasterVolume";

    public Image buttonMute;
    public Sprite onMuteSprite;
    public Sprite offMuteSprite;
    private bool mute = false;

    void Start()
    {
        mixer.audioMixer.SetFloat(soundVolParamName, PlayerPrefs.GetFloat(soundVolParamName, maxVolume));
        mixer.audioMixer.SetFloat(musicVolParamName, PlayerPrefs.GetFloat(musicVolParamName, maxVolume));
        mixer.audioMixer.SetFloat(masterVolParamName, PlayerPrefs.GetFloat(masterVolParamName, maxVolume));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enabled"></param>
    public void SoundToggle(bool enabled)
    {
        float volume = enabled? maxVolume: minVolume;                               // calculate volume
        mixer.audioMixer.SetFloat(soundVolParamName, volume);                       // change volume in mixer
        PlayerPrefs.SetFloat(soundVolParamName, volume);                            // store preference
    }

    public void MusicToggle(bool enabled)
    {
        float volume = enabled ? maxVolume : minVolume;
        mixer.audioMixer.SetFloat(musicVolParamName, volume);
        PlayerPrefs.SetFloat(musicVolParamName, volume);
    }

    public void ChangeVolume(float volume)
    {
        volume = Mathf.Lerp(minVolume, maxVolume, volume);
        mixer.audioMixer.SetFloat(masterVolParamName, volume);
        PlayerPrefs.SetFloat(masterVolParamName, volume);
    }

    public void MuteButtonClick()
    {
        mute = !mute;

        float volume = mute ? minVolume : maxVolume;
        mixer.audioMixer.SetFloat(masterVolParamName, volume);
        PlayerPrefs.SetFloat(masterVolParamName, volume);

        if (buttonMute != null && onMuteSprite != null && offMuteSprite != null)
            buttonMute.sprite = mute ? onMuteSprite : offMuteSprite;
    }
}
