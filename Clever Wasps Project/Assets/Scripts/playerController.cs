using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Basic Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Rigidbody rb;
    [SerializeField] Swinging swingScript;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 100)][SerializeField] float playerSpeed;
    [Range(1, 15)][SerializeField] float swingSpeed;
    [Range(1, 15)][SerializeField] float crouchSpeed;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-10, -40)][SerializeField] float gravityValue;
    [Range(1, 4)][SerializeField] int jumpMax;
    [SerializeField] bool canCrouch;


    [Header("----- Gun Stats -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;

    bool isShooting;

    int hpOriginal;
    int shootdamageOriginal;
    float playerSpeedOrginal;

    [Header("----- Crouch -----")]
    private float crouchHeight = 0.5f;
    private float standHeight = 2f;
    private float timeToCrouch = 0.25f;
    private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    private KeyCode crouchKey = KeyCode.LeftShift;


    // Start is called before the first frame update
    void Start()
    {
        hpOriginal = HP;
        playerSpeedOrginal = playerSpeed;
        shootdamageOriginal = shootDamage;
        swingScript = GetComponent<Swinging>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        controller = GetComponent<CharacterController>();
        PlayerSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            StartCoroutine(Crouch());
        }


        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        if (Input.GetButton("Shoot") && !isShooting)
        {
            StartCoroutine(shoot());
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        if (swingScript.isSwinging)
            controller.Move((swingScript.swingPoint - transform.position) * Time.deltaTime * swingSpeed);

        else if (isCrouching)
            controller.Move(move * Time.deltaTime * crouchSpeed);
        else
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
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if (hit.transform != transform && damageable != null)
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
        UpdatePlayerUI();
        StartCoroutine(gameManager.instance.PlayerFlashDamage());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    private IEnumerator Crouch()
    {
        if (isCrouching && Physics.Raycast(Camera.main.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeight = controller.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    public void PlayerSpawn()
    {
        controller.enabled = false;
        HP = hpOriginal;
        UpdatePlayerUI();
        transform.position = gameManager.instance.PlayerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.HealthBar.fillAmount = (float)HP / hpOriginal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HealthPU"))
        {
            if (HP == hpOriginal)
                return;
            other.gameObject.SetActive(false);
            HP++;
            UpdatePlayerUI();
            StartCoroutine(gameManager.instance.PlayerFlashHealth());
        }
        else if (other.gameObject.CompareTag("ComboPU"))
        {
            if (gameManager.instance.Multiplier == gameManager.instance.MaxMultiplier && gameManager.instance.MultiplierBar > .5)
                return;
            other.gameObject.SetActive(false);
            gameManager.instance.MultiplierAddValue = 0.5f;
            gameManager.instance.UpdateMultiplier();
            gameManager.instance.MultiplierAddValue = 0.25f;
        }
    }

    public int ShootDamage { get { return shootDamage; } set { shootDamage = value; } }
    public int OriginalShootDamage { get { return shootdamageOriginal; } }
    public float PlayerSpeed { get { return playerSpeed; } set { playerSpeed = value; } }
    public float OriginalPlayerSpeed { get { return playerSpeedOrginal; } }
}