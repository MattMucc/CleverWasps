using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosions : MonoBehaviour
{
    [Range(1,10)][SerializeField] float dmg;
    [SerializeField] ParticleSystem explosionEffect; 
    
    // Start is called before the first frame update
    void Start()
    {
        if (explosionEffect != null)
        Instantiate(explosionEffect, transform.position, explosionEffect.transform.rotation); 

        Destroy(gameObject, 0.1f);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;


        IDamage damageble = other.GetComponent<IDamage>();
        if (damageble != null )
        {
            damageble.takeDamage(dmg);
        }
        Destroy(gameObject);
    }
}
