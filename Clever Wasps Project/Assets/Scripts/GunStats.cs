using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject model;
    public ParticleSystem hitEffect;
    public AudioClip shootSound;
    [Range(0,1)]public float shootSoundVol;

    public int ammoCurr; 
    public int ammoMax;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
}
