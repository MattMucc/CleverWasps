using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    [SerializeField] int speed;
    [SerializeField] int destroyTimer;
    [SerializeField] int velUp;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject cGrenade;
    [SerializeField] Renderer model;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (Vector3.up * velUp) + (transform.forward) * speed;
        StartCoroutine(explode());
    }
    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTimer);
        if (explosion != null)
            StartCoroutine(explode());
        Instantiate(explosion, transform.position, explosion.transform.rotation);
      

        Instantiate(cGrenade, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z + 1.0f), transform.rotation);
        Instantiate(cGrenade, new Vector3(transform.position.x + 1.0f, transform.position.y + 1.0f, transform.position.z - .05f), transform.rotation);
        Instantiate(cGrenade, new Vector3(transform.position.x + 1.0f, transform.position.y + 1.0f, transform.position.z - .05f), transform.rotation);
        Destroy(gameObject);
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

}
