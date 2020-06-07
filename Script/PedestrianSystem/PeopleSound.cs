using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleSound : MonoBehaviour
{
    public AudioSource hitedSource;
    public AudioSource ouchSource;
    public AudioSource interactSource;
    public AudioClip[] ouchSound;
    public AudioClip[] hitedSound;
    public AudioClip[] hitedSound2;
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
        hitedSource.clip = hitedSound[Random.Range(0, hitedSound.Length)];
        hitedSource.Play();
    }
    public void OuchSFX()
    {
        ouchSource.clip = ouchSound[Random.Range(0, ouchSound.Length)];
        ouchSource.Play();
    }
}
