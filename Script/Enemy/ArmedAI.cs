using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum ArmedState
{
    Patrol,
    Arrest,
    Fight,
}
public class ArmedAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AnimatorStateInfo info;
    TrafficLight trafficLight;
    public WayStart originMainRoad;
    public AudioSource gunSound;
    public AudioSource talkSound;
    public AudioClip[] pistolSound;
    public AudioClip[] saySound;
    public bool isPolice,isArmed;
    [HideInInspector]
    public ArmedState state;
    private GameObject target;
    public GameObject[] pathPoint;
    public GameObject previousPoint;
    public GameObject nextCrossPoint;
    public bool walking = false;
    public bool _tL_stop = false;
    public bool arrive = false;
    private bool positive = true;
    public bool leadToCross = false;
    public bool attacking;
    public bool fighting;
    public bool fallDown;
    public bool wanted;
    public bool cover;
    private float attackDistance;
    public GameObject shootPoint;
    public GameObject bullet;
    public LayerMask partner;
    public LayerMask otherPeople;
    public LayerMask ob;
    public LayerMask coverPoint;
    public PedestrianAI otherPedestrian;
    public GameObject policeCB;
    public GameObject currentCoverPont;
    private float x, z;
    private float dis;
    private float minDis = 20;
    private int id;
    public int a, b;
    // Start is called before the first frame update
    void Start()
    {      
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (isPolice)
        {
            state = ArmedState.Patrol;
        }
        else if (isArmed)
        {
            state = ArmedState.Fight;
        }
        target = GameObject.FindGameObjectWithTag("Player");
        attacking = true;
        //talkSound.clip = saySound[0];
        //talkSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (agent.enabled)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed, 0.1f, Time.deltaTime);
        }
        if(state == ArmedState.Patrol)
        {
            Patrol();
        }
        else if (state == ArmedState.Arrest)
        {
            Arrest();
        }
        else if(state == ArmedState.Fight)
        {
            Fight();
        }
    }

    void Patrol()
    {
        walking = true;
        if (Vector3.Distance(target.transform.position, transform.position)<10f&&wanted)
        {
            fighting = true;
            animator.SetBool("Fight", fighting);
            state = ArmedState.Fight;
        }
        if (walking && !fallDown)
        {
            if (!_tL_stop)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 1.2f, ob))
                {
                    Debug.Log(12354);

                    trafficLight = hit.transform.GetComponent<TrafficLight>();
                    if (trafficLight != null && !trafficLight.canWalk)
                    {
                        Debug.Log(123544);
                        agent.isStopped = true;
                        _tL_stop = true;
                    }
                }
                RaycastHit hit2;
                if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit2, 1f, otherPeople))
                {
                    otherPedestrian = hit2.transform.GetComponent<PedestrianAI>();
                    if (otherPedestrian != null && otherPedestrian._tL_stop)
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
            if (otherPedestrian != null)
            {
                if (!otherPedestrian._tL_stop)
                {
                    agent.isStopped = false;

                }
            }




            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !arrive)
            {
                if (a == pathPoint.Length - 1 && !pathPoint[a].CompareTag("Cross"))
                {
                    InitializationPeople();
                    return;
                }

                if (pathPoint[a].CompareTag("Cross"))
                {
                    b = Random.Range(0, pathPoint[a].GetComponent<Crossing>().nextPoint.Length);
                    if (pathPoint[a].GetComponent<Crossing>().nextPoint[b] == previousPoint)
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
                    a = nextCrossPoint.GetComponent<Crossing>().pointIndex - 1;

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
        gameObject.SetActive(false);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        pathPoint = new GameObject[originMainRoad.wayNode.Length];
        for (int i = 0; i < originMainRoad.wayNode.Length; i++)
        {
            pathPoint[i] = originMainRoad.wayNode[i];
        }
        a = 0;
    }


    void Arrest()
    {
        agent.stoppingDistance = 1f;
        agent.SetDestination(target.transform.position);
    }



    void Fight()
    {
        if (!fallDown&&isPolice)
        {
            agent.stoppingDistance = 8f;
            info = animator.GetCurrentAnimatorStateInfo(0);
            attackDistance = Vector3.Distance(target.transform.position, transform.position);
            agent.updateRotation = false;
            Quaternion q = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.1f);
            if (attackDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;                
                if (attacking&&info.IsTag("Fight"))
                {
                    StartCoroutine(Shoot());
                    attacking = false;
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position + new Vector3(Random.Range(-1f,1f),0f,Random.Range(-1f,1f)));
            }
            if (attackDistance <= 2f)
            {
                animator.SetBool("Back", true);

            }
            else if (attackDistance >= 4f)
            {
                animator.SetBool("Back", false);
            }
           
            Collider[] hit = Physics.OverlapSphere(policeCB.transform.position, 0.5f, partner);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].GetComponentInParent<ArmedAI>().gameObject != gameObject)
                    {
                        Vector3 dir2 = (hit[i].transform.position - transform.position);
                        if (Vector3.Cross(transform.forward, dir2).y > 0)
                        {
                            animator.SetBool("StrafeLeft", true);
                        }
                        else
                        {
                            animator.SetBool("StrafeRight", true);
                        }
                    }
                    else
                    {
                        animator.SetBool("StrafeLeft", false);
                        animator.SetBool("StrafeRight", false);
                    }
                }
            }
            else
            {
                animator.SetBool("StrafeLeft", false);
                animator.SetBool("StrafeRight", false);
            }
        }



        else if (!fallDown && isArmed)
        {           
            info = animator.GetCurrentAnimatorStateInfo(0);
            attackDistance = Vector3.Distance(target.transform.position, transform.position);
            if (currentCoverPont == null)
            {
                Collider[] cover = Physics.OverlapSphere(transform.position, 20f, coverPoint);
                if (cover.Length > 0)
                {
                    for (int i = 0; i < cover.Length; i++)
                    {
                        dis = Vector3.Distance(cover[i].transform.position, transform.position);
                        Vector3 coverWithMe = transform.position - cover[i].transform.position;
                        bool meAtFoward = Vector3.Cross(cover[i].transform.right, coverWithMe).y < 0;
                        Vector3 coverWithTarget =  target.transform.position - cover[i].transform.position;
                        bool targetAtFoward = Vector3.Cross(cover[i].transform.right, coverWithTarget).y < 0;
                        if (dis < minDis && (!meAtFoward && targetAtFoward)&&!cover[i].GetComponent<CoverPoint>().occupy)
                        {
                            minDis = dis;
                            id = i;
                        }
                    }                    
                    currentCoverPont = cover[id].gameObject;
                    agent.stoppingDistance = 0.6f;
                    agent.SetDestination(currentCoverPont.transform.position);                  
                    /*if (attacking)
                    {
                        StartCoroutine(Shoot());
                        attacking = false;
                    }*/
                }
            }
            else
            {
                if (currentCoverPont.GetComponent<CoverPoint>().occupy&& currentCoverPont.GetComponent<CoverPoint>().occuper!=gameObject)
                {
                    currentCoverPont = null;
                }
            }
            if (Vector3.Distance(currentCoverPont.transform.position,transform.position) <= agent.stoppingDistance)
            {
                agent.updateRotation = false;
                currentCoverPont.GetComponent<CoverPoint>().occupy = true;
                currentCoverPont.GetComponent<CoverPoint>().occuper = gameObject;
                if (!cover)
                {
                    if (currentCoverPont.GetComponent<CoverPoint>().HiL)
                    {
                        animator.SetBool("HiCoverL", true);
                        cover = true;
                    }
                }

                if (attacking)
                {
                    StartCoroutine(Shoot());
                    attacking = false;
                }
            }
        }
    }
    public IEnumerator Shoot()
    {
        cover = false;
        yield return new WaitForSeconds(4f);
        Quaternion q = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 1f);
        animator.SetBool("RifleShoot", true);
        yield return new WaitForSeconds(4f);
        if (isPolice&&attackDistance <= agent.stoppingDistance+0.5f&&!fallDown&&info.IsTag("Fight"))
        {
            animator.SetTrigger("Shoot");
            gunSound.clip = pistolSound[Random.Range(0, 2)];
            gunSound.Play();
            Instantiate(bullet, shootPoint.transform.position, shootPoint.transform.rotation);
        }
        animator.SetBool("RifleShoot", false);
        attacking = true;
        cover = true;
    }
    public void CoverRotation()
    {
        Quaternion q = Quaternion.Euler(0, -90, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 0.1f);
    }
}
