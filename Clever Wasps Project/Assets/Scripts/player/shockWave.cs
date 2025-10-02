using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shockWave : MonoBehaviour
{
    public float startRadius = 1f;
    public float endRadius = 5f;
    public float growthDuration = 3f; // Time it takes for the collider to grow
    public int numberOfColliders = 10; // Number of colliders forming the ring


    private float startTime;
    private CapsuleCollider[] colliders;
    private float originalHeight;
    private float[] originalRadius;

    void Start()
    {

        colliders = GetComponentsInChildren<CapsuleCollider>();

        originalRadius = new float[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            originalRadius[i] = colliders[i].radius;
            colliders[i].height = 0f;
        }

        startTime = Time.time;
    }

    void Update()
    {
        float timeSinceStart = Time.time - startTime;
        float t = Mathf.Clamp01(timeSinceStart / growthDuration);

        float currentRadius = Mathf.Lerp(startRadius, endRadius, t);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].radius = originalRadius[i] * currentRadius;
        }

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {           
            // Apply damage to the player
            other.GetComponent<playerController>().takeDamage(1);
        }

    }
}


