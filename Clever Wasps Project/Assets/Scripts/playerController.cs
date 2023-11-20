using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    private const float NORMAL_FOV = 60f;
    private const float GRAPLE_FOV = 110f;

    [Header("----- Basic Components -----")]
    [SerializeField] GameObject Player;
    [SerializeField] CharacterController controller;
    [SerializeField] Swinging swingScript;
    [SerializeField] Camera playerCam;
    [SerializeField] ParticleSystem AnimeLines;
    [SerializeField] GameObject lava;
    [SerializeField] GameObject grappleBars;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 100)][SerializeField] float currentSpeed;
    [Range(1, 15)][SerializeField] float crouchSpeed;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-10, -40)][SerializeField] float gravityValue;
    [Range(1, 4)][SerializeField] int jumpMax;
    [SerializeField] bool canCrouch;
    private float gravityOrig;

    [Header("----- Gun Stats -----")]
    [SerializeField] ParticleSystem muzzleFlash;

    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] Image reloadCircle;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] int currentAmmo;
    [SerializeField] int maxAmmo;
    [SerializeField] float shootRate;
    [SerializeField] float reloadTime;
    bool isReloading;

    private Vector3 move;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpedTimes;
    public Vector3 lavaPosOrigin;

    // Grapple Variables
    private float grappleSpeed;
    private float grappleTime;
    private float grappleSpeedMin;
    private float grappleSpeedMax;
    private float currentFOV;
    private float lerpedSlideSpeed;

    // Zoom variables


    private MovablePlatformScript platform;
    bool isShooting;
    int hpOriginal;
    int shootdamageOriginal;
    float playerSpeedOriginal;
    int gunSelection;

    bool isPlayingSteps;

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
        grappleSpeedMax = 120f;
        grappleSpeedMin = 55f;
        grappleSpeed = 40f;
        lerpedSlideSpeed = 50f;
        reloadCircle = gameManager.instance.reloadCircle;
        reloadCircle.fillAmount = 0;

        hpOriginal = HP;
        gravityOrig = gravityValue;
        playerSpeedOriginal = currentSpeed;
        currentFOV = playerCam.fieldOfView;
        shootdamageOriginal = shootDamage;
        isReloading = false;
        currentAmmo = 0;
        maxAmmo = 0;
        UpdateAmmoUI();

        platform = lava.GetComponent<MovablePlatformScript>();
        lavaPosOrigin = lava.transform.position;
        if (platform == null)
        {
            Debug.Log("Nope");
        }

        AnimeLines.Stop();
        swingScript = GetComponent<Swinging>();
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

        if (Input.GetKeyUp(crouchKey))
        {
            StartCoroutine(Crouch());
        }


        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        if (gunList.Count > 0)
        {
            selectedGun();
            if (Input.GetButton("Shoot") && !isShooting && !isReloading)
            {
                StartCoroutine(shoot());
            }
        }

        if (Input.GetButtonDown("Reload") && !isReloading)
        {
            StartCoroutine(Reload());
            Debug.Log("Reloading");
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            //soundManager.PlaySound(soundManager.Sound.PlayerMove, Player);
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        StartCoroutine(movementType());

        if (TestInputJump() && jumpedTimes < jumpMax)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator shoot()
    {
        if (gunList[gunSelection].ammoCurr > 0)
        {
            isShooting = true;
            muzzleFlash.Play();
            soundManager.PlaySound(gunList[gunSelection].sound, gunModel);
            gunList[gunSelection].ammoCurr--;
            currentAmmo--;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDistance))
            {
                IDamage damageable = hit.collider.GetComponent<IDamage>();

                if (hit.transform != transform && damageable != null)
                {
                    Instantiate(gunList[gunSelection].hitEffect, hit.point, gunList[gunSelection].hitEffect.transform.rotation);
                    damageable.takeDamage(shootDamage);
                }
                else
                {
                    Instantiate(gunList[gunSelection].misFire, hit.point, gunList[gunSelection].misFire.transform.rotation);
                }
            }

            UpdateAmmoUI();
            yield return new WaitForSeconds(shootRate);

            isShooting = false;
        }
    }

    IEnumerator Reload()
    {
        Debug.Log("Entered Coroutine");
        isReloading = true;
        reloadCircle.fillAmount = 0;

        float remainingTime = 0;
        while (remainingTime <= reloadTime)
        {
            reloadCircle.fillAmount = Mathf.Lerp(remainingTime / reloadTime, reloadCircle.fillAmount, .1f);
            yield return new WaitForSeconds(.1f);
            remainingTime += .1f;
        }

        reloadCircle.fillAmount = 0;
        gunList[gunSelection].ammoCurr = gunList[gunSelection].ammoMax;
        currentAmmo = gunList[gunSelection].ammoCurr;
        UpdateAmmoUI();
        isReloading = false;
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
        soundManager.PlaySound(soundManager.Sound.PlayerHit, Player);
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
        if (gameManager.instance.menuActive == gameManager.instance.menuLose)
        {
            getPlayerPos();
        }
        else
        {
            transform.position = gameManager.instance.PlayerSpawnPos.transform.position;
        }
        controller.enabled = true;

        // Lava reset 
        lava.transform.position = lavaPosOrigin;
        platform.speed = 0;
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.HealthBar.fillAmount = (float)HP / hpOriginal;
    }

    public void UpdateAmmoUI()
    {
        TMP_Text ammoText = gameManager.instance.ammoText;
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }

    IEnumerator movementType()
    {
        if (swingScript.isGrappling)
        {
            yield return new WaitForSeconds(0.3f);
            grappleMovement();
        }
        else if (isCrouching)
        {
            changeFOV();
            AnimeLines.Play();
            lerpedSlideSpeed = Mathf.Lerp(lerpedSlideSpeed, 5, Time.deltaTime);
            controller.Move((transform.forward * 1.3f) * Time.deltaTime * lerpedSlideSpeed);
            currentSpeed = lerpedSlideSpeed;
        }
        else if (!swingScript.isGrappling)
        {
            walkingMovement();
        }
    }

    private void grappleMovement()
    {
        currentSpeed = 50 * gameManager.instance.Multiplier;
        gravityValue = 0;

        grappleTime += Time.deltaTime;
        AnimeLines.Play();
        changeFOV();
        grappleSpeed = Mathf.Clamp(Vector3.Distance(transform.position, swingScript.grapplePoint), grappleSpeedMin, grappleSpeedMax);

        // Grapple Movement
        controller.Move((swingScript.grapplePoint - new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z)).normalized * Time.deltaTime * grappleSpeed);
        controller.Move(move * Time.deltaTime * playerSpeedOriginal);

        if (Vector3.Distance(transform.position, swingScript.grapplePoint) < 3f)
        {
            swingScript.StopSwing();
            StartCoroutine(swingScript.Cooldown());
            swingScript.toggleGraple = false;
        }
    }

    private void walkingMovement()
    {
        AnimeLines.Stop();
        lerpedSlideSpeed = 50;

        //gravity lerped to origin
        gravityValue = Mathf.Lerp(gravityValue, gravityOrig, Time.deltaTime * 8f);

        //FOV reverting back to normal
        currentFOV = Mathf.Lerp(currentFOV, NORMAL_FOV, Time.deltaTime * 1.5f);
        playerCam.fieldOfView = currentFOV;

        //Player Speed being lerped to origin
        currentSpeed = Mathf.Lerp(currentSpeed, playerSpeedOriginal, Time.deltaTime * 0.5f);

        controller.Move(move * Time.deltaTime * currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HealthPU"))
        {
            if (HP == hpOriginal)
                return;
            other.gameObject.SetActive(false);
            HP += 3;
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

        if (other.gameObject.CompareTag("GrapplePU"))
        {
            other.gameObject.SetActive(false);
            grappleBars.SetActive(true);
            swingScript.GrappleObtained = true;
            swingScript.grappleGun.SetActive(true);
        }

        if (other.gameObject.CompareTag("Lava Trigger"))
        {
            platform.speed = 4;
        }
    }

    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public void getGunStats(GunStats guns)
    {
        gunList.Add(guns);
        shootDamage = guns.shootDamage;
        shootdamageOriginal = shootDamage;
        shootDistance = guns.shootDistance;
        currentAmmo = guns.ammoCurr;
        maxAmmo = guns.ammoMax;
        shootRate = guns.shootRate;

        UpdateAmmoUI();

        gunModel.GetComponent<MeshFilter>().sharedMesh = guns.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = guns.model.GetComponent<MeshRenderer>().sharedMaterial;

        gunSelection = gunList.Count - 1;
    }
    void changeGun()
    {
        shootDamage = gunList[gunSelection].shootDamage;
        shootdamageOriginal = shootDamage;
        shootDistance = gunList[gunSelection].shootDistance;
        currentAmmo = gunList[gunSelection].ammoCurr;
        maxAmmo = gunList[gunSelection].ammoMax;
        shootRate = gunList[gunSelection].shootRate;

        UpdateAmmoUI();

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunSelection].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunSelection].model.GetComponent<MeshRenderer>().sharedMaterial;
        isShooting = false;
    }
    void selectedGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunSelection < gunList.Count - 1)
        {
            gunSelection++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunSelection > 0)
        {
            gunSelection--;
            changeGun();
        }
    }

    public void setPlayerPos()
    {
        PlayerPrefs.SetFloat("PlayerPosX", transform.position.x);
        PlayerPrefs.SetFloat("PlayerPosY", transform.position.y);
        PlayerPrefs.SetFloat("PlayerPosZ", transform.position.z);
    }

    public void getPlayerPos()
    {
        controller.enabled = false;

        transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerPosX"), PlayerPrefs.GetFloat("PlayerPosY"), PlayerPrefs.GetFloat("PlayerPosZ"));

        controller.enabled = true;
    }

    private void changeFOV()
    {
        currentFOV = Mathf.Lerp(currentFOV, GRAPLE_FOV, Time.deltaTime * 2.5f);
        playerCam.fieldOfView = currentFOV;
    }

    public float PlayerSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float OriginalPlayerSpeed { get { return playerSpeedOriginal; } }
    public int ShootDamage { get { return shootDamage; } set { shootDamage = value; } }
    public int OriginalShootDamage { get { return shootdamageOriginal; } }
    public int CurrentAmmo { get { return currentAmmo; } set { currentAmmo = value; } }
    public int MaxAmmo { get { return maxAmmo; } set { maxAmmo = value; } }
}