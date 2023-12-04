using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager menu;
    public VolumeControl volumeControl;

    [Header("----- Menus -----")]
    public GameObject menuActive;
    public GameObject menuMain;
    public GameObject menuSettings;
    public GameObject gameControls;
    public GameObject gameCredits;

    [Header("----- Loading Bar -----")]
    public GameObject loadingScreen;
    public TMP_Text loadingBanner;
    public Image loadingBar;
    public TMP_Text loadingText;
    public TMP_Text clickText;
    public Image barBackground;

    private void Awake()
    {
        menu = this;
        menuActive = menuMain;
        volumeControl = gameObject.GetComponent<VolumeControl>();
    }

    public void Play()
    {
        menuActive.SetActive(false);
        menuActive = loadingScreen;
        menuActive.SetActive(true);

        StartCoroutine(LoadScene());
    }

    public void OpenSettings()
    {
        menuActive.SetActive(false);
        menuActive = menuSettings;
        menuActive.SetActive(true);
    }

    public void OpenControls()
    {
        menuActive.SetActive(false);
        menuActive = gameControls;
        menuActive.SetActive(true);
    }

    public void OpenCredits()
    {
        menuActive.SetActive(false);
        menuActive = gameCredits;
        menuActive.SetActive(true);
    }

    public void Back()
    {
        menuActive.SetActive(false);
        menuActive = menuMain;
        menuActive.SetActive(true);
    }

    IEnumerator LoadScene()
    {        
        loadingBar.fillAmount = 0;
        loadingText.text = "0 / 100";
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        operation.allowSceneActivation = false;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            loadingBar.fillAmount = Mathf.Clamp01(progress / .9f);
            loadingText.text = (progress * 100).ToString() + " / 100";

            if (operation.progress == 0.9f)
            {
                loadingBar.gameObject.SetActive(false);
                loadingText.gameObject.SetActive(false);
                barBackground.gameObject.SetActive(false);
                loadingBanner.gameObject.SetActive(false);

                clickText.gameObject.SetActive(true);

                if (Input.anyKey)
                    operation.allowSceneActivation = true;
            }

            yield return null;
        }

      

        loadingBar.fillAmount = 1;

        

    }
}