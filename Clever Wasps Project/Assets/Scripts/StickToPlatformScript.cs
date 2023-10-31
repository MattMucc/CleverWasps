using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToPlatformScript : MonoBehaviour
{
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.name == "Player")
    //    {
    //        collision.gameObject.transform.SetParent(transform);
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.name == "Player")
    //    {
    //        collision.gameObject.transform.SetParent(null);
    //    }
    //}

    public GameObject Player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player)
        {
            Player.transform.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = null;
        }
    }
}
