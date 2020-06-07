using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{

    private Rigidbody rigbody;
    private BoxCollider bc;
    private CarMove carMove;
    private HealthScript health;
    private WayStart way;
    public RebirthPoint rebirth;
    public GameObject[] pathPoint;
    public Vector3 currentPathPoint;
    public Vector3 nextPathPoint;
    public WayStart originMainRoad;
    int index = 0;
    public int a = 0;
    private Vector3 fwdVector;
    private Vector3 fwdVector2;
    private Vector3 LRVector;
    private float startSpeed;
    [SerializeField] private float curMoveSpeed;
    [SerializeField] private float angleBetweenPoint;
    private float targetSteerAngle;
    private float upTurnTimer;
    private bool arrive;
    private bool moveBrake;
    private bool isACar;
    private bool isABike;
    public bool isPolice;
    public bool tempStop;
    public bool insideSemaphore;
    private bool hasTrailer;
    public bool turn;
    public bool reverse;
    public bool overtake;
    public LayerMask collisionOb;
    public Collider ob;
    public ArmedManager armedManager;
    [SerializeField] [Tooltip("Vehicle Speed / Скорость автомобиля")] private float moveSpeed;
    [SerializeField] [Tooltip("Acceleration of the car / Ускорение автомобиля")] private float speedIncrease;
    [SerializeField] [Tooltip("Deceleration of the car / Торможение автомобиля")] private float speedDecrease;
    [SerializeField] [Tooltip("Distance to the car for braking / Дистанция до автомобиля для торможения")] private float distanceToCar;
    [SerializeField] [Tooltip("Distance to the traffic light for braking / Дистанция до светофора для торможения")] private float distanceToSemaphore;
    [SerializeField] [Tooltip("Maximum rotation angle for braking / Максимальный угол поворота для притормаживания")] private float maxAngleToMoveBreak = 8.0f;

    public float MOVE_SPEED
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public float INCREASE
    {
        get { return speedIncrease; }
        set { speedIncrease = value; }
    }

    public float DECREASE
    {
        get { return speedDecrease; }
        set { speedDecrease = value; }
    }

    public float START_SPEED
    {
        get { return startSpeed; }
        private set { }
    }

    public float TO_CAR
    {
        get { return distanceToCar; }
        set { distanceToCar = value; }
    }

    public float TO_SEMAPHORE
    {
        get { return distanceToSemaphore; }
        set { distanceToSemaphore = value; }
    }

    public float MaxAngle
    {
        get { return maxAngleToMoveBreak; }
        set { maxAngleToMoveBreak = value; }
    }

    public bool INSIDE
    {
        get { return insideSemaphore; }
        set { insideSemaphore = value; }
    }

    public bool TEMP_STOP
    {
        get { return tempStop; }
        private set { }
    }

    private void Awake()
    {
        rigbody = GetComponent<Rigidbody>();
        carMove = GetComponent<CarMove>();
    }
    void Start()
    {
        health = GetComponent<HealthScript>();
        currentPathPoint = pathPoint[a].transform.position;
        nextPathPoint = pathPoint[a + 1].transform.position;
        startSpeed = moveSpeed;
        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>();

        if (wheelColliders.Length > 2)
        {
            isACar = true;
        }
        else
        {
            isABike = true;
        }

        BoxCollider[] box = GetComponentsInChildren<BoxCollider>();
        bc = (isACar) ? box[0] : box[1];

        if (GetComponent<AddTrailer>())
        {
            hasTrailer = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (ob != null)
        {
            if (Vector3.Distance(ob.transform.position, transform.position) < 3.5f)
            {
                reverse = true;
            }
            else
            {
                reverse = false;
            }
        }*/
        fwdVector = new Vector3(transform.position.x + (transform.forward.x * bc.size.z / 2 + 0.1f), transform.position.y + 0.5f, transform.position.z + (transform.forward.z * bc.size.z / 2 + 0.1f));
        fwdVector2 = new Vector3(transform.position.x + (transform.forward.x * bc.size.z / 2), transform.position.y + 0.5f, transform.position.z + (transform.forward.z * bc.size.z / 2));
        LRVector = new Vector3(transform.position.x + (transform.forward.x * bc.size.z / 2 + 0.1f), transform.position.y + 0.5f, transform.position.z + (transform.forward.z * bc.size.z / 2 + 0.1f));
        PushRay();
        if (carMove != null && isACar) carMove.Move(curMoveSpeed, 0, 0);
        //*((reverse) ? -1 : 1)
    }
    private void FixedUpdate()
    {   
        Drive();
        GetPath();
        if (moveBrake)
        {
            moveSpeed = startSpeed * 0.5f;
        }
    }
    private void GetPath()
    {
        Vector3 targetPos = new Vector3(currentPathPoint.x, rigbody.transform.position.y, currentPathPoint.z);
        if (Vector3.Distance(transform.position, currentPathPoint) <= 1f)
        {
            
            arrive = true;          
            if (a == pathPoint.Length - 1&&!pathPoint[a].CompareTag("RelayEnd")&&!pathPoint[a].CompareTag("Cross"))
            {

                InitializationCar();
                return;
            }
        }
        if (arrive)
        {
            if (pathPoint[a].CompareTag("RelayEnd"))
            {
                GameObject branchNextPoint = pathPoint[a].GetComponent<PathPoint>().branchNextPoint;
                pathPoint = new GameObject[branchNextPoint.GetComponentInParent<WayStart>().wayNode.Length];
                for (int i = 0; i < branchNextPoint.GetComponentInParent<WayStart>().wayNode.Length; i++)
                {
                    pathPoint[i] = branchNextPoint.GetComponentInParent<WayStart>().wayNode[i];
                }
                a = branchNextPoint.GetComponent<PathPoint>().index;
                currentPathPoint = pathPoint[a].transform.position;
                if (a != pathPoint.Length - 1)
                {
                    nextPathPoint = pathPoint[a + 1].transform.position;
                }
                else
                {
                    nextPathPoint = currentPathPoint;
                }
                arrive = false;
            }
            else if (pathPoint[a].CompareTag("Cross"))
            {
                int x = Random.Range(0,1);
                
                if (x == 0)
                {
                    GameObject Branch = pathPoint[a].GetComponent<Crossing>().nextPoint[0];
                    pathPoint = new GameObject[Branch.GetComponent<WayStart>().wayNode.Length];
                    for (int i = 0; i < Branch.GetComponent<WayStart>().wayNode.Length; i++)
                    {
                        pathPoint[i] = Branch.GetComponent<WayStart>().wayNode[i];
                    }
                    a = 0;
                    currentPathPoint = pathPoint[a].transform.position;
                    if (a != pathPoint.Length - 1)
                    {
                        nextPathPoint = pathPoint[a + 1].transform.position;
                    }
                    else
                    {
                        nextPathPoint = currentPathPoint;
                    }
                    arrive = false;
                }
                else if (x == 1)
                {
                    GameObject Branch = pathPoint[a].GetComponent<Crossing>().nextPoint[1];
                    pathPoint = new GameObject[Branch.GetComponent<WayStart>().wayNode.Length];
                    for (int i = 0; i < Branch.GetComponent<WayStart>().wayNode.Length; i++)
                    {
                        pathPoint[i] = Branch.GetComponent<WayStart>().wayNode[i];
                    }
                    a = 0;
                    currentPathPoint = pathPoint[a].transform.position;
                    if (a != pathPoint.Length - 1)
                    {
                        nextPathPoint = pathPoint[a + 1].transform.position;
                    }
                    else
                    {
                        nextPathPoint = currentPathPoint;
                    }
                    arrive = false;
                }
                else if (x == 2&& pathPoint[a].GetComponent<Crossing>().haveNextMainPoint)
                {
                    a++;
                    currentPathPoint = pathPoint[a].transform.position;
                    if (a != pathPoint.Length - 1)
                    {
                        nextPathPoint = pathPoint[a + 1].transform.position;
                    }
                    else
                    {
                        nextPathPoint = currentPathPoint;
                    }
                    arrive = false;
                }
                else if(x == 2 && !pathPoint[a].GetComponent<Crossing>().haveNextMainPoint)
                {
                    GameObject Branch = pathPoint[a].GetComponent<Crossing>().nextPoint[Random.Range(0,2)];
                    pathPoint = new GameObject[Branch.GetComponent<WayStart>().wayNode.Length];
                    for (int i = 0; i < Branch.GetComponent<WayStart>().wayNode.Length; i++)
                    {
                        pathPoint[i] = Branch.GetComponent<WayStart>().wayNode[i];
                    }
                    a = 0;
                    currentPathPoint = pathPoint[a].transform.position;
                    arrive = false;
                }
                           
            }
            else if (a != pathPoint.Length - 1)
            {
                a++;
                currentPathPoint = pathPoint[a].transform.position;
                if(a!= pathPoint.Length - 1)
                {
                    nextPathPoint = pathPoint[a + 1].transform.position;
                }
                else
                {
                    nextPathPoint = currentPathPoint;
                }
                arrive = false;
            }
            
        }
        if (Vector3.Distance(transform.position, currentPathPoint) < 10f)
        {
            if (currentPathPoint != null)
            {
                Vector3 targetDirection = nextPathPoint - transform.position;
                angleBetweenPoint = (Mathf.Abs(Vector3.SignedAngle(targetDirection, -transform.forward, Vector3.up)));

                if (angleBetweenPoint > maxAngleToMoveBreak)
                {
                    moveBrake = true;
                }
            }
            
        }
        else
        {
            moveBrake = false;
        }
        if (!isACar)
        {
            Vector3 velocity = currentPathPoint - rigbody.transform.position;
            velocity.y = rigbody.velocity.y;
            rigbody.velocity = new Vector3(velocity.normalized.x * curMoveSpeed, velocity.y, velocity.normalized.z * curMoveSpeed);
        }
        if (!isACar)
        {
            Vector3 targetVector = targetPos - rigbody.transform.position;

            if (targetVector != Vector3.zero)
            {
                Quaternion look = Quaternion.identity;

                look = Quaternion.Lerp(rigbody.transform.rotation, Quaternion.LookRotation(targetVector),
                    Time.fixedDeltaTime * 4f);

                look.x = rigbody.transform.rotation.x;
                look.z = rigbody.transform.rotation.z;

                rigbody.transform.rotation = look;
            }
        }
    }

    public void InitializationCar()
    {
        
        gameObject.SetActive(false);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localRotation = Quaternion.identity;
        pathPoint = new GameObject[originMainRoad.wayNode.Length];
        for(int i = 0; i < originMainRoad.wayNode.Length; i++)
        {
            pathPoint[i] = originMainRoad.wayNode[i];
        }
        a = 0;
        health.health = 100;
        currentPathPoint = pathPoint[a].transform.position;
        nextPathPoint = pathPoint[a + 1].transform.position;
    }
    private void Drive()
    {
        CarWheels wheels = GetComponent<CarWheels>();
        if (tempStop)
        {
            if (hasTrailer)
            {
                curMoveSpeed = Mathf.Lerp(curMoveSpeed, 0.0f, Time.fixedDeltaTime * (speedDecrease * 2.5f));
            }
            else
            {
                curMoveSpeed = Mathf.Lerp(curMoveSpeed, 0, Time.fixedDeltaTime * speedDecrease);
            }

            if (curMoveSpeed < 0.15f)
            {
                curMoveSpeed = 0.0f;
            }
        }
        else 
        {
            curMoveSpeed = Mathf.Lerp(curMoveSpeed, moveSpeed, Time.fixedDeltaTime * speedIncrease);
        }

        //CarOverturned();

        for (int wheelIndex = 0; wheelIndex < wheels.WheelColliders.Length; wheelIndex++)
        {
            if (wheels.WheelColliders[wheelIndex].transform.localPosition.z > 0)
            {
                wheels.WheelColliders[wheelIndex].steerAngle = Mathf.Clamp(CarWheelsRotation.AngleSigned(transform.forward, pathPoint[a].transform.position - transform.position, transform.up), -30.0f, 30.0f);
                if (overtake)
                {
                    wheels.WheelColliders[wheelIndex].steerAngle = Mathf.Lerp(wheels.WheelColliders[wheelIndex].steerAngle,60,0.5f);
                }
                if (Mathf.Abs(wheels.WheelColliders[wheelIndex].steerAngle) >= 3f)
                {
                    turn = true;
                }
                else
                {
                    turn = false;
                }
            }
        }

        if (rigbody.velocity.magnitude > curMoveSpeed)
        {
            rigbody.velocity = rigbody.velocity.normalized * curMoveSpeed;
        }
    }

    private void CarOverturned()
    {
        WheelCollider[] wheels = GetComponent<CarWheels>().WheelColliders;

        bool removal = false;
        int number = wheels.Length;

        foreach (var item in wheels)
        {
            if (!item.isGrounded)
            {
                number--;
            }
        }

        if (number == 0)
        {
            removal = true;
        }

        if (removal)
        {
            upTurnTimer += Time.deltaTime;
        }
        else
        {
            upTurnTimer = 0;
        }

        if (upTurnTimer > 3)
        {
            Destroy(gameObject);
        }
    }
    private void PushRay()
    {
        RaycastHit hit;

        Ray fwdRay = new Ray(fwdVector, transform.forward * 10);
        Ray LRay = new Ray(LRVector - transform.right*0.5f, transform.forward * 10);
        Ray LRay2 = new Ray(LRVector - transform.right, transform.forward * 10);
        Ray RRay = new Ray(LRVector + transform.right*0.5f, transform.forward * 10);
        Ray RRay2 = new Ray(LRVector + transform.right, transform.forward * 10);
        //RaycastHit hit2;
        /*if(Physics.Raycast(fwdVector2,transform.forward,out hit2, 2f,collisionOb))
        {
            Debug.Log(41631);
            ob = hit2.collider;
        }
        */
        if (Physics.Raycast(fwdRay, out hit, 10) || Physics.Raycast(LRay, out hit, 10) || Physics.Raycast(LRay2, out hit, 10) || Physics.Raycast(RRay, out hit, 10)|| Physics.Raycast(RRay2, out hit, 10))
        {
            float distance = Vector3.Distance(fwdVector, hit.point);

            if (hit.transform.CompareTag("Car"))
            {
                GameObject car = (hit.transform.GetComponentInChildren<ParentOfTrailer>()) ? hit.transform.GetComponent<ParentOfTrailer>().PAR : hit.transform.gameObject;

                if (car != null)
                {
                    MovePath MP = car.GetComponent<MovePath>();

                    ReasonsStoppingCars.CarInView(car, rigbody, distance, startSpeed, ref moveSpeed, ref tempStop, distanceToCar);
                }
            }
            else if (hit.transform.CompareTag("Bcycle"))
            {
                ReasonsStoppingCars.BcycleGyroInView(hit.transform.GetComponentInChildren<BcycleGyroController>(), rigbody, distance, startSpeed, ref moveSpeed, ref tempStop);
            }
            else if (hit.transform.CompareTag("PeopleSemaphore"))
            {

                ReasonsStoppingCars.SemaphoreInView(hit.transform.GetComponent<TrafficLight>(), distance, startSpeed, insideSemaphore, ref moveSpeed, ref tempStop, distanceToSemaphore);
            }
            else if (hit.transform.CompareTag("Player") || hit.transform.CompareTag("People"))
            {
                ReasonsStoppingCars.PlayerInView(hit.transform, distance, startSpeed, ref moveSpeed, ref tempStop);
            }
            else
            {
                if (!moveBrake)
                {
                    moveSpeed = startSpeed;
                }
                overtake = false;
                tempStop = false;
            }
        }
        else
        {
            if (!moveBrake)
            {
                moveSpeed = startSpeed;
            }

            tempStop = false;

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (bc != null)
        {
            Gizmos.DrawRay(new Vector3(transform.position.x + transform.forward.x * bc.size.z / 2, transform.position.y + 0.5f, transform.position.z + transform.forward.z * bc.size.z / 2), transform.forward * 10);
            Gizmos.DrawRay(new Vector3(transform.position.x + transform.forward.x * bc.size.z / 2, transform.position.y + 0.5f, transform.position.z + transform.forward.z * bc.size.z / 2) + transform.right, transform.forward * 10);
            Gizmos.DrawRay(new Vector3(transform.position.x + transform.forward.x * bc.size.z / 2, transform.position.y + 0.5f, transform.position.z + transform.forward.z * bc.size.z / 2) + transform.right*0.5f, transform.forward * 10);
            Gizmos.DrawRay(new Vector3(transform.position.x + transform.forward.x * bc.size.z / 2, transform.position.y + 0.5f, transform.position.z + transform.forward.z * bc.size.z / 2) - transform.right, transform.forward * 10);
            Gizmos.DrawRay(new Vector3(transform.position.x + transform.forward.x * bc.size.z / 2, transform.position.y + 0.5f, transform.position.z + transform.forward.z * bc.size.z / 2) - transform.right*0.5f, transform.forward * 10);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Human"))
        {
            Animator humanAnimator = other.GetComponentInParent<Animator>();
            PedestrianAI pedestrian = other.GetComponentInParent<PedestrianAI>();
            StairDismount stairDismount = other.gameObject.GetComponentInParent<StairDismount>();
            stairDismount.startEffect = true;
            stairDismount.impactTarget = other.attachedRigidbody;
            stairDismount.impact = (other.transform.position - transform.position).normalized;
            stairDismount.power = 5f;

        }
    }
}
