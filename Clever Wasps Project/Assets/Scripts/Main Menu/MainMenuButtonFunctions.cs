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

    public void OpenControls()
    {
        MainMenuManager.menu.OpenControls();
    }

    public void OpenCredits()
    {
        MainMenuManager.menu.OpenCredits();
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