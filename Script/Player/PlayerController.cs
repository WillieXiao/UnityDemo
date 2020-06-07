using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public Collider[] hits;
    public LayerMask people;
    public LayerMask Enemy;
    private AnimatorClipInfo[] clips;
    private Animator animator;
    [HideInInspector]
    public CharacterController controller;
    private Rigidbody rb;
    [HideInInspector]
    public FightSystem fightSystem;
    private PlayerSound playerSound;


    private int id;
    private int targetIndex = 0;

    private float dis;
    private float dis_min = 3;
    public float runSpeed = 3;
    public float walkSpeed = 0.5f;
    public float currentSpeed =0f;
    float speedSmoothVelocity;
    public float speedSmoothTime=0.5f;
    float turnSmoothVelocity;
    public float turnSmoothTime=0.2f;
    [HideInInspector]
    public float fovStretchSmoothVelocity;
    public float fovStretchSmoothTime = 0.2f;
    private float animationSpeedPercent;
    public float gravity = -12;
    public float jumpHeight;
    public float velocityY;
    private int max_Jump = 1;
    private int currentJump = 0;
    private float Selection_status =1;
    private int a = 2;
    //計時器
    private float runTimer;
    public float jumpAccumulateTimer;
    private float jumpAccumulate;
    private float fightTimer;
    private float default_Combo_Timer = 1.8f;
    private float current_Combo_Timer;
    public float timer = 0;
    //布林邏輯
    private bool lockOnInput;
    public bool jump = false;
    public bool run;
    public bool walking;
    public bool fly;
    public bool flying;
    public bool flyUp;
    public bool flyDown;
    public bool flyFallDown;   
    public bool move;
    public bool onGround;
    public bool jumping;
    public bool lifting;
    public bool gliding = false;
    public bool landing;
    public bool accumulate;
    public bool fighting;
    public bool punch_State;
    public bool sword_State;  
    public bool airAccelerating;
    bool firstSegment;
    bool secondSegment;
    private bool activateTimerToReset;
    bool fightRun;
    public bool lockEnemy;
    public bool locking;
    public bool fallDown;
    public bool rescue;
    public bool dead;
    public bool deading;

    private Fight_State fight_State;
    [HideInInspector]
    public CameraShake cameraShake;

    private new ThirdPersonCamera camera;
    [HideInInspector]
    public Camera trueCamera;   

    public GameObject characterModel;
    public GameObject landPoint;
    public GameObject sprayPoint;  
    public GameObject footPoint;
    public GameObject footPoint2;
    public GameObject wrapPoint;
    public GameObject wrapAirPoint;
    public GameObject VFXObjectPool;
    public GameObject[] withEnemyMeele;
    public GameObject[] flyTrail;
    public GameObject[] aroundTarget;

    private CapsuleCollider collider;

    private float moveSpeed = 6;
    private float turnSpeed = 90;
    private float lerpSpeed = 10;
    public bool isGrounded;
    private float deltaGround = 0.2f;
    private float jumpSpeed = 10;
    private float jumpRange = 10;
    public float dropPoint;
    private float distGround;
    private float vertSpeed = 0;

    private Vector2 mouseRotation;
    private Vector3 surfaceNormal; 
    private Vector3 myNormal;
    private Vector2 input;
    private Vector2 inputDir;

    public LayerMask BigOB;
    public LayerMask normalTerrain;
    private AnimatorStateInfo stateInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();             
        collider = GetComponent<CapsuleCollider>();
        trueCamera = Camera.main.GetComponent<Camera>();
        camera = GameObject.FindGameObjectWithTag("Camera").GetComponent<ThirdPersonCamera>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        fightSystem = GetComponent<FightSystem>();
        playerSound = GetComponent<PlayerSound>();
        fight_State = Fight_State.Punch;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

    }
    public void Update()
    {
        if (dead)
        {
            deading = true;
            animator.SetTrigger("Dead");
            dead = false;
        }

        if (!deading)
        {
            DetectionEnemy();
            LockEnemy();
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (fallDown == false && !stateInfo.IsTag("GetUp"))
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                inputDir = input.normalized;
                //判斷是否有在移動
                if (input != Vector2.zero)
                {
                    move = true;
                }
                else
                {
                    move = false;
                }
                animator.SetBool("Move", move);

                if (Input.GetKeyDown(KeyCode.R))
                {
                    Selection_status++;

                    if (Selection_status > 2)
                    {
                        Selection_status = 1;
                    }
                }

                //角色旋轉相關
                if (move && !stateInfo.IsTag("Combat") && controller.enabled && !fightSystem.aiming)
                {

                    float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camera.gameObject.transform.eulerAngles.y;
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

                    if (flyUp)
                    {
                        Quaternion q = Quaternion.Euler(-50, 0, 0);
                        characterModel.transform.localRotation = Quaternion.Slerp(characterModel.transform.localRotation, q, 0.1f);
                    }
                    else if (flyDown)
                    {
                        Quaternion q = Quaternion.Euler(50, 0, 0);
                        characterModel.transform.localRotation = Quaternion.Slerp(characterModel.transform.localRotation, q, 0.1f);
                    }
                }
                else if (fightSystem.aiming)
                {
                    float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camera.gameObject.transform.eulerAngles.y;
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
                }
                if (!move)
                {
                    if (camera.lockOn)
                    {
                        Vector3 targetDir = camera.lockonTransform.value.position - transform.position;
                        targetDir.y = 0;

                        Quaternion tr = Quaternion.LookRotation(targetDir);
                        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, 0.3f);
                        transform.rotation = targetRotation;
                    }
                }
                if (!flyUp && !flyDown)
                {
                    Quaternion q = Quaternion.Euler(0, 0, 0);
                    characterModel.transform.localRotation = Quaternion.Slerp(characterModel.transform.localRotation, q, 0.1f);
                }
                //若在靜止中 衝刺時的速度與狀態恢復預設
                if (!move)
                {
                    if (flying)
                    {
                        airAccelerating = false;
                        runSpeed = 12;
                    }
                    else
                    {
                        airAccelerating = false;
                        runSpeed = 5;
                    }
                }

                if (animationSpeedPercent != 0)
                {
                    mouseRotation = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                    mouseRotation = mouseRotation.normalized;
                }
                else
                {
                    mouseRotation = Vector3.zero;
                }
                animator.SetFloat("Direction", mouseRotation.x, 0.5f, Time.deltaTime);
                //跑步
                if (controller.isGrounded)
                {
                    walking = Input.GetKey(KeyCode.LeftAlt);
                }
                float targetSpeed = ((walking) ? walkSpeed : runSpeed) * inputDir.magnitude;

                currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

                //重力    
                if (!flying)
                {
                    gravity = -20;
                    velocityY += Time.deltaTime * gravity;
                }
                Vector3 velocity = transform.forward * currentSpeed + transform.up * velocityY;
                if (stateInfo.IsTag("Hited") || stateInfo.IsTag("Combat") || stateInfo.IsTag("Block"))
                {
                    velocity = transform.forward * 0 + transform.up * velocityY;
                    controller.Move(velocity * Time.deltaTime);
                }
                else
                {
                    controller.Move(velocity * Time.deltaTime);
                }

                currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

                //重新回到地面上 重力歸0
                if (flying)
                {
                    timer = 0;
                }
                if (controller.isGrounded)
                {
                    onGround = true;
                    timer = 0;
                    gravity = -20;
                    animator.SetFloat("AnimationSpeed", 1f);
                    animator.SetBool("Grounded", true);
                    velocityY = 0;
                    flyFallDown = false;
                }
                else
                {
                    onGround = false;
                    timer += Time.deltaTime;
                    animator.SetBool("Grounded", false);
                }
                //判斷從高處掉落
                if (timer > 3f)
                {
                    animator.SetBool("FromHighFall", true);
                }
                else if (timer < 3f)
                {
                    animator.SetBool("FromHighFall", false);
                }

                animationSpeedPercent = ((walking) ? currentSpeed / walkSpeed * 0.5f : currentSpeed / runSpeed);
                animator.SetFloat("Speed", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
                GetInput();
                //攻擊和落地中不能移動
                if (stateInfo.IsTag("Combat"))
                {
                    currentSpeed = 0;
                }
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit, 0.5f, people))
                {
                    currentSpeed = 0;
                    animator.SetFloat("Speed", 0);
                }
                LandingDelegate();
            }

        }
    }
        
     

   




    public void AfterJump()
    {
        jump = false;
        animator.SetBool("Jump", false);

    }
    public void AfterFly()
    {
        fly = false;
        animator.SetBool("Fly", false);
    }

    public void GetInput()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            lockEnemy = !lockEnemy;
            locking = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SelectLock();
        }
        
        //跳躍
        animator.SetFloat("JumpAccumulateTime", jumpAccumulateTimer);
        if (Input.GetKey(KeyCode.Space)&&!jumping&&!flying&&controller.isGrounded)
        {
            jumpAccumulateTimer += Time.deltaTime;           
        }
        if (jumpAccumulateTimer > 0.3f && controller.isGrounded)
        {
            //特效物件池
            VFXObjectPool.transform.GetChild(18).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(18).transform.rotation = landPoint.transform.rotation;
            VFXObjectPool.transform.GetChild(18).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        if (jumpAccumulateTimer > 0.3f&&accumulate == false && controller.isGrounded)
        {
            
            accumulate = true;
            animator.SetBool("NoAccumulate", false);
            
            if (!move)
            {
                animator.SetTrigger("Accumulate");
            }          
        }
        if (jumpAccumulateTimer < 0.3f&& jumpAccumulateTimer > 0&& !Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {
            jumping = true;
            jump = true;
            animator.SetBool("NoAccumulate", true);
            animator.SetBool("Jump", true);
            jumpAccumulateTimer = 0;
            JumpForce(15f);

        }
        //跳躍鍵案住不放就會進入飛行狀態
        else if (jumpAccumulateTimer > 0.3f&& !Input.GetKey(KeyCode.Space) && controller.isGrounded)
        {
            cameraShake.StartShake(0.5f, 0.5f);
            playerSound.SpraySFX(0);
            playerSound.FlySFX();
            //特效物件池
            VFXObjectPool.transform.GetChild(16).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(16).transform.rotation = landPoint.transform.rotation;
            VFXObjectPool.transform.GetChild(16).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
            VFXObjectPool.transform.GetChild(1).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(1).transform.rotation = landPoint.transform.rotation;
            VFXObjectPool.transform.GetChild(1).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
            velocityY = 100;
            fly = true;
            flying = true;
            gravity = 0;           
            runSpeed = 12;
            animator.SetBool("Fly", fly);
            animator.SetBool("Flying", flying);
            jumpAccumulateTimer = 0;
            accumulate = false;

        }
        
        //飛行狀態
        if (!controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space)&&!flying)
            {
                jumping = false;
                fly = true;
                flying = true;
                velocityY = 0;
                gravity = 0;
                dropPoint = 0;
                runSpeed = 12;
                animator.SetBool("Fly", fly);
                animator.SetBool("Flying", flying);
                VFXObjectPool.transform.GetChild(11).transform.position = landPoint.transform.position;
                VFXObjectPool.transform.GetChild(11).transform.rotation = landPoint.transform.rotation;
                VFXObjectPool.transform.GetChild(11).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
            }
        }
        if (flying)
        {

            if (Input.GetKeyDown(KeyCode.LeftShift)&&!airAccelerating&&!fightSystem.aiming)
            {
                playerSound.SpraySFX(1);
                cameraShake.StartShake(0.5f, 0.5f);
                VFXObjectPool.transform.GetChild(16).transform.position = sprayPoint.transform.position;
                VFXObjectPool.transform.GetChild(16).transform.rotation = sprayPoint.transform.rotation;
                VFXObjectPool.transform.GetChild(16).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
                VFXObjectPool.transform.GetChild(17).transform.position = sprayPoint.transform.position;
                VFXObjectPool.transform.GetChild(17).transform.rotation = sprayPoint.transform.rotation;
                VFXObjectPool.transform.GetChild(17).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
                runSpeed = 80;
                airAccelerating = true;
            }
            if (Input.GetKey(KeyCode.E))
            {
                flyDown = true;
                flyUp = false;
                velocityY = velocityY-((airAccelerating)?10:1); 

            }
            else if(Input.GetKey(KeyCode.Q))
            {
                flyUp = true;
                flyDown = false;
                velocityY = velocityY+ ((airAccelerating) ? 10 : 1);
            }
            else
            {
                flyUp = false;
                flyDown = false;
                if (!fly||transform.position.y>=(transform.up*20f).y)
                {
                    velocityY = 0;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) &&stateInfo.IsTag("FlyIdle"))
            {
                fly = false;
                flying = false;
                gravity = -20;
                animator.SetBool("Fly", fly);
                animator.SetBool("Flying", flying);
            }
            if (!fly &&controller.isGrounded)
            {
                flying = false;
                airAccelerating = false;
                animator.SetBool("Flying", flying);
                runSpeed = 5;
            }
            if (airAccelerating)
            {
                for(int i = 0; i < flyTrail.Length; i++)
                {
                    if (!flyTrail[i].GetComponent<ParticleSystem>().isPlaying)
                    {
                        flyTrail[i].GetComponent<ParticleSystem>().Play();
                    }                    
                }
                trueCamera.fieldOfView = Mathf.SmoothDamp(trueCamera.fieldOfView, 100, ref fovStretchSmoothVelocity, fovStretchSmoothTime);
                VFXObjectPool.transform.GetChild(14).transform.position = wrapPoint.transform.position;
                VFXObjectPool.transform.GetChild(14).transform.rotation = wrapPoint.transform.rotation;
                VFXObjectPool.transform.GetChild(14).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
                VFXObjectPool.transform.GetChild(15).transform.position = wrapAirPoint.transform.position;
                VFXObjectPool.transform.GetChild(15).transform.rotation = wrapAirPoint.transform.rotation;
                VFXObjectPool.transform.GetChild(15).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
            }

            if (Input.GetMouseButtonDown(2))
            {
                animator.SetTrigger("Rescue");
                rescue = true;
            }

        }
        
        
        else
        {
            flyUp = false;
            flyDown = false;            
        }
        if (!airAccelerating&&!fightSystem.aiming)
        {
            trueCamera.fieldOfView = Mathf.SmoothDamp(trueCamera.fieldOfView, 60, ref fovStretchSmoothVelocity, fovStretchSmoothTime);
            for (int i = 0; i < flyTrail.Length; i++)
            {
                if (flyTrail[i].GetComponent<ParticleSystem>().isPlaying)
                {
                    flyTrail[i].GetComponent<ParticleSystem>().Stop();
                }
            }
        }


        //衝刺
        if (Input.GetKeyDown(KeyCode.LeftShift)&&controller.isGrounded)
        {
            animator.SetTrigger("Dash");
            trueCamera.fieldOfView = Mathf.SmoothDamp(trueCamera.fieldOfView, 100, ref fovStretchSmoothVelocity, fovStretchSmoothTime);
            cameraShake.StartShake(0.5f, 0.25f);
            playerSound.SpraySFX(2);
        }
        
        else if(controller.isGrounded)
        {
            animator.SetBool("FastRun", false);
            animator.SetBool("FastRun2State", false);          
            
            runTimer = 0;
            trueCamera.fieldOfView = Mathf.SmoothDamp(trueCamera.fieldOfView, 60, ref fovStretchSmoothVelocity, fovStretchSmoothTime);
        }  
    }

    
    //著陸相關
    public void LandingDelegate()
    {
        if (jump && !flying)
        {
            dropPoint = velocityY+transform.position.y;
        }
        else if (!jumping && !controller.isGrounded && !flying&&dropPoint == 0)
        {
            dropPoint = transform.position.y;
        }
    }

    public void LandingOn()
    {
        landing = true;
        float landingPoint = transform.position.y;
        if (Mathf.Abs(dropPoint - landingPoint)>=5&& Mathf.Abs(dropPoint - landingPoint) <= 15)
        {
            cameraShake.StartShake(0.2f, 0.5f);
            playerSound.LandSFX(0);
            VFXObjectPool.transform.GetChild(1).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(1).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        else if (Mathf.Abs(dropPoint - landingPoint )> 15)
        {
            cameraShake.StartShake(0.5f, 1f);
            playerSound.LandSFX(1);
            VFXObjectPool.transform.GetChild(0).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(0).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        dropPoint = 0;
        jumping = false;

    }
    public void LandingOff()
    {
        if (jumpHeight >= 5&& jumpHeight < 10)
        {
            VFXObjectPool.transform.GetChild(11).transform.position = transform.position;
            VFXObjectPool.transform.GetChild(11).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        if (jumpHeight >= 10)
        {
            VFXObjectPool.transform.GetChild(1).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(1).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();

            VFXObjectPool.transform.GetChild(11).transform.position = transform.position;
            VFXObjectPool.transform.GetChild(11).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        
        landing = false;
        
    }





    // 特效
    public void FootDecalLeft()
    {
        playerSound.FootStepSFX();
        if (stateInfo.IsName("FastRun")||stateInfo.IsName("FastRun2"))
        {
            FootDecalVFXleft();
            //Instantiate(footDecal, footPoint.transform.position, Quaternion.identity);
        }
    }
    public void FootDecalRight()
    {
        playerSound.FootStepSFX();
        if (stateInfo.IsName("FastRun") || stateInfo.IsName("FastRun2"))
        {
            FootDecalVFXright();
        }
    }
    public void PakourJumpVFX()
    {
        VFXObjectPool.transform.GetChild(10).transform.position = footPoint2.transform.position;
        VFXObjectPool.transform.GetChild(10).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();

    }

    public void FootDecalVFXleft()
    {
        VFXObjectPool.transform.GetChild(a).transform.position = footPoint.transform.position;
        VFXObjectPool.transform.GetChild(a).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        a++;
        if (a > 9)
        {
            a = 2;
        }
    }
    public void FootDecalVFXright()
    {
        VFXObjectPool.transform.GetChild(a).transform.position = footPoint2.transform.position;
        VFXObjectPool.transform.GetChild(a).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        a++;
        if (a > 9)
        {
            a = 2;
        }
    }





    public void JumpForce(float jumpheight)
    {

        velocityY = jumpheight;


    }
    //滑步
    public void Dash_()
    {
        animator.SetTrigger("Dash");

    }
    public void DashMove()
    {
        controller.Move(transform.forward*0.1f);
    } 
    //翻滾
    public void AnimationSpeed()
    {
        animator.SetFloat("AnimationSpeed", 0.5f);
    }
   

    void DetectionEnemy()
    {
        hits = Physics.OverlapSphere(transform.position, 30, Enemy);
        if (hits.Length > 0)
        {
            aroundTarget = new GameObject[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                if(hits[i].GetComponentInParent<HealthScript>().health > 0)
                {
                    aroundTarget[i] = hits[i].gameObject;
                }
            }

        }
        else
        {
            aroundTarget  = null;
        }
    }
    void LockEnemy()
    {
        
        if (lockEnemy==true)
        {
            if (locking==true)
            {
                if (aroundTarget!=null)
                {
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if(aroundTarget[i] == null)
                        {
                            continue;
                        }
                        dis = Vector3.Distance(transform.position, aroundTarget[i].transform.position);
                        if (dis < dis_min)
                        {
                            dis_min = dis;
                            id = i;
                        }
                    }
                    dis_min = 30;
                    if (aroundTarget[id] != null&&aroundTarget[id].GetComponentInParent<HealthScript>().health>0)
                    {
                        camera.lockOn = true;
                        camera.lockonTransform.value = aroundTarget[id].gameObject.transform;
                    }
                    else
                    {
                        lockEnemy = false;
                        camera.lockOn = false;
                        camera.lockonTransform.value = null;
                        return;
                    }
                }
            }            
            locking = false;
            if (camera.lockonTransform.value!=null&&camera.lockonTransform.value.GetComponentInParent<HealthScript>().health <= 0)
            {
                locking = true;
            }
        }
        else
        {
            camera.lockOn = false;
            camera.lockonTransform.value = null;
        }        
    }
    void SelectLock()
    {
        if (lockEnemy)
        {
            if (aroundTarget != null)
            {
                if (id >= aroundTarget.Length - 1)
                {
                    id = 0;
                }
                else
                {
                    id++;
                }
                if (aroundTarget[id] != null)
                {
                    camera.lockonTransform.value = aroundTarget[id].gameObject.transform;
                }
            }           
        }       
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Terrain")&&controller.velocity.magnitude>60f)
        {
            Debug.Log("高速墜落");
            cameraShake.StartShake(0.5f, 1f);
            playerSound.LandSFX(1);
            VFXObjectPool.transform.GetChild(0).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(0).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
            VFXObjectPool.transform.GetChild(20).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(20).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
    }
    
}
