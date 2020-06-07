using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disaster : MonoBehaviour
{
    public GameObject player;
    public bool disasterOn;
    public bool disastering;
    public ParticleSystem[]disasterVFX;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (disasterOn)
        {
            disastering = true;
            for(int i = 0; i < disasterVFX.Length; i++)
            {
                disasterVFX[i].Play();
            }
            disasterOn = false;
        }
        if (disastering)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 10f)
            {
                if (player.GetComponent<PlayerController>().rescue)
                {
                    disastering = false;                   
                    for (int i = 0; i < disasterVFX.Length; i++)
                    {
                        disasterVFX[i].Stop();
                        player.GetComponent<SlowMotion>().DoSlowMotion();
                    }
                    player.GetComponent<PlayerController>().rescue = false;
                }
            }
        }
    }
}
