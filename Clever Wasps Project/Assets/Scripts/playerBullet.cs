using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("--- Bullet Stats ---")]
    [Range(1, 10)] public int dmg;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    public IDamage damageable;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed; ;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        damageable = other.GetComponent<IDamage>();

        if (damageable != null && !other.CompareTag("Player"))
        {
            Instantiate(gameManager.instance.PlayerScript.gunList[gameManager.instance.PlayerScript.gunSelection].hitEffect, transform.position, gameManager.instance.PlayerScript.gunList[gameManager.instance.PlayerScript.gunSelection].hitEffect.transform.rotation);
            //damageable.takeDamage(dmg);
        }
        else if (damageable == null && !other.CompareTag("Player"))
        {
            Instantiate(gameManager.instance.PlayerScript.gunList[gameManager.instance.PlayerScript.gunSelection].misFire, transform.position, gameManager.instance.PlayerScript.gunList[gameManager.instance.PlayerScript.gunSelection].misFire.transform.rotation);
        }
        Destroy(gameObject);
    }
}
