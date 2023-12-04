
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Menu -----")]
    public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    public GameObject menuLose;
    [SerializeField] GameObject menuSettings;
    [SerializeField] GameObject gameControls;

    [Header("----- Settings Menu -----")]
    public VolumeControl volumeControl;
    [SerializeField] Slider sensitivity;
    [SerializeField] TMP_Text sensitivityTextValue;

    [Header("----- Gun UI -----")]
    [SerializeField] GameObject crossHair;
    public Image reloadCircle;
    public TMP_Text ammoText;

    [Header("----- Boss Settings")]
    [SerializeField] Rikayon boss;
    public Image bossHealthBar;

    [Header("----- Audio -----")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] soundController music;

    [Header("----- Multiplier -----")]
    [SerializeField] Image multiplierBar;
    [SerializeField] TMP_Text multiplierNumber;
    [SerializeField] int multiplier;
    [SerializeField] int maxMultiplier;
    [SerializeField] float timeBeforeDecrease;
    [SerializeField] float multiplierResetTime;
    [SerializeField] float multiplierAddedValue;
    Coroutine multiplierCoroutine;

    [Header("----- Player -----")]
    public GameObject player;
    [SerializeField] playerController playerScript;
    [SerializeField] GameObject playerSpawnPos;
    [SerializeField] GameObject playerFlashDamage;
    [SerializeField] GameObject playerFlashHealth;
    [SerializeField] GameObject playerFlashAmmo;
    [SerializeField] GameObject playerFlashShield;
    [SerializeField] GameObject grappleBarContainer;
    [SerializeField] CameraControls cameraScript;
    public GameObject playerLowHealthFrame;
    public Image grappleBar1;
    public Image grappleBar2;
    public Image grappleBar3;

    public Color GrappleYes;
    public Color GrappleNo;
    [SerializeField] Image healthBar;
    [SerializeField] TMP_Text enemyCount;

    public bool isBossSpawned;
    public bool isPaused;
    float timescaleOrig;
    public int enemiesRemaining;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timescaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        cameraScript = Camera.main.GetComponent<CameraControls>();
        volumeControl = gameObject.GetComponent<VolumeControl>();

        multiplier = 1;
        multiplierBar.fillAmount = 0;
        multiplierNumber.SetText("x" + multiplier.ToString());
        multiplierCoroutine = StartCoroutine(DecreaseMultiplier(multiplierResetTime));

        enemiesRemaining = 0;
        enemyCount.text = enemiesRemaining.ToString("0");

        sensitivity.value = cameraScript.Sensitivity;
        sensitivityTextValue.text = sensitivity.value.ToString();

        if (GameObject.FindWithTag("Boss"))
        {
            isBossSpawned = true;
            boss = GameObject.FindWithTag("Boss").GetComponent<Rikayon>();
        }
        else
            isBossSpawned = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSettings();
        if(Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
            
        }

        if (GameObject.FindWithTag("Boss") && !isBossSpawned)
        {
            isBossSpawned = true;
            boss = GameObject.FindWithTag("Boss").GetComponent<Rikayon>();
        }
    }

    public void statePause()
    {
        playerScript.audioSource.Pause();
        playerScript.isMusicPlayable = false;
        crossHair.SetActive(false);
        reloadCircle.enabled = false;
        isPaused = !isPaused;
        Time.timeScale = 0;
        //grappleBarContainer.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        playerScript.audioSource.UnPause();
        playerScript.isMusicPlayable = true;
        crossHair.SetActive(true);
        reloadCircle.enabled = true;
        isPaused = !isPaused;
        Time.timeScale = timescaleOrig;
        //grappleBarContainer.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void OpenSettings()
    {
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void UpdateSettings()
    {
        cameraScript.Sensitivity = (int)sensitivity.value;
        sensitivityTextValue.text = sensitivity.value.ToString();
    }

    public void OpenControls()
    {
        menuActive.SetActive(false);
        menuActive = gameControls;
        menuActive.SetActive(true);
    }

    public void Back()
    {
        menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
    {
        int previousEnemiesRemaining = enemiesRemaining;
        enemiesRemaining += amount;
        enemyCount.text = enemiesRemaining.ToString("0");

        if (previousEnemiesRemaining > enemiesRemaining) //Player killed an enemy
        {
            UpdateMultiplier();
        }

        if (boss != null)
        {
            if (boss.HP <= 0)
            {
                StartCoroutine(youWin());
            }
        }
    }

    IEnumerator youWin()
    {
        yield return new WaitForSeconds(5);
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        //yield return new WaitForSeconds(2.8f);
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public IEnumerator PlayerFlashDamage()
    {
        playerFlashDamage.SetActive(true);
        yield return new WaitForSeconds(.1f);
        playerFlashDamage.SetActive(false);
    }

    public IEnumerator PlayerFlashHealth()
    {
        playerFlashHealth.SetActive(true);
        yield return new WaitForSeconds(.1f);
        playerFlashHealth.SetActive(false);
    }
     public IEnumerator PlayerFlashAmmo()
    {
        playerFlashAmmo.SetActive(true);
        yield return new WaitForSeconds(.1f);
        playerFlashAmmo.SetActive(false);
    }

    public IEnumerator PlayerFlashShieldDamage()
    {
        playerFlashShield.SetActive(true);
        yield return new WaitForSeconds(.1f);
        playerFlashShield.SetActive(false);
    }

    public void UpdateMultiplier()
    {
        StopCoroutine(multiplierCoroutine);

        float total = multiplierBar.fillAmount + multiplierAddedValue;
        multiplierBar.fillAmount += multiplierAddedValue;

        if (multiplierBar.fillAmount >= 1) //If the added value goes over 1 which is the max value
        {
            total -= 1; //Any remaining value past 1
            if (multiplier < maxMultiplier)
            {
                multiplier++;
                multiplierBar.fillAmount = total;
                playerScript.PlayerSpeed *= multiplier;
                playerScript.ShootDamage *= multiplier;
            }
            else
            {
                multiplierBar.fillAmount = 1;
            }
        }

        playerScript.ShootDamage *= multiplier;
        multiplierNumber.SetText("x" + multiplier.ToString());
        multiplierCoroutine = StartCoroutine(DecreaseMultiplier(multiplierResetTime));
    }

    IEnumerator DecreaseMultiplier(float seconds)
    {
        float initialValue = multiplierBar.fillAmount;
        float duration = seconds * initialValue;
        float remainingTime = duration;

        yield return new WaitForSeconds(timeBeforeDecrease);

        while (remainingTime >= 0)
        {
            multiplierBar.fillAmount = Mathf.Lerp(remainingTime / seconds, multiplierBar.fillAmount, .1f);
            yield return new WaitForSeconds(.1f);
            remainingTime -= .1f;
        }

        multiplier = 1;
        multiplierBar.fillAmount = 0;
        playerScript.PlayerSpeed = playerScript.OriginalPlayerSpeed;
        playerScript.ShootDamage = playerScript.OriginalShootDamage;
        multiplierNumber.SetText("x" + multiplier.ToString());
    }

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public soundManager.Sound sound;
        public AudioClip[] audioClips;
        public float audVolume;
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("musicVol", Mathf.Log10(sliderValue) * 20);
    }

    public float GetMusicVolume()
    {
        float value;
        bool result = audioMixer.GetFloat("musicVol", out value);
        if (result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

    public playerController PlayerScript { get { return playerScript; } }
    public GameObject PlayerSpawnPos { get { return playerSpawnPos; } }
    public Image HealthBar { get { return healthBar; } }
    public int Multiplier { get { return multiplier; } set { multiplier = value; } }
    public float MultiplierAddValue { get { return multiplierAddedValue; } set { multiplierAddedValue = value; } }
    public int MaxMultiplier { get { return maxMultiplier; } set { maxMultiplier = value; } }
    public float MultiplierBar { get { return multiplierBar.fillAmount; } set { multiplierBar.fillAmount = value; } }
}