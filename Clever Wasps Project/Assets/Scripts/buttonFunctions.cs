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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayerRespawn()
    {
        gameManager.instance.PlayerScript.PlayerSpawn();
        gameManager.instance.stateUnpause();
    }
}