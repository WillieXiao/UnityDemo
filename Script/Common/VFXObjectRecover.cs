using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXObjectRecover : MonoBehaviour
{
    public GameObject objectPool;
    public float recoverSecond;
    private float timer;
    private Vector3 currentPosition;
    public ParticleSystem[] VFX;
    public bool recover =true;
    public GameObject attackUniversal;
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != currentPosition)
        {         
            currentPosition = transform.position;
            timer = Time.time;
        }
        if (Time.time >= timer + recoverSecond)
        {
            transform.localPosition = Vector3.zero;
        }
    }
    public void VFX_Play()
    {
        for(int i = 0; i < VFX.Length; i++)
        {
            if (!VFX[i].isPlaying)
            {
                VFX[i].Stop();
                VFX[i].Play();
            }           
        }
    }
}
