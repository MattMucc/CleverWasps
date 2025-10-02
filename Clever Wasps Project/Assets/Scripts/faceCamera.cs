using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceCamera : MonoBehaviour
{
    void Update()
    {
        Vector3 targetPos = transform.position + (transform.position - Camera.main.transform.position);
        targetPos.y = 0;
        transform.LookAt(targetPos);

        /*Vector3 lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0; // keep upright
        transform.rotation = Quaternion.LookRotation(lookPos);*/

        //transform.LookAt(Camera.main.transform.transform.position);
    }
}