using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDelegate : MonoBehaviour
{
    private new BoxCollider collider;
    public LayerMask playerLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.CheckSphere(transform.position, 2.5f, playerLayer))
        {
            collider.isTrigger = false;
        }

    }
    
}
