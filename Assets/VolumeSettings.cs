using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;   
   

    public void SetMusicVolume()
    {
        float volume = _musicSlider.value;
        _mixer.SetFloat("music", volume);
    }
    public void SetMasterVolume()
    {
        float volume = _masterSlider.value;
        _mixer.SetFloat("master", volume);
    }
    public void SetSFXVolume()
    {
        float volume = _sfxSlider.value;
        _mixer.SetFloat("sfx", volume);
    }

    private void LoadVolume()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("music");
    }
}
