using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittedPart : MonoBehaviour
{
    public bool hitted;
    public Vector3 dir;
    public bool right;
    DelegateHitted delegateHitted;
    Rigidbody rigidbody;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        delegateHitted = GetComponentInParent<DelegateHitted>();
        rigidbody = GetComponent<Rigidbody>();
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hitted)
        {
            delegateHitted.currentHitted = gameObject.GetComponent<HittedPart>();
        }
    }

}
