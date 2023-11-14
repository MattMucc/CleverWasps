using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class soundManager : MonoBehaviour
{
    [SerializeField] AudioSource audio;

    public enum Sound
    {
        PlayerMove,
        PlayerHit,
        PlayerJump,
        PlayerDeath,
        EnemyHit,
        BossVoice,
        Music,
        grappleLaunch,
        grappleSwoosh,
        pistolSound,
    }


    public static void PlaySound(Sound sound, GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("Sound Object" + soundGameObject.name + " Was not Found...");
        }

        audioSource.PlayOneShot(GetAudioClip(sound), GetAudioVolume(sound));
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
