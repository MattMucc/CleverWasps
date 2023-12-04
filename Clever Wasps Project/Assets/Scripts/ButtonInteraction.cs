using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler
{
    [Header("----- Audio Clips -----")]
    [SerializeField] AudioClip inspectAudio;
    [SerializeField] AudioClip clickAudio;

    Button button;
    AudioSource audioSource;

    void Start()
    {
        button = GetComponent<Button>();
        audioSource = GameObject.Find("UI").gameObject.GetComponent<AudioSource>();
        button.onClick.AddListener(OnClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(inspectAudio);

        if (!EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            if (!EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);
    }

    void OnClick()
    {
        audioSource.PlayOneShot(clickAudio);
    }
}