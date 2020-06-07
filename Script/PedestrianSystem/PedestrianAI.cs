using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PedestrianState
{
    Walking,
    Social,
    Fight,
    Parked,
}
public class PedestrianAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AudioSource audioSource;  
    PedestrianAttributes attributes;
    TrafficLight trafficLight;
    StairDismount stairDismount;
    RagdollHelper ragdoll;
    DelegateHitted hitted;
    public Collider[] attackPoint;
    HealthScript health;
    public PedestrianAI otherPedestrian;
    public GameObject mainPlayer;
    public GameObject[] pathPoint;
    public GameObject previousPoint;
    public GameObject nextCrossPoint;
    public WayStart originMainRoad;
    public bool arrive = false;
    public int a,b;
    private float x, z;
    public bool generalPedestrian, socialPedestrian;
    public bool atCrossing;
    public bool leadToCross = false;
    public bool Increment;
    public bool walking = false;
    public bool _tL_stop = false;
    private bool insideSemaphore;
    public bool fallDown;
    private bool attacking =true;
    public bool beCalled;
    private bool positive = true;
    [HideInInspector]
    public PedestrianState state;

    public GameObject target;
    public LayerMask player;
    public LayerMask restPlace;
    public LayerMask ob;
    public LayerMask otherPeople;
    
    public AudioClip Clip;
    public float walkSpeed;
    private float minDis = 20f;
    private float dis;
    private float afterAttackTime = 3f;
    private float attackDistance = 0.6f;
    private float withEnemyDistance;
    private int id;

    SpecialPoint currentSPoint;

    // Start is called before the first frame update
    void Start()
    {
        attributes = GetComponent<PedestrianAttributes>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        stairDismount = GetComponent<StairDismount>();
        ragdoll = GetComponent<RagdollHelper>();
        health = GetComponent<HealthScript>();
        mainPlayer = GameObject.FindGameObjectWithTag("Player");
        if (generalPedestrian)
        {
            walking = true;
            walkSpeed = Random.Range(0.8f, 1.2f);
            state = PedestrianState.Walking;
            x = Random.Range(-pathPoint[a].GetComponent<PathPoint>().x, pathPoint[a].GetComponent<PathPoint>().x);
            z = Random.Range(-pathPoint[a].GetComponent<PathPoint>().z, pathPoint[a].GetComponent<PathPoint>().z);
            agent.SetDestination(pathPoint[a].transform.position + new Vector3(x, 0, z));
        }

          
    }

    // Update is called once per frame
    void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        if (agent.enabled)
        {
            animator.SetFloat("Speed", speedPercent,0.1f,Time.deltaTime);
        }
        
        animator.SetFloat("WalkSpeed", walkSpeed);
        if (state == PedestrianState.Walking)
        {
            Walking();
        } 
    }
    void Walking()
    {
            
        if (walking&&!fallDown)
        {
            agent.stoppingDistance = 0.5f;
            if (!_tL_stop)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1.2f, ob))
                {
                    Debug.Log(12354);
                    trafficLight = hit.transform.GetComponent<TrafficLight>();
                    if (trafficLight!=null&&!trafficLight.canWalk)
                    {
                        Debug.Log(123544);
                        agent.isStopped = true;
                        _tL_stop = true;
                    }
                }
                RaycastHit hit2;
                if(Physics.SphereCast(transform.position,1f,transform.forward,out hit2, 1f, otherPeople))
                {
                    otherPedestrian = hit2.transform.GetComponent<PedestrianAI>();
                    if (otherPedestrian!=null&&otherPedestrian._tL_stop)
                    {
                        agent.isStopped = true;
                    }
                }
            }
            if (trafficLight != null)
            {
                if (trafficLight.canWalk)
                {
                    agent.isStopped = false;
                    _tL_stop = false;
                }
            }
            if(otherPedestrian != null)
            {
                if (!otherPedestrian._tL_stop)
                {
                    agent.isStopped = false;

                }
            }
            
            


            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !arrive)
            {
                if(a==pathPoint.Length-1&& !pathPoint[a].CompareTag("Cross"))
                {
                    InitializationPeople();
                    return;
                }

                if (pathPoint[a].CompareTag("Cross"))
                {
                    b = Random.Range(0, pathPoint[a].GetComponent<Crossing>().nextPoint.Length);
                    if(pathPoint[a].GetComponent<Crossing>().nextPoint[b] == previousPoint)
                    {
                        positive = false;
                    }
                    if (pathPoint[a].GetComponent<Crossing>().nextPoint[b] != previousPoint)
                        leadToCross = true;

                }

                if (leadToCross)
                {


                    previousPoint = pathPoint[a];
                    nextCrossPoint = pathPoint[a].GetComponent<Crossing>().nextPoint[b];

                    pathPoint = new GameObject[nextCrossPoint.GetComponentInParent<WayStart>().wayNode.Length];
                    for (int i = 0; i < nextCrossPoint.GetComponentInParent<WayStart>().wayNode.Length; i++)
                    {
                        pathPoint[i] = null;
                        pathPoint[i] = nextCrossPoint.GetComponentInParent<WayStart>().wayNode[i];

                    }
                    a = nextCrossPoint.GetComponent<Crossing>().pointIndex-1;
                    
                    leadToCross = false;

                }
                
                arrive = true;
                Debug.Log("123");
            }
            if (arrive)
            {

                if (a != pathPoint.Length - 1)
                {
                    Debug.Log("+");
                    a++;

                }
                x = Random.Range(-pathPoint[a].GetComponent<PathPoint>().x, pathPoint[a].GetComponent<PathPoint>().x);
                z = Random.Range(-pathPoint[a].GetComponent<PathPoint>().z, pathPoint[a].GetComponent<PathPoint>().z);

                agent.SetDestination(pathPoint[a].transform.position + new Vector3(x, 0, z));

                arrive = false;


            }
        }

    }

    public void InitializationPeople()
    {        
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        pathPoint = null;
        pathPoint = new GameObject[originMainRoad.wayNode.Length];
        for (int i = 0; i < originMainRoad.wayNode.Length; i++)
        {
            pathPoint[i] = originMainRoad.wayNode[i];
        }
        a = 0;
        if (generalPedestrian)
        {
            walking = true;
            walkSpeed = Random.Range(0.8f, 1.2f);
            state = PedestrianState.Walking;
            x = Random.Range(-pathPoint[a].GetComponent<PathPoint>().x, pathPoint[a].GetComponent<PathPoint>().x);
            z = Random.Range(-pathPoint[a].GetComponent<PathPoint>().z, pathPoint[a].GetComponent<PathPoint>().z);
            agent.SetDestination(pathPoint[a].transform.position + new Vector3(x, 0, z));           
        }
        gameObject.SetActive(false);
    }

      

  
    void saySound()
    {
        audioSource.clip = Clip;
        audioSource.Play();
    }


  

 
  
    public void AgentStop()
    {
        agent.isStopped = true;
    }
    public void AgentGoOn()
    {
        agent.isStopped = false;
    }

    
}
