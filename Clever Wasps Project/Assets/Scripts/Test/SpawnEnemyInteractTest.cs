using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyInteractTest : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    Transform player;
    bool playerIsInTrigger;
    bool playerWasOriginallyInTrigger;

    private void Update()
    {
        OnPause();
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!enemyPrefab)
            {
                Debug.LogError("Enemy prefab is not set!");
                return;
            }

            if (!player)
            {
                Debug.LogError("Player is null!");
                return;
            }

            Vector3 spawnPos = transform.position;
            spawnPos += player.forward * 5f;

            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.position = spawnPos;
        }
    }

    private void OnPause()
    {
        if (gameManager.instance.isPaused)
        {
            if (playerIsInTrigger)
            {
                playerIsInTrigger = false;
                playerWasOriginallyInTrigger = true;
            }
        }
        else
        {
            if (playerWasOriginallyInTrigger)
            {
                playerIsInTrigger = true;
                playerWasOriginallyInTrigger = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInTrigger = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInTrigger = false;
            player = null;
        }
    }
}