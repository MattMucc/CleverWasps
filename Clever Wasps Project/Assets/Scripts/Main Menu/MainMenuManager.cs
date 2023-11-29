using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager menu;

    [Header("----- Menus -----")]
    public GameObject menuActive;
    public GameObject menuMain;
    public GameObject menuSettings;
    public GameObject gameControls;

    [Header("----- Loading Bar -----")]
    public GameObject loadingScreen;
    public Image loadingBar;
    public TMP_Text loadingText;

    

    private void Awake()
    {
        menu = this;
        menuActive = menuMain;
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
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            loadingBar.fillAmount = Mathf.Clamp01(progress / .9f);
            loadingText.text = (progress * 100).ToString() + " / 100";

            yield return null;
        }

        loadingBar.fillAmount = 1;

       
    }

    
}