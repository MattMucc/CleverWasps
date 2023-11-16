using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    [SerializeField] GameObject Emitter;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClip;
    [Range(0, 1)][SerializeField] float audioVolume;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)], audioVolume);
        }
    }
}
