using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clusterExplosions : MonoBehaviour
{
    [Range(1,10)][SerializeField] int dmg;
    [SerializeField] ParticleSystem explosionEffect;
    //List of force numberes 
    [Range(1, 20)] float upForce; 
    
    // Start is called before the first frame update
    void Start()
    {
        if (explosionEffect != null)
        Instantiate(explosionEffect, new Vector3( transform.position.x,transform.position.y + 1.0f, transform.position.z + 1.0f),explosionEffect.transform.rotation);
        Instantiate(explosionEffect, new Vector3(transform.position.x + 1.0f, transform.position.y +1.0f, transform.position.z - 0.5f), explosionEffect.transform.rotation);
        Instantiate(explosionEffect, new Vector3(transform.position.x - 1.0f, transform.position.y + 1.0f, transform.position.z-0.5f), explosionEffect.transform.rotation);
        Destroy(gameObject, 0.1f);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        Rigidbody rb = GetComponent<Rigidbody>();
        IDamage damageble = other.GetComponent<IDamage>();
        if (damageble != null )
        {
            damageble.takeDamage(dmg);
        }
        Destroy(gameObject);
    }
}
