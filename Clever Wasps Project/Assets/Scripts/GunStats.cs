using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject model;
    public ParticleSystem hitEffect;
    public ParticleSystem misFire;
    public soundManager.Sound sound;
    public GameObject bulletType;
    private playerBullet bulletInstance;

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
