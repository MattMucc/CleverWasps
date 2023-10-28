using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header ("----- Basic Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range (1, 8)][SerializeField] float playerSpeed;
    [Range (8, 30)][SerializeField] float jumpHeight;
    [Range (-10, -40)][SerializeField] float gravityValue;
    [Range (1,4)][SerializeField] int jumpMax;

    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;
    [SerializeField] GameObject cube;

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
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

        if(Input.GetButtonDown("Jump") && jumpedTimes < jumpMax)
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
}
