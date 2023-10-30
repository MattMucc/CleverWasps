using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Menu -----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;

    [Header("----- Multiplier -----")]
    [SerializeField] Slider multiplierBar;
    [SerializeField] TMP_Text multiplierNumber;
    [SerializeField] int multiplier;
    [SerializeField] float timeBeforeDecrease;
    [SerializeField] float multiplierResetTime;
    [SerializeField] float multiplierAddedValue;
    Coroutine multiplierCoroutine;

    public GameObject player;

    public bool isPaused;
    float timescaleOrig;
    int enemiesRemaining;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timescaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");

        multiplier = 5;
        multiplierBar.value = multiplierBar.maxValue;
        multiplierNumber.SetText("x" + multiplier.ToString());
        multiplierCoroutine = StartCoroutine(DecreaseMultiplier(multiplierResetTime));
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel") && menuActive == null)
        {
            statePause();
            menuActive = menuPause;
            menuActive.SetActive(isPaused);
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timescaleOrig;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        int previousEnemiesRemaining = enemiesRemaining;
        enemiesRemaining += amount;

        if (previousEnemiesRemaining > enemiesRemaining) //Player killed an enemy
        {
            StopCoroutine(multiplierCoroutine);

            float total = multiplierBar.value + multiplierAddedValue;
            multiplierBar.value += multiplierAddedValue;

            if (multiplierBar.value >= 1) //If the added value goes over 1 which is the max value
            {
                total -= 1; //Any remaining value past 1
                if (multiplier < 5)
                    multiplier++;
                multiplierBar.value = total;
            }

            multiplierNumber.SetText("x" + multiplier.ToString());
            multiplierCoroutine = StartCoroutine(DecreaseMultiplier(multiplierResetTime));
        }

        if(enemiesRemaining <= 0)
        {
            youWin();
        }
    }

    public void youWin()
    {
        statePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    IEnumerator DecreaseMultiplier(float seconds)
    {
        float initialValue = multiplierBar.value;
        float duration = seconds * initialValue;
        float remainingTime = duration;

        yield return new WaitForSeconds(timeBeforeDecrease);

        while (remainingTime >= 0)
        {
            multiplierBar.value = remainingTime / seconds;
            yield return new WaitForSeconds(.1f);
            remainingTime -= .1f;
        }

        multiplier = 1;
        multiplierBar.value = multiplierBar.minValue;
        multiplierNumber.SetText("x" + multiplier.ToString());
    }
}