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
    [Range(0,1)][SerializeField] float audioVolume;
    bool playerInTrigger;


    private void Update()
    {
        if(playerInTrigger && Input.GetButtonDown("Interact"))
        {
            audioSource.PlayOneShot(audioClip[Random.Range(0, audioClip.Length)], audioVolume);
        }
    }

    // Start is called before the first frame update
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
