using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void Restart()
    {
        StartCoroutine(RestartDelay());
    }

    public void Quit()
    {
        StartCoroutine(QuitDelay());
    }

    public void PlayerRespawn()
    {
        //gameManager.instance.PlayerScript.anim.SetBool("Dead", false);
        StartCoroutine(RespawnDelay());
    }

    public void OpenSettings()
    {
        gameManager.instance.OpenSettings();
    }

    public void OpenControls()
    {
        gameManager.instance.OpenControls();
    }

    public void Back()
    {
        gameManager.instance.Back();
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        gameManager.instance.PlayerScript.PlayerSpawn();
        gameManager.instance.stateUnpause();
    }

    IEnumerator RestartDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    IEnumerator QuitDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        Application.Quit();
    }
}