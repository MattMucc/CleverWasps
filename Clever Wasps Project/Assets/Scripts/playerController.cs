using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header ("----- Basic Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Rigidbody rb;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 100)][SerializeField] float playerSpeed;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-10, -40)][SerializeField] float gravityValue;
    [Range (1,4)][SerializeField] int jumpMax;
    [SerializeField] bool canCrouch;
    
    
    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] float grappleForce = 50f;

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;

    bool isShooting;

    //Crouch
    private float crouchHeight = 0.5f;
    private float standHeight = 2f;
    private float timeToCrouch = 0.25f;
    private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;
    private CharacterController characterContr;


    // Start is called before the first frame update
    void Start()
    {
            characterContr = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
       

        if(Input.GetButtonDown("crouch"))
        {
            StartCoroutine(Crouch());
        }
        if(Camera.main != null) 
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
            
        }
        if(Input.GetButton("Shoot") && !isShooting) 
        {
            StartCoroutine(shoot());
        }

        groundedPlayer = controller.isGrounded;
        if(groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>(); 

            if(hit.transform != transform && damageable != null)
            {
                damageable.takeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;  
    }

    public void ApplyGrappleForce(Vector3 direction)
    {
         if (rb != null)
        {
            rb.AddForce(direction * grappleForce, ForceMode.Force);
        }
    }

    private IEnumerator Crouch()
    {
        if (isCrouching && Physics.Raycast(Camera.main.transform.position, Vector3.up, 1f)) 
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeight =  characterContr.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterContr.center;

        while (timeElapsed < timeToCrouch) 
        {
            characterContr.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterContr.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterContr.height = targetHeight;
        characterContr.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

}
