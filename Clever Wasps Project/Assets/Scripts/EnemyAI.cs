using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---- Component ---")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
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
        //removed win condition and added to EnemySpawn for now till boss added
        
    }


    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            playerDir = gameManager.instance.player.transform.position - transform.position;

            if (!isShooting)
                StartCoroutine(shoot());
                      
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                faceTarget();
            }



            agent.SetDestination(gameManager.instance.player.transform.position);
        }
    }    
    IEnumerator shoot()
    {
        isShooting = true;

        anim.SetTrigger("Shoot");        
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    
    public void createBullet()
    {
        Instantiate(bullet, shootPos.position, transform.rotation);
    }


    public void takeDamage(int amount)
    {
            
            HP -= amount;

            if (HP <= 0)
            {
                 gameManager.instance.updateGameGoal(-1);
                 anim.SetBool("Dead", true);
                 agent.enabled = false;
                 
                 StopAllCoroutines();
                 
                 
                 

            }
            else
            {
                anim.SetTrigger("Damage");
                agent.SetDestination(gameManager.instance.player.transform.position);
                StartCoroutine(flashRed());
                
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