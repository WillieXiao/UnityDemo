using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftSystem : MonoBehaviour
{
    private PlayerController player;
    private Animator animator;
    public GameObject liftPoint;
    public LayerMask obLayer;
    public GameObject liftTarget;
    private bool shoot = false;
    bool lifting;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.lifting)
        {
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5f, obLayer);
            if (colliders.Length > 0)
            {
                
                //colliders[0].attachedRigidbody.useGravity = false;
                liftTarget = colliders[0].gameObject;
                
                //liftTarget.GetComponentInParent<PedestrianAI>().beLifted = true;
                player.lifting = false;
            }
        }

        if (liftTarget != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                shoot = true;
                liftTarget.transform.GetComponentInParent<StairDismount>().impactTarget = liftTarget.GetComponent<Rigidbody>();
                liftTarget.transform.GetComponentInParent<StairDismount>().power = 15;
                liftTarget.transform.GetComponentInParent<StairDismount>().startEffect = true;

               
                shoot = false;
                liftTarget = null;
                lifting = false;
                return;

            }
            player.lifting = false;
            if (Vector3.Distance(liftTarget.transform.root.position, liftPoint.transform.position) <= 1f&&!shoot)
            {
                return;
            }
            else if(Vector3.Distance(liftTarget.transform.root.position, liftPoint.transform.position) > 1f&&!shoot)
            {
                liftTarget.transform.root.position = Vector3.MoveTowards(liftTarget.transform.root.position, liftPoint.transform.position, 10 * Time.deltaTime);
            }           
            Debug.Log(123);
        }


        
    }
}
