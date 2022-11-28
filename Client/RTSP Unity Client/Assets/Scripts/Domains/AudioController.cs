using UnityEngine;
using UnityEngine.Audio;

namespace Arwel.Scripts.Domains
{
    public class AudioController : MonoBehaviour
    {
        public AudioMixer musicMixer;
        private float _currentValue;

        public void SetLevel(float value)
        {
            musicMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
            musicMixer.SetFloat("SoundVolume", Mathf.Log10(value) * 20);
            _currentValue = value;
        }

        public void SetSound(bool isOn)
        {
            Debug.Log("sound to");
            Debug.Log(isOn);
            Debug.Log(musicMixer);

            if (!isOn)
            {
                musicMixer.SetFloat("SoundVolume", Mathf.Log10(0.0001f) * 20);
            }
            else
            {
                musicMixer.SetFloat("SoundVolume", Mathf.Log10(_currentValue) * 20);
            }
        }

        public void SetMusic(bool isOn)
        {
            Debug.Log("Music to");
            Debug.Log(isOn);

            if (!isOn)
            {
                musicMixer.SetFloat("MusicVolume", Mathf.Log10(0.0001f) * 20);
            }
            else
            {
                musicMixer.SetFloat("MusicVolume", Mathf.Log10(_currentValue) * 20);
            }
        }
    }
}