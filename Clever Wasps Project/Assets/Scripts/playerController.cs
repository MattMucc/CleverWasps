using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
//using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    private const float NORMAL_FOV = 60f;
    private const float GRAPLE_FOV = 110f;

    [Header("----- Basic Components -----")]
    public GameObject Player;
    [SerializeField] CharacterController controller;
    [SerializeField] Animator anim;
    [SerializeField] Swinging swingScript;
    [SerializeField] Camera playerCam;
    [SerializeField] ParticleSystem AnimeLines;
    [SerializeField] GameObject lava;
    [SerializeField] GameObject grappleBars;
    [SerializeField] Collider slideCollider;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject soundFXObjects;
    [SerializeField] GameObject slidingFX;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] float HP;
    [Range(1, 100)][SerializeField] float currentSpeed;
    [Range(1, 15)][SerializeField] float crouchSpeed;
    [Range(8, 30)][SerializeField] float jumpHeight;
    [Range(-10, -40)] public float gravityValue;
    [Range(1, 4)][SerializeField] int jumpMax;
    [SerializeField] bool canCrouch;
    GameObject lowHealthFrame;
    float gravityOrig;

    [Header("----- Gun Stats -----")]
    [SerializeField] ParticleSystem muzzleFlash;
    public List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform gunTip;
    [SerializeField] Image reloadCircle;
    [SerializeField] float shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] int currentAmmo;
    [SerializeField] int maxAmmo;
    [SerializeField] float shootRate;
    [SerializeField] float reloadTime;
    [SerializeField] private float knockBackStrength;
    bool isReloading;
    GameObject bulletType;
    ParticleSystem hitEffect;

    [Header("----- Shield -----")]
    [SerializeField] Image shieldBar;
    [SerializeField] TMP_Text shieldPercentage;
    [SerializeField] float shield;
    [SerializeField] float shieldOriginal;
    [SerializeField] float timeBeforeRegenerate;
    [SerializeField] float howLongToRegenerate;
    Coroutine shieldCoroutine;

    [Header("----- Music -----")]
    [SerializeField] AudioClip[] soundClips;
    public AudioClip bossMusic;
    public AudioSource audioSource;
    public AudioSource audioFX;
    private int currentClipIndex = 0;
    public bool isMusicPlayable;
    private float volumeFx = 1;

    [Header("----- Misc -----")]
    private Vector3 move;
    public Vector3 playerVelocity;
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

    private MovablePlatformScript platform;
    bool isShooting;
    float hpOriginal;
    int ammoOriginal;
    float shootdamageOriginal;
    float playerSpeedOriginal;
    public int gunSelection;

    // Wall Running Variables 
    [Header("----- Wall Running -----")]
    [SerializeField] LayerMask wallMask;
    [SerializeField] float wallRunGravity;

    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private Vector3 wallNormal;
    private Vector3 forwardDirection;
    private bool isWallRunning;
    private bool onLeftWall;
    private bool onRightWall;

    private Quaternion originalRotation;
    public float cameraChangeTime;
    public float wallRunTilt;
    public float tilt;

    [Header("----- Crouch -----")]
    [SerializeField] float weaponSwingSpeed;
    private bool isSwordObtained;
    private bool isSlideAttacking;
    private float weaponSwingRotation = -90;
    private float crouchHeight = 0.5f;
    private float standHeight = 2f;
    private float timeToCrouch = 0f;
    private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    private Vector3 standingCenter = new Vector3(0, 0, 0);
    public bool isCrouching;
    private bool isPlayingSteps;
    private bool duringCrouchAnimation;

    private bool canDynamicHeadbob = true;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0.65F;
    private float timer;

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem jumpEffect;
    [SerializeField] GameObject sparks;



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
        originalRotation = playerCam.transform.rotation;
        lowHealthFrame = gameManager.instance.playerLowHealthFrame;
        shieldBar = GameObject.Find("Shield Bar").GetComponent<Image>();
        shieldPercentage = GameObject.Find("Shield Percentage").GetComponent<TMP_Text>();

        shieldOriginal = shield;
        shieldBar.fillAmount = 1;
        shieldPercentage.text = "100%";

        hpOriginal = HP;
        gravityOrig = gravityValue;
        playerSpeedOriginal = currentSpeed;
        currentFOV = playerCam.fieldOfView;
        shootdamageOriginal = shootDamage;
        isReloading = false;
        currentAmmo = 0;
        maxAmmo = 0;

        UpdatePlayerUI();
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
        checkWallRun();

        // Left Wall Ray Debug
        Debug.DrawRay(transform.position, -transform.right * 1f, Color.red);

        // Right Wall Ray Debug
        Debug.DrawRay(transform.position, transform.right * 1f, Color.blue);

        if (Input.GetKeyDown(crouchKey))
        {
            StartCoroutine(Crouch());
            soundManager.PlayFullSound(soundManager.Sound.slideSound, slidingFX);
            isSlideAttacking = true;
            if (isSwordObtained)
                soundManager.PlaySound(soundManager.Sound.SwordSlash, sword);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            soundManager.LowerSound(slidingFX, volumeFx);
            sparks.SetActive(false);
            StartCoroutine(Crouch());
        }

        //-- SPARKS IF PLAYER IS SLIDING AND IF HE'S GROUNDED --\\
        if (isCrouching && controller.isGrounded)
        {
            if (audioFX.volume <= 3)
            {
                soundManager.RaiseSound(slidingFX, volumeFx);
            }
            sparks.SetActive(true);
        }
        else
        {
            soundManager.LowerSound(slidingFX, volumeFx);
            sparks.SetActive(false);
        }
        //-----------------------------------------------------\\

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

        if (controller.isGrounded && move.normalized.magnitude > 0.4f && !isCrouching && !isPlayingSteps || onRightWall && !isPlayingSteps && !isCrouching && move.normalized.magnitude > 0.4f || onLeftWall && !isPlayingSteps && !isCrouching && move.normalized.magnitude > 0.4f)
            StartCoroutine(steps());

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        if (move != null)
        {
            HandleHeadBob();
        }

        StartCoroutine(movementType());

        if (TestInputJump() && jumpedTimes < jumpMax)
        {
            jumpEffect.Play();
            if (!controller.isGrounded && isWallRunning)
            {
                if (onLeftWall)
                {
                    playerVelocity.z = -jumpHeight;
                    playerVelocity.y = -jumpHeight;
                }
                else if (onRightWall)
                {
                    playerVelocity.z = jumpHeight;
                    playerVelocity.y = jumpHeight;
                }
            }
            else if (!isWallRunning)
            {
                playerVelocity.y = jumpHeight;
            }
            jumpedTimes++;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (!controller.isGrounded)
            cameraEffects();
        else
            tilt = Mathf.Lerp(tilt, 0, cameraChangeTime * Time.deltaTime);


        if (isSwordObtained)
        {
            if (isSlideAttacking)
            {
                sword.SetActive(true);
                slideCollider.enabled = true;
                weaponSwingRotation = Mathf.Lerp(weaponSwingRotation, 90, weaponSwingSpeed * Time.deltaTime);
                sword.transform.localEulerAngles = new Vector3(0, weaponSwingRotation, 0);

                if (weaponSwingRotation >= 65)
                {
                    sword.transform.localEulerAngles = new Vector3(0, -90, 0);
                    weaponSwingRotation = -85;
                    isSlideAttacking = false;
                    slideCollider.enabled = false;
                }
            }
        }


        if (isMusicPlayable)
        {
            if (!audioSource.isPlaying)
            {
                PlayNextSong();
            }
        }

        if (HP <= hpOriginal * .25f)
        {
            lowHealthFrame.SetActive(true);
        }
        else
        {
            lowHealthFrame.SetActive(false);
        }

    }

    private void cameraEffects()
    {
        if (onRightWall)
        {
            tilt = Mathf.Lerp(tilt, wallRunTilt, cameraChangeTime * Time.deltaTime);
        }
        else if (onLeftWall)
        {
            tilt = Mathf.Lerp(tilt, -wallRunTilt, cameraChangeTime * Time.deltaTime);
        }

        else
        {
            tilt = Mathf.Lerp(tilt, 0, cameraChangeTime * Time.deltaTime);
        }
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
            playerBullet bullet = bulletType.GetComponent<playerBullet>();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, 300))
            {
                bullet.damageable = hit.collider.GetComponent<IDamage>();
                Instantiate(bulletType, gunTip.position, gunTip.rotation);

                if (hit.transform != transform && bullet.damageable != null)
                {



                    //Vector3 direction = Player.transform.position - hit.transform.position;
                    //       direction.y = 25f;

                    //controller.Move(direction.normalized * knockBackStrength * Time.deltaTime);
                    //rb.AddForce(direction.normalized * knockBackStrength, ForceMode.Impulse);

                    hitEffect = Instantiate(gunList[gunSelection].hitEffect, hit.point, gunList[gunSelection].hitEffect.transform.rotation);
                    bullet.damageable.takeDamage(shootDamage);

                }
                else
                {
                    hitEffect = Instantiate(gunList[gunSelection].misFire, hit.point, gunList[gunSelection].misFire.transform.rotation);
                }
            }

            UpdateAmmoUI();
            yield return new WaitForSeconds(shootRate);

            Destroy(hitEffect);

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
    //public void knockBack()
    //{

    //    if(Input.GetMouseButtonDown(1))
    //    {
    //        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //        Vector3 forceDirection = transform.position - clickPos;
    //        StartCoroutine(knockBackDelay()); 
    //        GetComponent<Rigidbody>().AddForce(forceDirection);
    //    }

    //}
    //IEnumerator knockBackDelay()
    //{
    //    playerController player  = GetComponent<playerController>();
    //    if(player)
    //    {
    //        player.enabled = false;
    //        yield return new WaitForSeconds(.3f);
    //        player.enabled = true;
    //    }
    //}
    public void takeDamage(float amount)
    {
        DamageShield(amount);

        if (HP <= 0)
        {
            anim.enabled = true;
            anim.SetBool("Dead", true);

        }
    }

    private void whenYouLose()
    {
        gameManager.instance.youLose();
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

        swingScript.grappleGunMesh.enabled = false;
        controller.height = targetHeight;
        controller.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    public void PlayerSpawn()
    {
        anim.SetBool("Dead", false);
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
        transform.rotation = Quaternion.Euler(0, -90, 0);
        // Lava reset 
        lava.transform.position = lavaPosOrigin;
        platform.speed = 0;
    }

    public void UpdatePlayerUI()
    {
        gameManager.instance.HealthBar.fillAmount = HP / hpOriginal;
    }

    public void DamageShield(float amount)
    {
        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        float total = ((shieldBar.fillAmount * 10) - amount);
        shieldBar.fillAmount -= (amount / 10);
        shield = shieldBar.fillAmount * 10;

        if (shieldBar.fillAmount <= 0 && total != 0) //If the subtracted value is less than or equal to 0 which is the min value
        {
            HP += total;
            UpdatePlayerUI();
            StartCoroutine(gameManager.instance.PlayerFlashDamage());
            soundManager.PlaySound(soundManager.Sound.PlayerHit, soundFXObjects);
        }
        else if (shieldBar.fillAmount > 0)
        {
            StartCoroutine(gameManager.instance.PlayerFlashShieldDamage());
        }

        shieldPercentage.SetText(Mathf.Round((shieldBar.fillAmount * 100) / 1).ToString() + "%");
        shieldCoroutine = StartCoroutine(RegenerateShield());
    }

    public void UpdateAmmoUI()
    {
        TMP_Text ammoText = gameManager.instance.ammoText;
        ammoText.text = currentAmmo + " / " + maxAmmo;
    }

    IEnumerator RegenerateShield()
    {
        float initialValue = shieldBar.fillAmount;
        float duration = howLongToRegenerate * initialValue;
        float remainingTime = duration;

        yield return new WaitForSeconds(timeBeforeRegenerate);

        while (remainingTime <= howLongToRegenerate + 1)
        {
            shieldBar.fillAmount = Mathf.Lerp(shieldBar.fillAmount, remainingTime / howLongToRegenerate, .1f);
            shieldPercentage.SetText(Mathf.Round((shieldBar.fillAmount * 100) / 1).ToString() + "%");
            shield = shieldBar.fillAmount * 10;
            yield return new WaitForSeconds(.1f);
            remainingTime += .1f;
        }

        shield = shieldOriginal;
        shieldBar.fillAmount = 1;
        shieldPercentage.SetText(Mathf.Round((shieldBar.fillAmount * 100) / 1).ToString() + "%");
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
            lerpedSlideSpeed = Mathf.Lerp(lerpedSlideSpeed, 0, Time.deltaTime);
            controller.Move((transform.forward * 1.3f) * Time.deltaTime * lerpedSlideSpeed);
            currentSpeed = lerpedSlideSpeed;
        }
        else if (!swingScript.isGrappling && !isWallRunning)
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


    private void HandleHeadBob()
    {
        if (!controller.isGrounded) return;

        if (Mathf.Abs(move.x) > 0.1f || Mathf.Abs(move.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : walkBobSpeed);
            playerCam.transform.localPosition = new Vector3(
                playerCam.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : walkBobAmount),
                playerCam.transform.localPosition.z);
        }
    }

    IEnumerator steps()
    {
        isPlayingSteps = true;
        audioFX.volume = 1;
        soundManager.PlaySound(soundManager.Sound.PlayerMove, soundFXObjects);
        if (onRightWall || onLeftWall)
            yield return new WaitForSeconds(0.25f / gameManager.instance.Multiplier);
        else
            yield return new WaitForSeconds(0.5f / gameManager.instance.Multiplier);

        isPlayingSteps = false;
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

    private void wallRun()
    {
        isWallRunning = true;
        jumpedTimes = 0;
        playerVelocity = new Vector3(0f, -1.5f, 0f);
        currentSpeed = 35;

        wallNormal = onLeftWall ? leftWallHit.normal : rightWallHit.normal;
        forwardDirection = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(forwardDirection, transform.forward) < 0f)
        {
            forwardDirection = -forwardDirection;
        }
    }

    private void exitWallRun()
    {
        isWallRunning = false;
    }

    void checkWallRun()
    {
        onLeftWall = Physics.Raycast(transform.position, -transform.right, out leftWallHit, 1.3f, wallMask);
        onRightWall = Physics.Raycast(transform.position, transform.right, out rightWallHit, 1.3f, wallMask);


        if ((onRightWall || onLeftWall) && !isWallRunning)
        {
            isWallRunning = true;
            wallRun();
        }
        if ((!onRightWall || !onLeftWall) && isWallRunning)
        {
            exitWallRun();
        }
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
        if (other.gameObject.CompareTag("ComboPU"))
        {
            if (gameManager.instance.Multiplier == gameManager.instance.MaxMultiplier && gameManager.instance.MultiplierBar > .5)
                return;
            other.gameObject.SetActive(false);
            gameManager.instance.MultiplierAddValue = 0.5f;
            gameManager.instance.UpdateMultiplier();
            gameManager.instance.MultiplierAddValue = 0.25f;
        }
        if (other.gameObject.CompareTag("AmmoPU"))
        {
            if (currentAmmo == maxAmmo)
                return;
            other.gameObject.SetActive(false);
            currentAmmo += 2;
            UpdateAmmoUI();
            StartCoroutine(gameManager.instance.PlayerFlashAmmo());

        }

        if (other.gameObject.CompareTag("In Game Music"))
        {
            isMusicPlayable = true;
        }

        if (other.gameObject.CompareTag("GrapplePU"))
        {
            other.gameObject.SetActive(false);
            grappleBars.SetActive(true);
            swingScript.GrappleObtained = true;
            swingScript.grappleGun.SetActive(true);
        }

        if (other.gameObject.CompareTag("SwordPU"))
        {
            other.gameObject.SetActive(false);
            sword.SetActive(true);
            soundManager.PlaySound(soundManager.Sound.SwordSlash, sword);
            isSwordObtained = true;
        }

        if (other.gameObject.CompareTag("Lava Trigger"))
        {
            platform.speed = 4;
        }

        //if (other.GameObject.C)
    }

    private void PlayNextSong()
    {
        if (currentClipIndex < soundClips.Length)
        {
            audioSource.clip = soundClips[currentClipIndex];
            audioSource.Play();

            currentClipIndex++;
        }
        else
        {
            currentClipIndex = 0;
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
        bulletType = guns.bulletType;

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
        bulletType = gunList[gunSelection].bulletType;

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
    public float ShootDamage { get { return shootDamage; } set { shootDamage = value; } }
    public float OriginalShootDamage { get { return shootdamageOriginal; } }
    public int CurrentAmmo { get { return currentAmmo; } set { currentAmmo = value; } }
    public int MaxAmmo { get { return maxAmmo; } set { maxAmmo = value; } }
}