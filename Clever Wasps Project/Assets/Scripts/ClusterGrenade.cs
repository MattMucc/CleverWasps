using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterGrenade : MonoBehaviour
{
    [SerializeField] float speed = 15.0f;
    [SerializeField] float destroyTimer = 3.0f;
    [SerializeField] float velUp = 10.0f;
    private int explosionCount = 0;
    [SerializeField] Rigidbody clusterBomb;
    [SerializeField] GameObject explosion;
    private float radius = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (clusterBomb == enabled)
        {
           
                Invoke("explode", 3);
         
        }
    }
    void explode()
    {

     
            Instantiate(explosion, transform.position, explosion.transform.rotation);
        Instantiate(explosion, transform.position, explosion.transform.rotation);
        //   Instantiate(clusterBomb, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z + 1.0f), transform.rotation);
        // Instantiate(clusterBomb, new Vector3(transform.position.x + 1.0f, transform.position.y + 1.0f, transform.position.z - .05f), transform.rotation);
        //Instantiate(clusterBomb, new Vector3(transform.position.x + 1.0f, transform.position.y + 1.0f, transform.position.z - .05f), transform.rotation);

        Vector3 explosionPos = clusterBomb.transform.position;
            Collider[] collide = Physics.OverlapSphere(explosionPos, radius);
            foreach (Collider hit in collide)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(speed, explosionPos, radius, velUp, ForceMode.Impulse);
                }
            }

            Destroy(explosion);
            Destroy(gameObject);
           
           
        


    }

}
