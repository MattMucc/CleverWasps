using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundController : MonoBehaviour
{
     public AudioSource audioSource;

  
    
     void OnTriggerEnter(Collider other)
     {
        if (other.tag == "Player" && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
     }

    public void PauseAudio()
    {
        if(!audioSource.isPlaying)
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
