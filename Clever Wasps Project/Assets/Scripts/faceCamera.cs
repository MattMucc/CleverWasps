using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.transform.position);
    }
}