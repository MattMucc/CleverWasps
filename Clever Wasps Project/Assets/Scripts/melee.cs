using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class melee : MonoBehaviour
{
    [Header("--- Melee Stats ---")]
    [Range(1, 10)][SerializeField] int dmg;
    
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;



        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.takeDamage(dmg);
        }
        
    }


}
