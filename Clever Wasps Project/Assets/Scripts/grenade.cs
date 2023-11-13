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
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = (Vector3.up * velUp) + (transform.forward) * speed;
        StartCoroutine(explode());
    }
    IEnumerator explode()
    {
        yield return new WaitForSeconds(destroyTimer);
        if(explosion !=null)
        Instantiate(explosion, transform.position, explosion.transform.rotation);
        Destroy(gameObject);
    }

}
