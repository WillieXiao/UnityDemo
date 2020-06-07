using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class StairDismount : MonoBehaviour {
	//Declare a member variables for distributing the impacts over several frames
	float impactEndTime=0;
	public Rigidbody impactTarget;
	public Vector3 impact;
    //Current score
    public bool startEffect = false;
    public Vector3 dir;
    private float timer = 0;
    private float timeUp;
    private HealthScript health;
    public float power;
    private NavMeshAgent agent;
    private Animator animator;
    private PedestrianAI pedestrian;
    private ArmedAI armed;
    private PlayerController player;
    private BossAI boss;
    RagdollHelper helper;
    public bool isPeople, isArmed,isPlayer,isBoss;
    // Use this for initialization
    void Start () {

        if (isPeople)
        {
            pedestrian = GetComponent<PedestrianAI>();
        }
        else if (isArmed)
        {
            armed = GetComponent<ArmedAI>();
        }
        else if (isPlayer)
        {
            player = GetComponent<PlayerController>();
        }
        else if (isBoss)
        {
            boss = GetComponent<BossAI>();
        }
		//Get all the rigid bodies that belong to the ragdoll
		Rigidbody[] rigidBodies=GetComponentsInChildren<Rigidbody>();        
        animator = GetComponent<Animator>();
        //Add the RagdollPartScript to all the gameobjects that also have the a rigid body
        health = GetComponent<HealthScript>();
        agent = GetComponent<NavMeshAgent>();
        helper = GetComponent<RagdollHelper>();

    }
	
	// Update is called once per frame
	void Update () {
        //if left mouse button clicked
        timer = 0f + Time.time;
        if (startEffect == true)
		{
            //Get a ray going from the camera through the mouse cursor            
            //find the RagdollHelper component and activate ragdolling
            if (isPeople)
            {
                pedestrian.fallDown = true;
            }
            else if (isArmed)
            {
                armed.fighting = false;
                armed.fallDown = true;
            }
            else if (isPlayer)
            {
                player.fallDown = true;
                player.flying = false;
            }
            else if (isBoss)
            {
                boss.fallDown = true;
            }
            helper.ragdolled = true;
            if (agent != null)
            {
                agent.isStopped = true;
            }
            
            animator.applyRootMotion = false;           
            //set the impact target to whatever the ray hit

            //impact direction also according to the ray
            /*if (enemy.beLifted)
            {
                impact = (player.transform.forward).normalized;
            }
            else
            {
                impact = (transform.position - player.transform.position).normalized;
            }*/
            

            //the impact will be reapplied for the next 250ms
            //to make the connected objects follow even though the simulated body joints
            //might stretch
            impactEndTime = Time.time + 0.25f;
            timeUp = timer;
        }
		
		//Pressing space makes the character get up, assuming that the character root has
		//a RagdollHelper script
			
		
		//Check if we need to apply an impact
		if (Time.time<impactEndTime)
		{
			impactTarget.AddForce(impact*power,ForceMode.Impulse);
            startEffect = false;


            
		}
        if(timer>timeUp + Random.Range(2f,3.5f) && health.health > 0&& helper.ragdolled)
        {
            helper.ragdolled = false;
            if (isBoss)
            {
                boss.attacking = false;
                boss.attackEnd = true;
            }
            
            Debug.Log("135131");
        }
	}

    public void GetUp()
    {
        if (isPeople)
        {
            pedestrian.fallDown = false;
        }
        else if (isArmed)
        {
            armed.fallDown = false;
        }
        else if (isPlayer)
        {
            player.fallDown = false;
        }
        else if (isBoss)
        {
            boss.fallDown = false;
        }
    }
    
}
