using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonFunctions : MonoBehaviour
{
    public void Play()
    {
        MainMenuManager.menu.Play();
    }

    public void OpenSettings()
    {
        MainMenuManager.menu.OpenSettings();
    }

    public void Back()
    {
        MainMenuManager.menu.Back();
    }

    public void Quit()
    {
        Application.Quit();
    }
}