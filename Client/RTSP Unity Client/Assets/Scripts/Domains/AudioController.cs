using Arwel.EventBus;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour//, IEventSubscriber<ToggleSoundEvent>, IEventSubscriber<ToggleMusic>
{
    public AudioMixer musicMixer;
    
    public void SetLevel(float value)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        musicMixer.SetFloat("SoundVolume", Mathf.Log10(value) * 20);
    }
}
