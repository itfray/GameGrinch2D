using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SettingsMenuController : MonoBehaviour
{
    public AudioMixerGroup mixer;

    public const string musicVolParamName = "MusicVolume";
    public const string soundVolParamName = "SoundVolume";
    public const string masterVolParamName = "MasterVolume";

    public Image btMute;
    public Sprite onMute;
    public Sprite offMute;
    private bool muted = false;

    public void SoundToggle(bool enabled)
    {
        if (enabled)
            mixer.audioMixer.SetFloat(soundVolParamName, 0);
        else
            mixer.audioMixer.SetFloat(soundVolParamName, -80);
    }

    public void MusicToggle(bool enabled)
    {
        if (enabled)
            mixer.audioMixer.SetFloat(musicVolParamName, 0);
        else
            mixer.audioMixer.SetFloat(musicVolParamName, -80);
    }

    public void ChangeVolume(float volume)
    {
        mixer.audioMixer.SetFloat(masterVolParamName, Mathf.Lerp(-80, 0, volume));
    }

    public void MuteButtonClick()
    {
        muted = !muted;

        if (muted)
        {
            btMute.sprite = onMute;
            mixer.audioMixer.SetFloat(masterVolParamName, -80);
        }
        else
        {
            btMute.sprite = offMute;
            mixer.audioMixer.SetFloat(masterVolParamName, 0);
        }
    }
}
