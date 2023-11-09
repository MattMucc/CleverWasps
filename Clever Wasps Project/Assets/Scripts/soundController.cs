using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundController : MonoBehaviour
{
     public AudioSource audioSource;
    public AudioClip clip;
    bool isPlaying;
  
    
     void OnTriggerEnter(Collider other)
     {
        if (other.tag == "Player" && !isPlaying)
        {
            audioSource.PlayOneShot(clip);
            isPlaying = true;
        }
     }

    public void PauseAudio()
    {
        if(audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void resumeAudio()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
   
}
