using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject[] enemiesToSpawn;
    [SerializeField] int numberOfEnemies;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] Collider spawnColliders;

    int spawnCount;
    bool isSpawning;
    bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        //gameManager.instance.updateGameGoal(numberOfEnemies);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numberOfEnemies)
        {
            StartCoroutine(spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawnColliders.enabled = false;
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        int randomNum = Random.Range(0, spawnPos.Length);
        int randomNum2 = Random.Range(0, enemiesToSpawn.Length);
        Instantiate(enemiesToSpawn[randomNum2], spawnPos[randomNum].position, spawnPos[randomNum].rotation);
        spawnCount++;

        yield return new WaitForSeconds(timeBetweenSpawns);

        isSpawning = false;
    }
}
