using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    [SerializeField] GunStats gun; 
    // Start is called before the first frame update
    void Start()
    {
        gun.ammoCurr = gun.ammoMax;
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.PlayerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
