using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Patrol,
    Chase,
    Fight,
}
public class BossAI : MonoBehaviour
{
    public BossState state;
    private AnimatorStateInfo stateInfo;
    CharacterController controller;
    Animator animator;
    HealthScript health;
    [HideInInspector]
    public CameraShake cameraShake;

    public PlayerController player;
    public GameObject VFXObjectPool;
    public GameObject landPoint;
    public GameObject sprayPoint;
    public GameObject footPoint;
    public GameObject footPoint2;
    public GameObject getBlockPoint;
    public GameObject footAttackVFXPoing;
    public GameObject emissionPoint;
    public GameObject laser;
    public GameObject attackCollisionPoint;

    private int a = 2;
    public int attackTimes = 3;
    public int hitedTimes = 3;

    public float currentSpeed = 0f;
    float speedSmoothVelocity;
    public float speedSmoothTime = 0.5f;
    float turnSmoothVelocity;
    public float turnSmoothTime = 0.2f;
    public float gravity;
    public float velocityY;
    public float walkSpeed;
    public float runSpeed = 12;
    float animationSpeedPercent;
    public float afterAttackTime = 0f;
    float withPlayerDis;
    public float distToGround;
    public float dropPoint;
    public float jumpHeight;
    private float noDownTimer = 5f;
    public float observedTime = 3f;

    public bool StartObserved;
    public bool onGround;
    public bool flying;
    public bool walking;
    public bool attackEnd = true;
    public bool attacking = true;
    public bool jump;
    public bool jumping;
    public bool jumped;
    public bool landing;
    public bool fallDown;
    public bool heavyAttack;
    public bool track = true;
    public bool getBlock;
    public bool noDown;
    public bool oneState;
    public bool twoState;
    public bool threeState;
    public bool Shouted = false;
    public bool dead;
    public bool deading;

    private Vector2 Dir;
    

    public LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {       
        attackEnd = true;
        track = true;
        state = BossState.Patrol;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        health = GetComponent<HealthScript>();
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position+new Vector3(0f,0.1f,0f), -Vector3.up, distToGround + 0.2f,layer);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            deading = true;
            animator.SetTrigger("Dead");
            dead = false;
        }
        if (!deading)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            withPlayerDis = Vector3.Distance(player.transform.position, transform.position);
            Dir = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.z - transform.position.z).normalized;
            if ((controller.enabled) && track)
            {
                float targetRotation = Mathf.Atan2(Dir.x, Dir.y) * Mathf.Rad2Deg;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }
            float targetSpeed = ((walking) ? walkSpeed : runSpeed) * Dir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
            Vector3 velocity = transform.forward * currentSpeed + transform.up * velocityY;
            if (state == BossState.Patrol)
            {
                Patrol();
            }
            else if (state == BossState.Chase)
            {
                Chase();
            }
            else if (state == BossState.Fight)
            {
                Fight();
            }
            gravity = -30;
            velocityY += Time.deltaTime * gravity;
            if (stateInfo.IsTag("Attack"))
            {
                velocity = transform.forward * 0 + transform.up * velocityY;
                controller.Move(velocity * Time.deltaTime);
            }
            else if (withPlayerDis <= 2f)
            {
                velocity = transform.forward * 0 + transform.up * velocityY;
                controller.Move(velocity * Time.deltaTime);
            }
            else if (stateInfo.IsTag("Right") || stateInfo.IsTag("Left"))
            {
                velocity = transform.forward * 0 + transform.up * velocityY;
                controller.Move(velocity * Time.deltaTime);
            }
            else
            {
                controller.Move(velocity * Time.deltaTime);
            }
            if (controller.isGrounded)
            {
                animator.SetBool("Grounded", true);
                onGround = true;
                gravity = -30;
                velocityY = 0f;
            }
            else
            {
                onGround = false;
                animator.SetBool("Grounded", false);
            }
            currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
            animationSpeedPercent = ((walking) ? currentSpeed / walkSpeed * 0.5f : currentSpeed / runSpeed);
            animator.SetFloat("Speed", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
            LandingDelegate();
            DelegateHealth();
            if (noDown)
            {
                animator.ResetTrigger("HitHighRight");
                animator.ResetTrigger("HitHighLeft");
                animator.ResetTrigger("HitMiddleRight");
                animator.ResetTrigger("HitMiddleLeft");
                animator.ResetTrigger("HitLowRight");
                animator.ResetTrigger("HitLowLeft");
            }
        }       
    }

    private void Patrol()
    {
        if (withPlayerDis <= 10f)
        {
            state = BossState.Fight;
        }
             
    }
    
    private void Fight()
    {
        
        if (withPlayerDis <= 1f)
        {
            observedTime = 0;
            Debug.Log("觀察時間結束");
        }

        if (observedTime > 0)
        {
            Debug.Log("進入觀察狀態");
            walking = true;            
            observedTime -= Time.deltaTime;
            if (!StartObserved)
            {
                int x = Random.Range(0, 3);
                switch (x)
                {
                    case 0:
                        animator.SetBool("WalkRight",true);
                        animator.SetBool("WalkLeft", false);
                        break;
                    case 1:
                        animator.SetBool("WalkRight",false);
                        animator.SetBool("WalkLeft", true);
                        break;
                    case 2:
                        animator.SetBool("WalkRight", false);
                        animator.SetBool("WalkLeft", false);
                        break;
                }
                StartObserved = true;
                afterAttackTime = 0;
            }
        }
        else
        {
            animator.SetBool("WalkRight", false);
            animator.SetBool("WalkLeft", false);           
            Debug.Log("進入攻擊狀態");
            NormalAttack();
        }
        
        
    }

    private void NormalAttack()
    {
        if(attackTimes > 0)
        {
            if (withPlayerDis <= 3f)
            {
                if (attackEnd == true)
                {
                    afterAttackTime -= Time.deltaTime;
                }
                if (afterAttackTime <= 0)
                {
                    attacking = true;
                    afterAttackTime = 0.5f;
                }
                if (attacking)
                {
                    attackTimes--;
                    attackEnd = false;
                    track = false;
                    int x = Random.Range(0, 3);
                    switch (x)
                    {
                        case 0:
                            animator.SetTrigger("AttackOne");
                            break;
                        case 1:
                            animator.SetTrigger("AttackTwo");
                            break;
                        case 2:
                            animator.SetTrigger("AttackThree");
                            break;
                    }                   
                    attacking = false;
                }
            }
            else if(withPlayerDis<=8f&&withPlayerDis>3f)
            {
                if (attackEnd == true)
                {
                    afterAttackTime -= Time.deltaTime;
                }
                if (afterAttackTime <= 0)
                {
                    attacking = true;
                    afterAttackTime = 0.5f;
                }
                if (attacking)
                {
                    attackTimes--;
                    attackEnd = false;
                    track = false;
                    int x = Random.Range(0, 2);
                    switch (x)
                    {
                        case 0:
                            animator.SetTrigger("MiddleAttack");
                            break;
                        case 1:
                            animator.SetTrigger("MiddleAttackTwo");
                            break;
                    }                    
                    attacking = false;
                }
            }
            else if(withPlayerDis>8f&&withPlayerDis<=15f)
            {
                if (attackEnd == true)
                {
                    afterAttackTime -= Time.deltaTime;
                }
                if (afterAttackTime <= 0)
                {
                    attacking = true;
                    afterAttackTime = 0.5f;
                }
                if (attacking)
                {
                    attackTimes--;
                    attackEnd = false;
                    Debug.Log("跳躍攻擊");
                    animator.SetTrigger("JumpAttack");
                    attacking = false;                                 
                }
            }
            else if (withPlayerDis > 15f)
            {
                track = true;
                Debug.Log("衝刺攻擊");
                state = BossState.Chase;
            }
        }
        else
        {
            StartObserved = false;
            if (oneState)
            {
                observedTime = 5f;
            }
            else
            {
                observedTime = 2f;
            }
            attackTimes = 3;
        }
        
    }
    
    public void Chase()
    {
        walking = false;            
        if (withPlayerDis<=3f)
        {
            attackEnd = false;
            track = false;
            attackTimes--;
            animator.SetTrigger("SprintAttack");
            attacking = false;
            state = BossState.Fight;
        }
    }

    public void LaserAttack()
    {
        if (attackEnd == true)
        {
            afterAttackTime -= Time.deltaTime;
        }
        if (afterAttackTime <= 0)
        {
            attacking = true;
            if(oneState == true)
            {
                afterAttackTime = 0.5f;
            }
        }
        if (attacking)
        {
            attackEnd = false;
            heavyAttack = true;
            animator.SetTrigger("LaserAttack");
            attacking = false;
        }        
    }

    public void AttackEnd()
    {
        attackEnd = true;
        track = true;
        heavyAttack = false;
    }
    public void AttackMove(float dis)
    {
        controller.Move(transform.forward * dis*Time.deltaTime);
    }
    public void SprintAttackMove()
    {        
        VFXObjectPool.transform.GetChild(17).transform.position = sprayPoint.transform.position;
        VFXObjectPool.transform.GetChild(17).transform.rotation = sprayPoint.transform.rotation;
        VFXObjectPool.transform.GetChild(17).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        controller.Move(transform.forward * 10f);
        track = false;
    }
    public void JumpForce(float jumpheight)
    {
        velocityY = jumpheight;
    }
    public void AfterJump()
    {
        jump = false;
        animator.SetBool("Jump", false);

    }

    public void StopTrackPlayer()
    {
        track = false;
    }

    public void LandingDelegate()
    {
        if (jump)
        {
            dropPoint = 8f;
            if ((dropPoint - transform.position.y) < 0)
            {
                dropPoint = transform.position.y;
            }
        }
        else if (!jumping && !IsGrounded() && !flying && dropPoint == 0)
        {
            dropPoint = transform.position.y;
        }
    }

    public void LandingOn()
    {
        landing = true;
        float landingPoint = transform.position.y;
        if (dropPoint - landingPoint >= 5 && dropPoint - landingPoint <= 15)
        {
            cameraShake.StartShake(0.2f, 0.5f);
            VFXObjectPool.transform.GetChild(1).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(1).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        else if (dropPoint - landingPoint > 15)
        {
            cameraShake.StartShake(0.5f, 1f);
            VFXObjectPool.transform.GetChild(0).transform.position = landPoint.transform.position;
            VFXObjectPool.transform.GetChild(0).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        dropPoint = 0;
        jumpHeight = 0;
        jumping = false;

    }
    public void LandingOff()
    {
        if (jumpHeight >= 5 && jumpHeight < 10)
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


    public void FootDecalLeft()
    {
        if (walking == false)
        {
            FootDecalVFXleft();
            //Instantiate(footDecal, footPoint.transform.position, Quaternion.identity);
        }
        cameraShake.StartShake(0.1f, 0.1f);
    }
    public void FootDecalRight()
    {
        if (walking == false)
        {
            FootDecalVFXright();
        }
        cameraShake.StartShake(0.1f, 0.1f);
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
    public void DelegateHealth()
    {
        if (hitedTimes <= 0)
        {
            noDown = true;
            noDownTimer -= Time.deltaTime;
        }
        if (noDownTimer <= 0)
        {
            noDown = false;
            noDownTimer = 10f;
            hitedTimes = 3;
        }


        if (health.health > 50)
        {
            oneState = true;
            twoState = false;            
        }
        else if (health.health <= 50)
        {
            oneState = false;
            twoState = true;
            if (Shouted == false)
            {
                animator.SetTrigger("Shout");
                Shouted = true;
                attackEnd = true;
                track = true;
                observedTime = 1f;
                attackTimes = 3;
            }            
        }
        else if (health.health <= 10)
        {

        }
        
    }
    public void HeavyAttackOn()
    {
        if(twoState == true)
        {
            heavyAttack = true;
        }       
    }
    public void AttackShake()
    {
        cameraShake.StartShake(0.2f, 0.5f);
    }
    public void ShoutShake()
    {
        cameraShake.StartShake(1.8f, 0.5f);
    }
    public void FootAttackVFX()
    {
        if(twoState == true)
        {
            VFXObjectPool.transform.GetChild(19).GetComponent<VFXObjectRecover>().attackUniversal.SetActive(true);
            VFXObjectPool.transform.GetChild(19).GetComponentInChildren<AttackUniversal>().user = gameObject;
            VFXObjectPool.transform.GetChild(19).transform.position = footAttackVFXPoing.transform.position;
            VFXObjectPool.transform.GetChild(19).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }      
    }
    public void AttackCollisionVFX()
    {
        VFXObjectPool.transform.GetChild(1).transform.position = attackCollisionPoint.transform.position;
        VFXObjectPool.transform.GetChild(1).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
    }
    void AttackEmission()
    {
        Quaternion q = Quaternion.LookRotation((player.transform.position+Vector3.up*0.5f) - transform.position);
        emissionPoint.transform.rotation = q;
        VFXObjectPool.transform.GetChild(17).transform.position = emissionPoint.transform.position;
        VFXObjectPool.transform.GetChild(17).transform.rotation = Quaternion.Euler(-90f, emissionPoint.transform.eulerAngles.y, emissionPoint.transform.eulerAngles.z);
        VFXObjectPool.transform.GetChild(17).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        GameObject emissionObject = Instantiate(laser, emissionPoint.transform.position, emissionPoint.transform.rotation);
        emissionObject.GetComponent<ProjectileMoveScript>().user = gameObject;
    }

}
