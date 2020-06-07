using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject[] playersVFX;
    private GameObject result;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < playersVFX.Length; i++)
        {
            result = Instantiate<GameObject>(playersVFX[i], transform.position, Quaternion.identity, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
