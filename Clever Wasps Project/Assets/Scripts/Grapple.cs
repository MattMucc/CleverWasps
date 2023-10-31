using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Swinging : MonoBehaviour
{
    [Header("References")]
    [SerializeField] playerController playerScript;
    [SerializeField] Transform player;
    [SerializeField] Transform cam;
    [SerializeField] Transform gunTip;
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] LineRenderer lr;

    [Header("Swinging")]
    public float maxSwingDistance;

    public Vector3 swingPoint;
    private SpringJoint joint;


    [Header("Cooldown")]
    [SerializeField] float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode swingKey = KeyCode.Mouse1;

    public bool isSwinging;

    public void Start()
    {
        playerScript = GetComponent<playerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(swingKey))
            StartSwing();

        if (Input.GetKeyUp(swingKey))
            StopSwing();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartSwing()
    {
        if (grapplingCdTimer > 0)
            return;
        

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable))
        {
            isSwinging = true;
            swingPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = swingPoint;

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }

    }


    private void StopSwing()
    {
        isSwinging = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    private void DrawRope()
    {
        if (!joint)
            return;

        currentGrapplePosition =
            Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);

    }
}
