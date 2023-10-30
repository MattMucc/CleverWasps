using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("---- Component ---")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
   

    [Header("---- Enemy Stats ---")]
    [Range(1,10)][SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;

    [Header("---- Blicky Stats ---")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    

    Vector3 playerDir;
    bool isShooting;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
