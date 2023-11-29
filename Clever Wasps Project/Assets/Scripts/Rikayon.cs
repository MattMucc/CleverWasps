using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Rikayon : MonoBehaviour, IDamage 
{
	[Header("---- Component ----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Collider damageCol;
    [SerializeField] Collider weaponCol;
    [SerializeField] GameObject Enemy;
    [SerializeField] Image healthBar;

    [Header("---- Enemy Stats ---")]
    [Range(1, 100)] public int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int viewCone;
    [SerializeField] int shootCone;
    int hpOriginal;

    [Header("---- Melee Stats ---")]
    [SerializeField] GameObject claws;
    [SerializeField] float attackRate;

    [Header("------ Laser Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 playerDir;
    bool isShooting;
    bool isAttacking;
    bool playerInRange;
    float angleToPlayer;
    float stoppingDistOrig;

    // Use this for initialization
    void Start ()
    {
        if (transform.gameObject.CompareTag("Boss"))
        {
            healthBar = gameManager.instance.bossHealthBar;
            healthBar.transform.parent.gameObject.SetActive(true);
        }
        gameManager.instance.PlayerScript.isMusicPlayable = false;
        gameManager.instance.PlayerScript.audioSource.clip = gameManager.instance.PlayerScript.bossMusic;
        gameManager.instance.PlayerScript.audioSource.Play();
        hpOriginal = HP;
        healthBar.fillAmount = 1;
        gameManager.instance.updateGameGoal(1);
        //removed win condition and added to EnemySpawn for now till boss added
    }

    // Update is called once per frame
    void Update () 
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            playerDir = gameManager.instance.player.transform.position - transform.position;

            
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                faceTarget();
            }

            agent.SetDestination(gameManager.instance.player.transform.position);
        }


        // need to change to if player is within range of melee
        // do attack_1 - attack_5 randomly
        if (playerInRange) 
        {
            anim.SetTrigger("Attack");
        }
        if(!playerInRange)
        {
            anim.SetTrigger("Shoot");
        }

	}

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);
        Debug.Log(angleToPlayer);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewCone)
            {
                agent.stoppingDistance = stoppingDistOrig;

                if (angleToPlayer <= shootCone && !isAttacking)
                    StartCoroutine(attack());

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                agent.SetDestination(gameManager.instance.player.transform.position);

                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        UpdateHealthBar();
        soundManager.PlaySound(soundManager.Sound.EnemyHit, Enemy);

        if (HP <= 0)
        {
            damageCol.enabled = false;
            agent.enabled = false;
            //gameManager.instance.updateGameGoal(-1);

            if (!transform.gameObject.CompareTag("Boss"))
                Destroy(healthBar.transform.parent.parent.gameObject);
            else
            {
                Destroy(healthBar.transform.parent.gameObject);
            }

            gameManager.instance.updateGameGoal(-1);
            anim.SetBool("Die", true);

        }
        else
        {
            anim.SetTrigger("TakeDamage");
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

    IEnumerator attack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackRate);
        isAttacking = false;

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

    void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)HP / hpOriginal;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);// Time delta time is frame rate independent 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    public void weaponColOn()
    {
        weaponCol.enabled = true;
    }

    public void weaponColOff()
    {
        weaponCol.enabled = false;
    }
}
