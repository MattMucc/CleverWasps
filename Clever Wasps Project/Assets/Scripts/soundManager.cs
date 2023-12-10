using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class soundManager : MonoBehaviour
{
    [SerializeField] new AudioSource audio;

    public enum Sound
    {
        PlayerMove,
        PlayerHit,
        PlayerJump,
        PlayerDeath,
        EnemyHit,
        Music,
        grappleLaunch,
        grappleSwoosh,
        pistolSound,
        shotgunSound,
        AssaultRifleSound,
        SwordSlash,
        slideSound,
        
           
    }


    public static void PlaySound(Sound sound, GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("Sound Object " + soundGameObject.name + " Was not Found...");
        }

        audioSource.PlayOneShot(GetAudioClip(sound), GetAudioVolume(sound));
    }

    public static void PlayFullSound(Sound sound, GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }

        audioSource.clip = GetAudioClip(sound);
        audioSource.Play();
    }

    public static void PauseSound(GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }

        audioSource.Pause();
    }

    public static void UnPauseSound(GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }

        audioSource.UnPause();
    }

    public static void LowerSound(GameObject soundGameObject, float volumeLerp)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }
        volumeLerp = Mathf.Lerp(audioSource.volume, 0f, 5.5f * Time.deltaTime);
        audioSource.volume = volumeLerp;
    }
    public static void RaiseSound(GameObject soundGameObject, float volumeLerp)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }
        volumeLerp = Mathf.Lerp(audioSource.volume, 1f, 7f * Time.deltaTime);
        audioSource.volume = volumeLerp;
    }

    public static void StopSound(GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }

        audioSource.Stop();
    }

    public static bool IsPlaying (GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.Log("Sound Object " + soundGameObject.name + " Was not Found...");
        }

        return audioSource.isPlaying;
    }


    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (gameManager.SoundAudioClip soundAudioClip in gameManager.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClips[Random.Range(0, soundAudioClip.audioClips.Length)];
            }
        }
        return null;
    }

    private static float GetAudioVolume(Sound sound)
    {
        foreach (gameManager.SoundAudioClip soundAudioClip in gameManager.instance.soundAudioClipArray)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.audVolume;
            }
        }
        return 1;
    }
}
