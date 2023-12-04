using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    /*public float GetVolumeValue(string volName)
    {
        float value;
        bool result = audioMixer.GetFloat(volName, out value);
        if (result)
        {
            Debug.Log($"Value Returned: {value}");
            return value;
        }
        else
        {
            Debug.Log($"Couldn't find {volName} in Mixer");
            return 0f;
        }
    }*/

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("musicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSFXVolume(float slidervaliue)
    {
        audioMixer.SetFloat("sfxVol", Mathf.Log10(slidervaliue) * 20);
    }

    public void SetUIVolume(float sliderValue)
    {
        audioMixer.SetFloat("uiVol", Mathf.Log10(sliderValue) * 20);
    }
}
