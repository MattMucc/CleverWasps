using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] bool invertY;
    Vector3 currentRotation;

    [SerializeField] playerController player;
    float xRot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        if (invertY)
            xRot += mouseY;
        else
            xRot -= mouseY;

        xRot = Mathf.Clamp(xRot, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);

        transform.parent.Rotate(Vector3.up * mouseX);

        currentRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentRotation.x,currentRotation.y, player.tilt);
    }

    public int Sensitivity {get {return sensitivity;} set {sensitivity = value;}}
}
