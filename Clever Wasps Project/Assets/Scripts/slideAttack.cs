using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slideAttack : MonoBehaviour
{
    [Header("--- Melee Stats ---")]
    [Range(1, 10)][SerializeField] int dmg;
    [SerializeField] Collider weaponCol;


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.transform.parent == this)
            return;



        IDamage damageable = other.GetComponent<IDamage>();

        if (damageable != null && !other.CompareTag("Player"))
        {
            damageable.takeDamage(dmg);
        }

        //if (other.CompareTag("Player"))
        //{
        //    gameManager.instance.PlayerScript.takeDamage(3);
        //    gameManager.instance.PlayerScript.UpdatePlayerUI();
        //}

    }
}
