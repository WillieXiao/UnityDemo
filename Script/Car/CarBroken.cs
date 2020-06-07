using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarBroken : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(transform.position.x-player.transform.position.x,8f,transform.position.z-player.transform.position.z)*500,ForceMode.Impulse);
        GameObject.Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
