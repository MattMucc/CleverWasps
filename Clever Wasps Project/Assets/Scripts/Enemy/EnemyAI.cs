using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---- Component ---")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Collider damageCol;
    [SerializeField] GameObject Enemy;
    [SerializeField] ParticleSystem necroParticle;
    [SerializeField] Image healthBar;

    [Header("---- Enemy Stats ---")]
    [Range(1, 10)]public float HP;
    [SerializeField] int playerFaceSpeed;
    float hpOriginal;

    [Header("---- Blicky Stats ---")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Vector3 playerDir;
    bool isShooting;
    bool isDead = false;

    void Start()
    {
        if (transform.gameObject.CompareTag("Boss"))
        {
            healthBar = gameManager.instance.bossHealthBar;
            healthBar.transform.parent.gameObject.SetActive(true);
        }
        hpOriginal = HP;
        healthBar.fillAmount = 1;
        gameManager.instance.updateGameGoal(1);
        //removed win condition and added to EnemySpawn for now till boss added
        necroParticle = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (agent.isActiveAndEnabled)
        {
            anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
            playerDir = gameManager.instance.player.transform.position - transform.position;

            if (HP > 0 && !isShooting)
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
        if(isDead == false)
        {
            Instantiate(bullet, shootPos.position, transform.rotation);
        }
    }

    public void takeDamage(float amount)
    {
        HP -= amount;
        UpdateHealthBar();
        soundManager.PlaySound(soundManager.Sound.EnemyHit, Enemy);

        if (HP <= 0)
        {
            isDead = true;
            damageCol.enabled = false;
            agent.enabled = false;
            
            if(necroParticle != null)
            {
                necroParticle.Stop();
            }

            if (!transform.gameObject.CompareTag("Boss"))
                Destroy(healthBar.transform.parent.parent.gameObject);
            else
            {
                Destroy(healthBar.transform.parent.gameObject);
            }

            gameManager.instance.updateGameGoal(-1);
            anim.SetBool("Dead", true);

            StopCoroutine(shoot());
            fadeAway();
            
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

    void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)HP / hpOriginal;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);// Time delta time is frame rate independent 
    }

    void fadeAway()
    {
        SkinnedMeshRenderer enemyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (enemyRenderer != null)
        {
            StartCoroutine(fadeOut(enemyRenderer, 5f));
        }
        else
        {

            Destroy(gameObject);
        }
    }

    IEnumerator fadeOut(SkinnedMeshRenderer renderer, float fadeTime)
    {

        float progress = 0.0f;
        Vector3 startScale = transform.localScale;
        Vector3 minScale = Vector3.zero;
        float rate = 1.0f / fadeTime;

        while (progress < 1.0f)
        {
            transform.localScale = Vector3.Lerp(startScale, minScale, progress);
            progress += rate * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        }
        
}