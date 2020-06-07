using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmedManager : MonoBehaviour
{
    public bool oneStar, twoStar, threeStar, fourStar, fiveStar;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (oneStar)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }
}
