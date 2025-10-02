using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GroundEnemyAI : MonoBehaviour, IDamage
{
    [Header("---- Component ---")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;


    [Header("---- Enemy Stats ---")]
    [Range(1, 10)][SerializeField] float HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] GameObject explosion; 

    Vector3 playerDir;
    [SerializeField] int destroyTimer;
    bool playerInRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - transform.position;
        if (playerInRange)
        {
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                faceTarget();
                StartCoroutine(explode());
            }
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

    }
     void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            GetComponent<NavMeshAgent>().acceleration = 8;
        }
    }

    public void takeDamage(float amount)
    {

        HP -= amount;
        StartCoroutine(flashRed());


        GetComponent<NavMeshAgent>().acceleration = 8;


        if (HP <= 0)
        {
            StartCoroutine(explode());
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);// Time delta time is frame rate independent 
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTimer);
        if (explosion != null)
            Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }
}
