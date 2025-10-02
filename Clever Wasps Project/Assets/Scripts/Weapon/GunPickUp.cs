using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    [SerializeField] GunStats gun;

    [SerializeField] GameObject button;

    bool playerInTrigger;
    // Start is called before the first frame update
    void Start()
    {
        gun.ammoCurr = gun.ammoMax;
    }

    private void Update()
    {
        if (playerInTrigger)
        {
            gameManager.instance.PlayerScript.getGunStats(gun);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            button.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            button.SetActive(false);
        }
    }
}
