using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PakourSystem : MonoBehaviour
{
    private PlayerController player;
    private Animator animator;
    public LayerMask obLayer;
    private CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
        animator = GetComponentInParent<Animator>();
        controller = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        

        float random = Random.Range(0, 2);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position,1f,obLayer);
            if (colliders.Length>0)
            {
                
                if (colliders[0].transform.localScale.y>=1.6f)
                {
                    player.JumpForce(5.5f);
                }
                
                player.JumpForce(2.5f);
                switch (random)
                {
                    case 0:
                        animator.SetTrigger("OverObstacle");
                        
                        break;

                    case 1:
                        animator.SetTrigger("OverObstacle");
                        break;
                }

                colliders = null;
                gameObject.SetActive(false);
            }           
        }
       
        

    }
}
