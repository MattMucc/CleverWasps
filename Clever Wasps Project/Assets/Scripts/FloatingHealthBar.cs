using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class floatingHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarBackground;
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    private void Update()
    {
        transform.position = target.position + offset;
    }

    public void UpdateHealth(float currentValue)
    {
        healthBarBackground.fillAmount = currentValue / 1;
    }
}