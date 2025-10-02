using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerText : MonoBehaviour
{
    [SerializeField] Collider textCollider;
    [SerializeField] GameObject text;
    bool playerIsInTrigger;
    bool textwasOriginallyactive;

    private void Start()
    {
        if (text)
            text.SetActive(false);
        else
            Debug.LogWarning("Text GameObject is not set!");
    }

    private void Update()
    {
        if (!text)
            return;

        OnPause();
    }

    private void OnPause()
    {
        if (gameManager.instance.isPaused)
        {
            if (text.activeSelf)
            {
                textwasOriginallyactive = true;
                text.SetActive(false);
            }
        }
        else
        {
            if (textwasOriginallyactive && playerIsInTrigger)
                text.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            text.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            text.SetActive(false);
        }
    }
}
