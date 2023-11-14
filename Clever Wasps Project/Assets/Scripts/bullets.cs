using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullets : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("--- Bullet Stats ---")]
    [Range(1,10)][SerializeField] int dmg;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
            
        
            
        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null && other.CompareTag("Player"))
        {
            damageable.takeDamage(dmg);
        }
        Destroy(gameObject);
    }
 

}
