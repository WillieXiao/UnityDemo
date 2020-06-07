using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateHitted : MonoBehaviour
{
    public bool isPeople,isArmed,isBoss,isPlayer;
    public HittedPart head, chest, hips,leftUpLeg,rightUpLeg,leftLeg,rightLeg,leftArm,rightArm,leftForceArm,rightForceArm;
    Animator animator;
    PedestrianAI controller;
    ArmedAI armed;
    BossAI boss;
    PlayerController player;
    FightSystem fightSystem;
    StairDismount stairDismount;
    RagdollHelper ragdoll;
    public Vector3 attackPosition;
    public bool kO;
    public bool hurt;
    public HittedPart currentHitted;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (isPeople)
        {
            controller = GetComponent<PedestrianAI>();
        }
        if (isArmed)
        {
            armed = GetComponent<ArmedAI>();
        }
        if (isBoss)
        {
            boss = GetComponent<BossAI>();
        }
        if (isPlayer)
        {
            player = GetComponent<PlayerController>();
            fightSystem = GetComponent<FightSystem>();
        }
        stairDismount = GetComponent<StairDismount>();
        ragdoll = GetComponent<RagdollHelper>();
        
    }

    // Update is called once per frame
    void Update()
    {             


        if (isArmed)
        {
            if (armed.state == ArmedState.Fight)
            {
                if (head.hitted || chest.hitted || leftArm.hitted || rightArm.hitted)
                {
                    Vector3 dir = (attackPosition - transform.position);
                    if (!kO)
                    {
                        Debug.Log(123131);
                        if (Vector3.Cross(transform.forward, dir).y > 0)
                        {
                            animator.SetTrigger("HitHighLeft");
                            if(currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }                           
                        }
                        else
                        {
                            animator.SetTrigger("HitHighRight");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }
                    }
                }
                else if (hips.hitted || leftUpLeg.hitted || rightUpLeg.hitted || leftForceArm.hitted || rightForceArm.hitted)
                {
                    Debug.Log(123131);
                    Vector3 dir = (attackPosition - transform.position);
                    if (!kO)
                    {
                        animator.SetTrigger("HitMiddle");
                        if (currentHitted != null)
                        {
                            currentHitted.hitted = false;
                        }
                    }
                }
                else if (leftLeg.hitted || rightLeg.hitted)
                {
                    Vector3 dir = (attackPosition - transform.position);
                    if (!kO)
                    {
                        Debug.Log(123131);
                        if (Vector3.Cross(transform.forward, dir).y > 0)
                        {
                            animator.SetTrigger("HitHighLeft");
                            currentHitted.hitted = false;
                        }
                        else
                        {
                            animator.SetTrigger("HitHighRight");
                            currentHitted.hitted = false;
                        }
                    }
                }
            }
        }




        if (isBoss)
        {
            if (boss.state == BossState.Fight)
            {
                if (head.hitted || chest.hitted || leftArm.hitted || rightArm.hitted)
                {
                    
                    Vector3 dir = (attackPosition - transform.position);
                    if (!kO&&!boss.noDown)
                    {
                        if (Vector3.Cross(transform.forward, dir).y > 0)
                        {
                            animator.SetTrigger("HitHighLeft");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }
                        else
                        {
                            animator.SetTrigger("HitHighRight");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }
                    }
                }
                else if (hips.hitted || leftUpLeg.hitted || rightUpLeg.hitted || leftForceArm.hitted || rightForceArm.hitted)
                {
                    Vector3 dir = (attackPosition - transform.position);
                    if (!kO && !boss.noDown)
                    {
                        if (Vector3.Cross(transform.forward, dir).y > 0)
                        {
                            animator.SetTrigger("HitMiddleLeft");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }
                        else
                        {
                            animator.SetTrigger("HitMiddleRight");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }                       
                    }
                }
                else if (leftLeg.hitted || rightLeg.hitted)
                {
                    Vector3 dir = (attackPosition - transform.position);
                    if (!kO && !boss.noDown)
                    {
                        if (Vector3.Cross(transform.forward, dir).y > 0)
                        {
                            animator.SetTrigger("HitLowLeft");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }
                        else
                        {
                            animator.SetTrigger("HitLowRight");
                            if (currentHitted != null)
                            {
                                currentHitted.hitted = false;
                            }
                        }                       
                    }
                }
            }
        }


        if (isPlayer)
        {
            if (player.fightSystem.fighting)
            {
                if (fightSystem.block == false)
                {
                    if (head.hitted || chest.hitted || leftArm.hitted || rightArm.hitted)
                    {
                        Vector3 dir = (attackPosition - transform.position);
                        if (!kO)
                        {
                            if (Vector3.Cross(transform.forward, dir).y > 0)
                            {
                                animator.Play("GetHitRight", 0);
                                player.controller.Move(-transform.forward * 20f * Time.deltaTime);
                                if (currentHitted != null)
                                {
                                    currentHitted.hitted = false;
                                }
                            }
                            else
                            {
                                animator.Play("GetHitLeft", 0);
                                player.controller.Move(-transform.forward * 20f * Time.deltaTime);
                                if (currentHitted != null)
                                {
                                    currentHitted.hitted = false;
                                }
                            }
                        }
                    }
                    else if (hips.hitted || leftUpLeg.hitted || rightUpLeg.hitted || leftForceArm.hitted || rightForceArm.hitted)
                    {
                        Vector3 dir = (attackPosition - transform.position);
                        if (!kO)
                        {
                            if (Vector3.Cross(transform.forward, dir).y > 0)
                            {
                                animator.Play("GetHitRight", 0);
                                player.controller.Move(-transform.forward * 20f * Time.deltaTime);
                                if (currentHitted != null)
                                {
                                    currentHitted.hitted = false;
                                }
                            }
                            else
                            {
                                animator.Play("GetHitLeft", 0);
                                player.controller.Move(-transform.forward * 20f * Time.deltaTime);
                                if (currentHitted != null)
                                {
                                    currentHitted.hitted = false;
                                }
                            }
                        }
                    }
                    else if (leftLeg.hitted || rightLeg.hitted)
                    {
                        Vector3 dir = (attackPosition - transform.position);
                        if (!kO)
                        {
                            if (Vector3.Cross(transform.forward, dir).y > 0)
                            {
                                animator.Play("GetHitRight", 0);
                                player.controller.Move(-transform.forward * 20f * Time.deltaTime);
                                if (currentHitted != null)
                                {
                                    currentHitted.hitted = false;
                                }
                            }
                            else
                            {
                                animator.Play("GetHitLeft", 0);
                                player.controller.Move(-transform.forward * 20f * Time.deltaTime);
                                if (currentHitted != null)
                                {
                                    currentHitted.hitted = false;
                                }
                            }
                        }
                    }
                }
            }
           
        }
    }

}
