using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler
{
    [Header("----- Components -----")]
    [SerializeField] Button[] buttons;
    [SerializeField] AudioSource audioSource;

    [Header("----- Audio Clips -----")]
    [SerializeField] AudioClip inspectAudio;
    [SerializeField] AudioClip clickAudio;

    [Header("----- Audio settings -----")]
    [SerializeField] float volume;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(OnClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(inspectAudio, volume);
    }

    void OnClick()
    {
        audioSource.PlayOneShot(clickAudio, volume);
    }
}