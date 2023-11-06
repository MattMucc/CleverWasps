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
    public Transform grappleGun;

    [Header("Grapple")]
    public float maxGrappleDistance;
    private Vector3 grappleGunOrigin;
    private Quaternion grappleGunShootPos;
    public Vector3 grapplePoint;
    private SpringJoint joint;


    [Header("Cooldown")]
    [SerializeField] float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    public bool isGrappling;

    public void Start()
    {
        playerScript = GetComponent<playerController>();
        grappleGunOrigin = grappleGun.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            //grappleGun.SetLocalPositionAndRotation(new Vector3(-0.476f, 0.131f, 0.244f), new Quaternion(0.95f, 0.69f, 0.005f, 1)); 
            StartSwing();
        }

        if (Input.GetKeyUp(grappleKey))
        {
            StopSwing();
        }

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            gameManager.instance.grappleBar1.color = gameManager.instance.GrappleYes;
            gameManager.instance.grappleBar2.color = gameManager.instance.GrappleYes;
            gameManager.instance.grappleBar3.color = gameManager.instance.GrappleYes;
        }
        else
        {
            gameManager.instance.grappleBar1.color = gameManager.instance.GrappleNo;
            gameManager.instance.grappleBar2.color = gameManager.instance.GrappleNo;
            gameManager.instance.grappleBar3.color = gameManager.instance.GrappleNo;
        }
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
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            isGrappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }

    }
    IEnumerator grappleTimer()
    {
        isGrappling = true;


        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            isGrappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }

        yield return new WaitForSeconds(grapplingCd);
        isGrappling = false;
    }


    public void StopSwing()
    {
        isGrappling = false;
        lr.enabled = false;
        lr.positionCount = 2;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    public void DrawRope()
    {
        if (!joint)
            return;

        lr.enabled = true;
        currentGrapplePosition =
            Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);

    }
}
