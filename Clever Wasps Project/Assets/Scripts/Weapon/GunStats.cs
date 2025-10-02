using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public static GunStats instance;
    public GameObject model;
    public ParticleSystem hitEffect;
    public ParticleSystem misFire;
    public soundManager.Sound sound;
    public GameObject bulletType;
    private playerBullet bulletInstance;

    [Range(0f, 3f)] public float knockBackForce;
    [Range(0f, 3f)] public float cameraShake; 

    public int ammoCurr; 
    public int ammoMax;
    public float shootDamage;
    public int shootDistance;
    public float shootRate;

    private void Awake()
    {
        bulletInstance = bulletType.GetComponent<playerBullet>();

        bulletInstance.dmg = shootDamage;
    }
}
