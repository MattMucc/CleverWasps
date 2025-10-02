using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class foundAudio : MonoBehaviour
{
    [SerializeField] GameObject button;
    [SerializeField] GameObject emitter;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClip;
    [SerializeField] float audioVolume;
    bool playerInTrigger;
    bool playerWasOriginallyInTrigger;
    bool audioIsPlaying;

    private void Update()
    {
        OnPause();
        PlayAudio();
    }

    private void PlayAudio()
    {
        if (playerInTrigger && Input.GetButtonDown("Interact"))
        {
            if (!audioSource)
            {
                Debug.LogError("Audio Source is not set!");
                return;
            }

            AudioClip clip = audioClip[Random.Range(0, audioClip.Length)];
            if (!clip)
            {
                Debug.LogError("Audio clip could not be set!");
                return;
            }

            if (audioIsPlaying)
                return;

            audioIsPlaying = true;
            audioVolume = gameManager.instance.sfxVol.value;
            audioSource.volume = audioVolume;

            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(ResetAudioFlag(clip.length));
        }
    }

    private void OnPause()
    {
        if (gameManager.instance.isPaused)
        {
            if (playerInTrigger)
            {
                playerInTrigger = false;
                playerWasOriginallyInTrigger = true;
                button.SetActive(false);
            }

            if (audioSource.isPlaying)
                audioSource.Pause();
        }
        else
        {
            if (playerWasOriginallyInTrigger)
            {
                playerInTrigger = true;
                playerWasOriginallyInTrigger = false;
                button.SetActive(true);
            }

            if (audioIsPlaying)
            {
                audioVolume = gameManager.instance.sfxVol.value;
                audioSource.volume = audioVolume;
                audioSource.UnPause();
            }
        }
    }

    IEnumerator ResetAudioFlag(float duration)
    {
        yield return new WaitForSeconds(duration); // Stops when paused which works in our case
        audioIsPlaying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = true;
            button.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            button.SetActive(false);
        }
    }
}
