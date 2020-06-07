using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
public enum Fight_State
{
    Punch,
    Sword,
}
public enum LightAttack
{
    None,
    into,
    LightAttackOne,
    LightAttackTwo,
    LightAttackThree,
    LightAttackFour,

}
public enum HeavyAttack
{
    None,
    into,
    HeavyAttackOne,
    HeavyAttackTwo,
    HeavyAttackThree,
    HeavyAttackFour,
}
public enum SLightAttack
{
    None,
    SpecialLightAttackOne,
    SpecialLightAttackTwo,
    SpecialLightAttackThree,
}
public enum SHeavyAttack
{
    None,
    SpecialHeavyAttackOne,
    SpecialHeavyAttackTwo,
    SpecialHeavyAttackThree,
}

public class FightSystem : MonoBehaviour
{
    PlayerController player;
    CharacterController controller;
    Animator animator;
    ThirdPersonCamera camera;
    BlockSystem blockSystem;
    public PostProcessingProfile processingProfile;
    ChromaticAberrationModel.Settings chromatic;
    private AnimatorStateInfo stateInfo;
    private Fight_State fight_State;
    //體術模式 一段
    public LightAttack current_LightPunch;
    private HeavyAttack current_HeavyPunch;
    //體術模式 二段
    private SLightAttack current_SLightPunch;
    public SHeavyAttack current_SHeavyPunch;

    public bool fighting;
    public bool punch_State;
    public bool sword_State;
    public bool firstSegment;
    public bool secondSegment;
    private bool activateTimerToReset;
    public bool heavyAttack;
    public bool comboAdd;
    public bool block;
    public bool noBlock;
    public bool perfectBlock, normalBlock;
    public bool dodge;
    public bool aiming;

    private float fightTimer;
    private float Selection_status = 1;
    private float default_Combo_Timer = 1.8f;
    private float current_Combo_Timer;


    public GameObject target;
    public GameObject attackVFXPoing;
    public GameObject blockVFXPoint;
    public GameObject emissionPoint;
    public GameObject chargePoint;
    public GameObject laser;
    public GameObject look;

    // Start is called before the first frame update
    void Start()
    {
        comboAdd = true;
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        camera = GameObject.FindGameObjectWithTag("Camera").GetComponent<ThirdPersonCamera>();
        blockSystem = GetComponent<BlockSystem>();
    }

    // Update is called once per frame
    void Update()
    {

        FightInput();
        FightState();
        ResetComboState();
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        animator.SetBool("Fighting", fighting);
        //戰鬥
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Fighting();
        }
        if (player.flying)
        {
            AirAttack();
        }
        PunchComboAttack();
        if (stateInfo.IsTag("Dodge") || block)
        {
            animator.ResetTrigger("HitHighRight");
            animator.ResetTrigger("HitHighLeft");
            animator.ResetTrigger("HitMiddleRight");
            animator.ResetTrigger("HitMiddleLeft");
            animator.ResetTrigger("HitLowRight");
            animator.ResetTrigger("HitLowLeft");

            animator.ResetTrigger("PunchOne");
            animator.ResetTrigger("PunchTwo");
            animator.ResetTrigger("PunchThree");
            animator.ResetTrigger("KickOne");
            animator.ResetTrigger("KickTwo");
            animator.ResetTrigger("KickThree");
            comboAdd = true;
        }


    }

    void Fighting()
    {

        fightTimer = 0;
        fighting = true;

    }

    void FightState()
    {
        switch (Selection_status)
        {
            case 1:
                fight_State = Fight_State.Punch;
                break;
            case 2:
                fight_State = Fight_State.Sword;
                break;
        }

        if (fight_State == Fight_State.Punch && fighting == true)
        {
            punch_State = true;
            sword_State = false;

            animator.SetBool("Punch_State", punch_State);
            animator.SetBool("Sword_State", sword_State);

        }

        if (fight_State == Fight_State.Sword && fighting == true)
        {
            punch_State = false;
            sword_State = true;

            animator.SetBool("Punch_State", punch_State);
            animator.SetBool("Sword_State", sword_State);

        }


        if (fighting == true)
        {
            fightTimer += Time.deltaTime;




            /*if (fightTimer >= 6)
            {
                fighting = false;

                punch_State = false;
                sword_State = false;

                animator.SetBool("Fighting", fighting);
                animator.SetBool("Punch_State", punch_State);
                animator.SetBool("Sword_State", sword_State);

                fightTimer = 0;


            }*/


        }
    }


    void FightInput()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            animator.SetTrigger("Dodge");
        }

        if (Input.GetKey(KeyCode.F))
        {
            block = true;
            PerfectBlockOn();
            animator.SetBool("Block", true);
        }
        else
        {
            animator.SetBool("Block", false);
        }

        if (Input.GetMouseButtonDown(2) && fighting&&blockSystem.buff)
        {
            animator.SetTrigger("SuperAttack");
            blockSystem.buff = false;
        }
        if (Input.GetMouseButtonDown(0) && stateInfo.IsTag("Dash"))
        {
            animator.SetTrigger("DashAttack");
        }
    }
    void AirAttack()
    {
        emissionPoint.transform.rotation = Quaternion.Euler(look.transform.eulerAngles.x, look.transform.eulerAngles.y, 0f);
        if (Input.GetMouseButton(1) && player.flying == true && stateInfo.IsTag("FlyIdle") && !player.airAccelerating)
        {
            aiming = true;
            player.trueCamera.fieldOfView = Mathf.SmoothDamp(player.trueCamera.fieldOfView, 30, ref player.fovStretchSmoothVelocity, player.fovStretchSmoothTime);

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("AirAttack");

            }
        }
        else if (!Input.GetMouseButton(1) || !player.flying)
        {
            aiming = false;
        }
    }
    void AirAttackEmission()
    {
        player.VFXObjectPool.transform.GetChild(17).transform.position = emissionPoint.transform.position;
        player.VFXObjectPool.transform.GetChild(17).transform.rotation = Quaternion.Euler(-90f, emissionPoint.transform.eulerAngles.y, emissionPoint.transform.eulerAngles.z);
        player.VFXObjectPool.transform.GetChild(17).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        GameObject emissionObject = Instantiate(laser, emissionPoint.transform.position, emissionPoint.transform.rotation);
        emissionObject.GetComponent<ProjectileMoveScript>().user = gameObject;
    }
    void PunchComboAttack()
    {
        //體術模式

        if (Input.GetMouseButtonDown(0) && punch_State == true && stateInfo.IsTag("Fight"))
        {
            if (stateInfo.IsTag("Combat"))
            {
                return;
            }

            if (current_LightPunch == LightAttack.LightAttackFour)
                return;

            if (comboAdd == true)
            {
                current_LightPunch++;
                activateTimerToReset = true;
                current_Combo_Timer = 1.5f;
                firstSegment = true;
                secondSegment = false;
                animator.SetBool("firstSegment", firstSegment);
                animator.SetBool("secondSegment", secondSegment);
                if (current_LightPunch == LightAttack.into)
                {
                    current_LightPunch = LightAttack.LightAttackOne;
                }

                if (current_LightPunch == LightAttack.LightAttackOne)
                {
                    animator.SetTrigger("LAttackOne");
                }
                if (current_LightPunch == LightAttack.LightAttackTwo)
                {
                    animator.SetTrigger("LAttackTwo");
                }
                if (current_LightPunch == LightAttack.LightAttackThree)
                {
                    animator.SetTrigger("LAttackThree");
                }
                if (current_LightPunch == LightAttack.LightAttackFour)
                {
                    animator.SetTrigger("LAttackFour");
                }

                comboAdd = false;
            }

        }


        if (Input.GetMouseButtonDown(1) && punch_State == true && stateInfo.IsTag("Fight"))
        {


            if (stateInfo.IsTag("Combat"))
            {
                return;
            }
            //二段一套

            if (current_LightPunch == LightAttack.LightAttackTwo)
            {
                if (stateInfo.IsTag("Combat"))
                {
                    return;
                }
                if (current_SLightPunch == SLightAttack.SpecialLightAttackThree)
                {
                    return;
                }
                if (comboAdd == true)
                {
                    current_SLightPunch++;
                    activateTimerToReset = true;
                    current_Combo_Timer = 2.5f;
                    firstSegment = false;
                    secondSegment = true;
                    animator.SetBool("firstSegment", firstSegment);
                    animator.SetBool("secondSegment", secondSegment);

                    if (current_SLightPunch == SLightAttack.SpecialLightAttackOne)
                    {
                        animator.SetTrigger("SLAttackOne");
                    }
                    if (current_SLightPunch == SLightAttack.SpecialLightAttackTwo)
                    {
                        animator.SetTrigger("SLAttackTwo");
                    }
                    if (current_SLightPunch == SLightAttack.SpecialLightAttackThree)
                    {
                        animator.SetTrigger("SLAttackThree");
                    }
                    comboAdd = false;
                }
                return;
            }
            //二段二套

            if (current_LightPunch == LightAttack.LightAttackThree)
            {
                if (stateInfo.IsTag("Combat"))
                {
                    return;
                }
                if (current_SHeavyPunch == SHeavyAttack.SpecialHeavyAttackThree)
                {
                    return;
                }
                if (comboAdd == true)
                {
                    current_SHeavyPunch++;
                    activateTimerToReset = true;
                    current_Combo_Timer = 2.5f;
                    firstSegment = false;
                    secondSegment = true;
                    animator.SetBool("firstSegment", firstSegment);
                    animator.SetBool("secondSegment", secondSegment);

                    if (current_SHeavyPunch == SHeavyAttack.SpecialHeavyAttackOne)
                    {

                        animator.SetTrigger("SHAttackOne");
                    }
                    if (current_SHeavyPunch == SHeavyAttack.SpecialHeavyAttackTwo)
                    {

                        animator.SetTrigger("SHAttackTwo");
                    }
                    if (current_SHeavyPunch == SHeavyAttack.SpecialHeavyAttackThree)
                    {

                        animator.SetTrigger("SHAttackThree");
                    }
                    comboAdd = false;
                }
                return;
            }
            if (current_HeavyPunch == HeavyAttack.HeavyAttackFour)
                return;

            if (comboAdd == true)
            {
                current_HeavyPunch++;
                activateTimerToReset = true;
                current_Combo_Timer = 1.6f;
                firstSegment = true;
                secondSegment = false;
                animator.SetBool("firstSegment", firstSegment);
                animator.SetBool("secondSegment", secondSegment);

                if (current_HeavyPunch == HeavyAttack.into)
                {
                    current_HeavyPunch = HeavyAttack.HeavyAttackOne;
                }

                if (current_HeavyPunch == HeavyAttack.HeavyAttackOne)
                {
                    animator.SetTrigger("HAttackOne");
                }
                if (current_HeavyPunch == HeavyAttack.HeavyAttackTwo)
                {
                    animator.SetTrigger("HAttackTwo");
                }
                if (current_HeavyPunch == HeavyAttack.HeavyAttackThree)
                {
                    animator.SetTrigger("HAttackThree");
                }
                if (current_HeavyPunch == HeavyAttack.HeavyAttackFour)
                {
                    animator.SetTrigger("HAttackFour");
                }
                comboAdd = false;
            }
        }
    }



    void ResetComboState()
    {
        if (activateTimerToReset)
        {
            current_Combo_Timer -= Time.deltaTime;
            if (current_Combo_Timer <= 0f)
            {
                //一段
                current_LightPunch = LightAttack.None;
                current_HeavyPunch = HeavyAttack.None;
                //二段
                current_SLightPunch = SLightAttack.None;
                current_SHeavyPunch = SHeavyAttack.None;

                activateTimerToReset = false;
                current_Combo_Timer = default_Combo_Timer;

            }
        }


    }
    public void PerfectBlockOn()
    {
        StartCoroutine(PerfectBlock());
    }
    public IEnumerator PerfectBlock()
    {
        perfectBlock = true;      
        yield return new WaitForSecondsRealtime(0.1f);       
        perfectBlock = false;
        normalBlock = true;
        yield return new WaitForSecondsRealtime(0.5f);
        normalBlock = false;
        ChromaticAberrationOff();
    }
    
    public void BlockOff()
    {
        block = false;
    }
    public void DodgeOn()
    {
        dodge = true;
    }
    public void DodgeOff()
    {
        dodge = false;
    }
    public void ComboEnabled()
    {
        comboAdd = true;
    }
    public void HeavyAttackOn()
    {
        heavyAttack = true;
    }
    public void HeavyAttackOff()
    {
        heavyAttack = false;
    }
    public void AttackMove(float speed)
    {
        player.controller.Move(transform.forward * speed * Time.deltaTime);
    }
    public void AttackVFX()
    {

        player.VFXObjectPool.transform.GetChild(19).GetComponent<VFXObjectRecover>().attackUniversal.SetActive(true);
        player.VFXObjectPool.transform.GetChild(19).GetComponentInChildren<AttackUniversal>().user = gameObject;
        player.VFXObjectPool.transform.GetChild(19).transform.position = attackVFXPoing.transform.position;
        player.VFXObjectPool.transform.GetChild(19).transform.rotation = Quaternion.Euler(-90f, attackVFXPoing.transform.eulerAngles.y, attackVFXPoing.transform.eulerAngles.z);
        player.VFXObjectPool.transform.GetChild(19).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
    }

    public void RescueVFX()
    {
        player.VFXObjectPool.transform.GetChild(19).transform.position = attackVFXPoing.transform.position;
        player.VFXObjectPool.transform.GetChild(19).transform.rotation = Quaternion.Euler(-90f, attackVFXPoing.transform.eulerAngles.y, attackVFXPoing.transform.eulerAngles.z);
        player.VFXObjectPool.transform.GetChild(19).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
    }
    public IEnumerator CameraZoomOn()
    {
        camera.currentZoom = 2f;
        yield return new WaitForSeconds(0.5f);
        camera.currentZoom = 4f;
    }

    public void BlockVFX(int index)
    {
        if (index == 0)
        {
            player.VFXObjectPool.transform.GetChild(21).transform.position = blockVFXPoint.transform.position;
            player.VFXObjectPool.transform.GetChild(21).transform.eulerAngles = new Vector3(Random.Range(-50, 0), Random.Range(-90, 90),blockVFXPoint.transform.eulerAngles.z);
            player.VFXObjectPool.transform.GetChild(21).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
        else if (index == 1)
        {
            player.VFXObjectPool.transform.GetChild(20).transform.position = blockVFXPoint.transform.position;
            player.VFXObjectPool.transform.GetChild(20).transform.eulerAngles = new Vector3(Random.Range(-50, 0), Random.Range(-90, 90), blockVFXPoint.transform.eulerAngles.z);
            player.VFXObjectPool.transform.GetChild(20).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
        }
    }

    public IEnumerator ChangeChromaticAberration()
    {
        ChromaticAberrationOn();
        yield return new WaitForSecondsRealtime(1f);
        ChromaticAberrationOff();
    }
    public void ChromaticAberrationOn()
    {
        chromatic = processingProfile.chromaticAberration.settings;
        chromatic.intensity = 1;
        processingProfile.chromaticAberration.settings = chromatic;
    }
    public void ChromaticAberrationOff()
    {
        chromatic = processingProfile.chromaticAberration.settings;
        chromatic.intensity = 0;
        processingProfile.chromaticAberration.settings = chromatic;
    }
    public void SuperAttackVFX()
    {
    }

    public void Charge()
    {
    }
}
