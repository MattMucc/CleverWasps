using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class knockback : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if( Input.GetMouseButtonDown(1))
        {
            Vector3 hitPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 forceDirection = transform.position = hitPosition;

            GetComponent<Rigidbody>().AddForce(forceDirection);
          
        }
    }
    IEnumerator knockBackDelay()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.enabled = false;
            yield return new WaitForSeconds(.3f);
            agent.enabled = true;
            
        }


    }
}
