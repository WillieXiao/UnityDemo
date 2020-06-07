using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource flySpraySource;
    public AudioSource flySource;
    public AudioSource moveSource;
    public AudioSource hitedSource;
    public AudioClip[] spraySound;
    public AudioClip[] flyingSound;
    public AudioClip[] landSound;
    public AudioClip footStep;
    public AudioClip[] swingPunchSound;
    public AudioClip[] swingSwordSound;
    public AudioClip emissionSound;
    public AudioClip[] hitedSound;
    public AudioClip[] blockSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FlySFX()
    {
        flySource.clip = flyingSound[0];
        flySource.Play();
    }
    public void MeleeAttackSFX()
    {
        moveSource.clip = swingPunchSound[Random.Range(0, 6)];
        moveSource.volume = 1f;
        moveSource.Play();
    }
    public void SwordAttackSFX()
    {
        moveSource.clip = swingSwordSound[Random.Range(0,swingSwordSound.Length)];
        moveSource.volume = 1f;
        moveSource.Play();
    }
    public void EmissionSFX()
    {
        moveSource.clip = emissionSound;
        moveSource.volume = 1f;
        moveSource.Play();
    }
    public void SpraySFX(int index)
    {
        flySpraySource.clip = spraySound[index];
        flySpraySource.volume = 1f;
        flySpraySource.Play();
    }
    public void LandSFX(int index)
    {
        moveSource.clip = landSound[index];
        moveSource.Play();
    }
    public void FootStepSFX()
    {
        moveSource.clip = footStep;
        moveSource.volume = 0.5f;
        moveSource.Play();
    }
    public void HitedSFX()
    {
        hitedSource.clip = hitedSound[Random.Range(0, hitedSound.Length)];
        hitedSource.Play();
    }
    public void BlockSFX(int index)
    {
        if(index == 0)
        {
            hitedSource.clip = blockSound[Random.Range(0, blockSound.Length-1)];
            hitedSource.Play();
        }
        else if(index == 1)
        {
            hitedSource.clip = blockSound[2];
            hitedSource.Play();
        }
    }
}
