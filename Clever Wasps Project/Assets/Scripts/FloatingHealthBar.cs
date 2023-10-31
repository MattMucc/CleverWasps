using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarBackground;
    [SerializeField] Camera cam;
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    public void UpdateSlider(float currentValue)
    {
        healthBarBackground.fillAmount = currentValue / 1;
    }

    private void Update()
    {
        transform.rotation = cam.transform.rotation;
        transform.position = target.position + offset;
    }
}