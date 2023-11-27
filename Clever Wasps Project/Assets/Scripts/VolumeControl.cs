using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public static VolumeControl instance;

    [SerializeField] AudioMixer audioMixer;

    private void Start()
    {
        instance = this;
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("musicVol", Mathf.Log10(sliderValue) * 20);
    }

    public float GetMusicVolume()
    {
        float value;
        bool result = audioMixer.GetFloat("musicVol", out value);
        if (result)
            return value;
        else
        {
            Debug.Log("Couldn't find");
            return 0f;
        }
    }

    public void SetPlayerVolume(float slidervaliue)
    {

    }
}
