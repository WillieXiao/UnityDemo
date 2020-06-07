using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSound : MonoBehaviour
{
    public AudioSource hitedSource;
    public AudioSource swingWeaponSource;
    public AudioSource collisionSource;
    public AudioSource saySource;
    public AudioClip[] collisionSound;
    public AudioClip[] hitedSound;
    public AudioClip[] swingSound;
    public AudioClip[] shoutSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HitedSFX()
    {
        hitedSource.clip = hitedSound[Random.Range(0, hitedSound.Length-1)];
        hitedSource.Play();
    }
    public void SHitedSFX()
    {
        hitedSource.clip = hitedSound[3];
        hitedSource.Play();
    }
    public void SwingSFX()
    {
        swingWeaponSource.clip = swingSound[Random.Range(0, swingSound.Length)];
        swingWeaponSource.Play();
    }
    public void collisionSFX()
    {
        collisionSource.clip = collisionSound[Random.Range(0, collisionSound.Length)];
        collisionSource.Play();
    }
    public void ShoutSFX()
    {
        saySource.clip = shoutSound[Random.Range(0, shoutSound.Length)];
        saySource.Play();
    }
}
