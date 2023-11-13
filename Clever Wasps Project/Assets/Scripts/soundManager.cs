using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    [SerializeField] AudioSource audio;

    public enum Sound
    {
        PlayerMove,
        PlayerHit,
        PlayerJump,
        PlayerDeath,
        BulletSound,
        EnemyHit,
        EnemyDeath,
        BossVoice,
        Music
    }
   

    public static void PlaySound(Sound sound, GameObject soundGameObject)
    {
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();

        if(audioSource == null )
        {
            Debug.LogError("Sound Object" + soundGameObject.name + " Was not Found...");
        }

        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (gameManager.SoundAudioClip soundAudioClip in gameManager.instance.soundAudioClipArray)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClips[Random.Range(0, soundAudioClip.audioClips.Length)];
            }
        }
        return null;
    }

}
