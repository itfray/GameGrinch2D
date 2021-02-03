using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public virtual void PlaySound(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }
}
