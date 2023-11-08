using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---- Component ---")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;


    [Header("---- Enemy Stats ---")]
    [Range(1, 10)][SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;

    [Header("---- Blicky Stats ---")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;


    Vector3 playerDir;
    bool isShooting;

    void Start()
    {
        gameManager.instance.updateGameGoal(1);

    }


    void Update()
    {

        playerDir = gameManager.instance.player.transform.position - transform.position;

        if (!isShooting)
            StartCoroutine(shoot());




        if (agent.remainingDistance < agent.stoppingDistance)
        {
            faceTarget();
        }

        agent.SetDestination(gameManager.instance.player.transform.position);
    }
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {

        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);// Time delta time is frame rate independent 
    }
}