using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackUniversal : MonoBehaviour
{
    public LayerMask collisionLayer;
    public float radius = 1f;
    public float damage = 2f;
    public float skillPower = 10f;

    public bool is_Player, isBoss,isArmed,isPolice,isVFX;

    public GameObject hit_FX_Prefab;
    public GameObject blood;
    public GameObject slamedFX;
    public GameObject[] BloodDecal;
    public GameObject willPowerVFX;
    private GameObject weapon;
    public GameObject user;

    [HideInInspector]
    public float[] BloodDecalsequenceX;
    [HideInInspector]
    public float[] BloodDecalsequenceY;
    [HideInInspector]
    public float BloodDecalDiffusionX;
    [HideInInspector]
    public float BloodDecalDiffusionY;

    Animator animator;   
    private PlayerController player;
    private new ThirdPersonCamera camera;
    private FightSystem fightSystem;
    private CameraShake cameraShake;
    private BossAI boss;
    private BlockSystem blockSystem;
    public Vector3 startPoint,endPoint;
    SlowMotion slowMotion;
    
    // Start is called before the first frame update
    void Start()
    {
        if (isBoss)
        {
            boss = GetComponentInParent<BossAI>();
        }
        else if (is_Player)
        {
            player = GetComponentInParent<PlayerController>();
            fightSystem = GetComponentInParent<FightSystem>();
            slowMotion = GetComponentInParent<SlowMotion>();
            blockSystem = GetComponentInParent<BlockSystem>();
        }
        if (user != null)
        {
            if (user.CompareTag("Enemy"))
            {
                boss = GetComponentInParent<BossAI>();
            }
        }
        animator = GetComponentInParent<Animator>();
        camera = GameObject.FindGameObjectWithTag("Camera").GetComponent<ThirdPersonCamera>();
        cameraShake = Camera.main.GetComponent<CameraShake>();              
        //weapon = GameObject.FindGameObjectWithTag("Weapon");
    }

    // Update is called once per frame
    void Update()
    {
        
        DetectCollision();
        
    }
    void DetectCollision()
    {
        if (gameObject.name == "WillPowerPoint")
        {
            radius = 5f;
            player.VFXObjectPool.transform.GetChild(13).transform.position = transform.position;
            player.VFXObjectPool.transform.GetChild(13).transform.rotation = transform.rotation;
            player.VFXObjectPool.transform.GetChild(13).gameObject.GetComponent<VFXObjectRecover>().VFX_Play();
            skillPower = 10f;
        }



        if (is_Player)
        {
            

            Collider[] hit = Physics.OverlapSphere(transform.position, radius, collisionLayer);
            if (hit.Length > 0)
            {                
                for(int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].GetComponentInParent<PedestrianAI>())
                    {
                        if (i > 0)
                        {
                            if (hit[i].GetComponentInParent<PedestrianAI>().gameObject == hit[i - 1].GetComponentInParent<PedestrianAI>().gameObject) continue;
                        }

                        hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                        cameraShake.StartShake(0.2f,0.25f);
                        //cameraShake.HitStopTimeOn();
                        print("We Hit The " + hit[i].gameObject.name);
                        hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                        hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - startPoint).normalized;
                        hit[i].gameObject.GetComponentInParent<StairDismount>().power = 30f;
                        hit[i].GetComponentInParent<PedestrianAI>().target = GameObject.FindGameObjectWithTag("Player");
                        PeopleSound peopleSound = hit[i].GetComponentInParent<PeopleSound>();
                        peopleSound.HitedSFX();
                        peopleSound.OuchSFX();
                        
                        //Vector3 hitFX_Pos = hit[0].transform.position;
                        Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                        BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                        BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                        /*if (hit[0].transform.forward.x > 0)
                        {
                            hitFX_Pos.x += 0.3f;
                            blood_Pos.x += 0.3f;
                        }
                        else if (hit[0].transform.forward.x < 0)
                        {
                            hitFX_Pos.x -= 0.3f;
                            blood_Pos.x -= 0.3f;
                        }*/

                        /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                        {
                            //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                        }
                        else
                        {
                            //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                        }*/

                        Instantiate(blood, blood_Pos, Quaternion.identity);
                        //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                        hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                    }

                    if (hit[i].GetComponentInParent<ArmedAI>())
                    {
                        if (i > 0)
                        {
                            if (hit[i].GetComponentInParent<ArmedAI>().gameObject == hit[i - 1].GetComponentInParent<ArmedAI>().gameObject) continue;
                        }

                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("AttackThree") || animator.GetCurrentAnimatorStateInfo(0).IsTag("WillPower"))
                        {
                            hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                            hit[i].GetComponentInParent<DelegateHitted>().kO = true;
                        }
                        else
                        {
                            hit[i].GetComponentInParent<DelegateHitted>().kO = false;
                        }
                        print("We Hit The " + hit[i].gameObject.name);
                        hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                        hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - startPoint).normalized;
                        hit[i].gameObject.GetComponentInParent<StairDismount>().power = skillPower;
                        hit[i].GetComponent<HittedPart>().hitted = true;
                        hit[i].GetComponentInParent<DelegateHitted>().currentHitted = hit[i].GetComponent<HittedPart>();
                        hit[i].GetComponentInParent<DelegateHitted>().attackPosition = startPoint;
                        //hit[i].GetComponentInParent<PedestrianAI>().target = GameObject.FindGameObjectWithTag("Player");


                        //camera.ShakePower = 0.2f;
                        //camera.ShakeTimes = 5;
                        //Vector3 hitFX_Pos = hit[0].transform.position;
                        Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                        BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                        BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                        /*if (hit[0].transform.forward.x > 0)
                        {
                            hitFX_Pos.x += 0.3f;
                            blood_Pos.x += 0.3f;
                        }
                        else if (hit[0].transform.forward.x < 0)
                        {
                            hitFX_Pos.x -= 0.3f;
                            blood_Pos.x -= 0.3f;
                        }*/

                        /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                        {
                            //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                        }
                        else
                        {
                            //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                        }*/

                        Instantiate(blood, blood_Pos, Quaternion.identity);
                        //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                        hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                    }


                    if (hit[i].GetComponentInParent<BossAI>())
                    {
                        if (i > 0)
                        {
                            if (hit[i].GetComponentInParent<BossAI>().gameObject == hit[i - 1].GetComponentInParent<BossAI>().gameObject) continue;
                        }
                        if (blockSystem.buff)
                        {
                            damage = 10;
                        }
                        else
                        {
                            damage = 2;
                        }
                        hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                        if (fightSystem.heavyAttack|| hit[i].GetComponentInParent<HealthScript>().health<=0)
                        {
                            slowMotion.DoSlowMotion();
                            //hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                            //hit[i].GetComponentInParent<DelegateHitted>().kO = true;
                            //BossSound bossSound = hit[i].GetComponentInParent<BossSound>();
                            //bossSound.SHitedSFX();
                        }
                        else
                        {
                            cameraShake.StartShake(0.2f, 0.25f);
                        }
                        hit[i].GetComponentInParent<DelegateHitted>().kO = false;                      
                        BossSound bossSound = hit[i].GetComponentInParent<BossSound>();
                        bossSound.HitedSFX();
                        print("We Hit The " + hit[i].gameObject.name);
                        hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                        hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - startPoint).normalized;
                        hit[i].gameObject.GetComponentInParent<StairDismount>().power = skillPower;
                        hit[i].GetComponent<HittedPart>().hitted = true;
                        hit[i].GetComponentInParent<DelegateHitted>().currentHitted = hit[i].GetComponent<HittedPart>();
                        hit[i].GetComponentInParent<DelegateHitted>().attackPosition = startPoint;
                        


                        Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                        BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                        BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                        /*if (hit[0].transform.forward.x > 0)
                        {
                            hitFX_Pos.x += 0.3f;
                            blood_Pos.x += 0.3f;
                        }
                        else if (hit[0].transform.forward.x < 0)
                        {
                            hitFX_Pos.x -= 0.3f;
                            blood_Pos.x -= 0.3f;
                        }*/

                        /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                        {
                            //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                        }
                        else
                        {
                            //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                        }*/

                        Instantiate(blood, blood_Pos, Quaternion.identity);
                        //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                    }

                    if (hit[i].gameObject.CompareTag("Car"))
                    {
                        hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                    }



                }
                

                //擊飛
                /*if (gameObject.CompareTag("HitFly_Arm"))
                {
                    Debug.Log("We Hit fly " + hit[0].gameObject.name);

                    hit[0].GetComponent<AI>().HittedFly();


                }*/

                gameObject.SetActive(false);
            }
        }
        else if (isBoss)
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, radius, collisionLayer);
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (hit[i].GetComponentInParent<PlayerController>())
                    {
                        if (i > 0)
                        {
                            if (hit[i].GetComponentInParent<PlayerController>().gameObject == hit[i - 1].GetComponentInParent<PlayerController>().gameObject) continue;
                        }                                                
                        if (boss.heavyAttack||hit[i].GetComponentInParent<PlayerController>().fallDown|| hit[i].GetComponentInParent<HealthScript>().health<=0)
                        {
                            //hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                            hit[i].GetComponentInParent<DelegateHitted>().kO = true;
                        }
                        else
                        {
                            hit[i].GetComponentInParent<DelegateHitted>().kO = false;
                        }
                        if (!hit[i].GetComponentInParent<FightSystem>().dodge&&!hit[i].GetComponentInParent<FightSystem>().block)
                        {
                            hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                            cameraShake.StartShake(0.2f, 0.25f);
                            print("We Hit The " + hit[i].gameObject.name);
                            hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                            hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - startPoint).normalized;
                            hit[i].gameObject.GetComponentInParent<StairDismount>().power = skillPower;
                            hit[i].GetComponent<HittedPart>().hitted = true;
                            hit[i].GetComponentInParent<DelegateHitted>().attackPosition = startPoint;
                            PlayerSound playerSound = hit[i].GetComponentInParent<PlayerSound>();
                            playerSound.HitedSFX();
                            hit[i].GetComponentInParent<FightSystem>().fighting = true;
                            //Vector3 hitFX_Pos = hit[0].transform.position;
                            Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                            BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                            BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                            /*if (hit[0].transform.forward.x > 0)
                            {
                                hitFX_Pos.x += 0.3f;
                                blood_Pos.x += 0.3f;
                            }
                            else if (hit[0].transform.forward.x < 0)
                            {
                                hitFX_Pos.x -= 0.3f;
                                blood_Pos.x -= 0.3f;
                            }*/

                            /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                            {
                                //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                            }
                            else
                            {
                                //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                            }*/

                            Instantiate(blood, blood_Pos, Quaternion.identity);
                            //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                        }
                        else if (hit[i].GetComponentInParent<FightSystem>().block)
                        {
                            if (!hit[i].GetComponentInParent<FightSystem>().noBlock)
                            {
                                int x = Random.Range(0, 4);
                                switch (x)
                                {
                                    case 0:
                                        hit[i].GetComponentInParent<Animator>().Play("Guard_Hit_1", 0);
                                        break;
                                    case 1:
                                        hit[i].GetComponentInParent<Animator>().Play("Guard_Hit_2", 0);
                                        break;
                                    case 2:
                                        hit[i].GetComponentInParent<Animator>().Play("Block_GetHit_Right", 0);
                                        break;
                                    case 3:
                                        hit[i].GetComponentInParent<Animator>().Play("Block_GetHit_Left", 0);
                                        break;
                                }

                                if (hit[i].GetComponentInParent<FightSystem>().perfectBlock)
                                {
                                    Debug.Log("完美格黨");
                                    cameraShake.StartShake(0.2f, 0.25f);
                                    hit[i].GetComponentInParent<FightSystem>().ChromaticAberrationOn();
                                    hit[i].GetComponentInParent<FightSystem>().BlockVFX(0);
                                    hit[i].GetComponentInParent<PlayerSound>().BlockSFX(0);
                                    hit[i].GetComponentInParent<BlockSystem>().SetBlockValue(20f);
                                    hit[i].GetComponentInParent<BlockSystem>().SetFatigueValue(20f);
                                }
                                else if (hit[i].GetComponentInParent<FightSystem>().normalBlock)
                                {
                                    Debug.Log("普通格黨");
                                    cameraShake.StartShake(0.2f, 0.25f);
                                    hit[i].GetComponentInParent<FightSystem>().ChromaticAberrationOn();
                                    hit[i].GetComponentInParent<FightSystem>().BlockVFX(1);
                                    hit[i].GetComponentInParent<PlayerSound>().BlockSFX(1);
                                    hit[i].GetComponentInParent<BlockSystem>().SetBlockValue(10f);
                                    hit[i].GetComponentInParent<BlockSystem>().SetFatigueValue(30f);
                                }
                            }
                            else
                            {
                                hit[i].GetComponentInParent<Animator>().Play("Guard_Break", 0);
                                cameraShake.StartShake(0.2f, 0.25f);
                                hit[i].GetComponentInParent<FightSystem>().BlockVFX(0);
                                hit[i].GetComponentInParent<PlayerSound>().BlockSFX(0);
                                hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage*0.5f);
                            }
                        }
                    }                    
                }
                gameObject.SetActive(false);
            }
        }
        else if (isVFX)
        {
            if(user!=null&&user.CompareTag("Enemy"))
            {
                collisionLayer = 1<<13;
                Collider[] hit = Physics.OverlapSphere(transform.position, radius, collisionLayer);
                if (hit.Length > 0)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        if (hit[i].GetComponentInParent<PlayerController>())
                        {
                            if (i > 0)
                            {
                                if (hit[i].GetComponentInParent<PlayerController>().gameObject == hit[i - 1].GetComponentInParent<PlayerController>().gameObject) continue;
                            }

                            if (user.GetComponent<BossAI>().heavyAttack || hit[i].GetComponentInParent<PlayerController>().fallDown)
                            {
                                hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                                hit[i].GetComponentInParent<DelegateHitted>().kO = true;
                            }
                            else
                            {
                                hit[i].GetComponentInParent<DelegateHitted>().kO = false;
                            }
                            cameraShake.StartShake(0.2f, 0.25f);
                            print("We Hit The " + hit[i].gameObject.name);
                            hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                            hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - transform.position).normalized;
                            hit[i].gameObject.GetComponentInParent<StairDismount>().power = skillPower;
                            PlayerSound playerSound = hit[i].GetComponentInParent<PlayerSound>();
                            playerSound.HitedSFX();
                            hit[i].GetComponentInParent<FightSystem>().fighting = true;
                            //Vector3 hitFX_Pos = hit[0].transform.position;
                            Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                            BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                            BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                            /*if (hit[0].transform.forward.x > 0)
                            {
                                hitFX_Pos.x += 0.3f;
                                blood_Pos.x += 0.3f;
                            }
                            else if (hit[0].transform.forward.x < 0)
                            {
                                hitFX_Pos.x -= 0.3f;
                                blood_Pos.x -= 0.3f;
                            }*/

                            /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                            {
                                //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                            }
                            else
                            {
                                //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                            }*/

                            Instantiate(blood, blood_Pos, Quaternion.identity);
                            //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                            hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                        }
                    }
                    gameObject.SetActive(false);
                }
                gameObject.SetActive(false);
            }
            if (user!=null&&user.CompareTag("Player"))
            {
                Collider[] hit = Physics.OverlapSphere(transform.position, radius, collisionLayer);
                if (hit.Length > 0)
                {
                    for (int i = 0; i < hit.Length; i++)
                    {
                        if (hit[i].GetComponentInParent<PedestrianAI>())
                        {
                            if (i > 0)
                            {
                                if(hit[i - 1].GetComponentInParent<PedestrianAI>())
                                {
                                    if (hit[i].GetComponentInParent<PedestrianAI>().gameObject == hit[i - 1].GetComponentInParent<PedestrianAI>().gameObject) continue;
                                }                              
                            }
                            hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);

                            hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                            cameraShake.StartShake(0.2f, 0.25f);
                            //cameraShake.HitStopTimeOn();
                            print("We Hit The " + hit[i].gameObject.name);
                            hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                            hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - transform.position).normalized;
                            hit[i].gameObject.GetComponentInParent<StairDismount>().power = skillPower;
                            hit[i].GetComponentInParent<PedestrianAI>().target = GameObject.FindGameObjectWithTag("Player");
                            PeopleSound peopleSound = hit[i].GetComponentInParent<PeopleSound>();
                            peopleSound.HitedSFX();
                            peopleSound.OuchSFX();

                            //Vector3 hitFX_Pos = hit[0].transform.position;
                            Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                            BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                            BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                            /*if (hit[0].transform.forward.x > 0)
                            {
                                hitFX_Pos.x += 0.3f;
                                blood_Pos.x += 0.3f;
                            }
                            else if (hit[0].transform.forward.x < 0)
                            {
                                hitFX_Pos.x -= 0.3f;
                                blood_Pos.x -= 0.3f;
                            }*/

                            /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                            {
                                //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                            }
                            else
                            {
                                //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                            }*/


                            //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                        }

                        if (hit[i].gameObject.CompareTag("Car"))
                        {                            
                            hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                        }



                        if (hit[i].GetComponentInParent<BossAI>())
                        {
                            if (i > 0)
                            {
                                if (hit[i - 1].GetComponentInParent<BossAI>())
                                {
                                    if (hit[i].GetComponentInParent<BossAI>().gameObject == hit[i - 1].GetComponentInParent<BossAI>().gameObject) continue;
                                }
                            }
                            hit[i].GetComponentInParent<HealthScript>().ApplyDamage(damage);
                            if (!hit[i].GetComponentInParent<BossAI>().noDown)
                            {
                                user.GetComponent<SlowMotion>().DoSlowMotion();
                                hit[i].transform.GetComponentInParent<StairDismount>().startEffect = true;
                                cameraShake.StartShake(0.2f, 0.25f);
                            }                            
                            print("We Hit The " + hit[i].gameObject.name);
                            hit[i].gameObject.GetComponentInParent<StairDismount>().impactTarget = hit[i].attachedRigidbody;
                            hit[i].GetComponentInParent<StairDismount>().impact = (hit[i].transform.position - transform.position).normalized;
                            hit[i].gameObject.GetComponentInParent<StairDismount>().power = skillPower;
                            BossSound bossSound = hit[i].GetComponentInParent<BossSound>();
                            bossSound.SHitedSFX();

                            //Vector3 hitFX_Pos = hit[0].transform.position;
                            Vector3 blood_Pos = hit[i].transform.position + ((transform.position - hit[i].transform.position).normalized * 0.4f);

                            BloodDecalsequenceX = new float[3] { -1, 0, 1 };
                            BloodDecalsequenceY = new float[3] { -1, 0, 1 };

                            /*if (hit[0].transform.forward.x > 0)
                            {
                                hitFX_Pos.x += 0.3f;
                                blood_Pos.x += 0.3f;
                            }
                            else if (hit[0].transform.forward.x < 0)
                            {
                                hitFX_Pos.x -= 0.3f;
                                blood_Pos.x -= 0.3f;
                            }*/

                            /*if(hit[0].transform.GetComponentInParent<EnemyController>().slamed == true&&slamedFX!=null)
                            {
                                //Instantiate(slamedFX, hitFX_Pos, Quaternion.identity);
                            }
                            else
                            {
                                //Instantiate(hit_FX_Prefab, hitFX_Pos, Quaternion.identity);
                            }*/


                            //Instantiate(BloodDecal[Random.Range(0, BloodDecal.Length - 1)], hit[0].transform.position + new Vector3(GetBloodDecalRandomX(), 0, GetBloodDecalRandomY()), Quaternion.Euler(90, 0, 0));
                        }
                    }
                    gameObject.SetActive(false);
                }
                
            }
        }
    }

    public float GetBloodDecalRandomX()
    {

        int end = 3 - 1;
        int num = Random.Range(0, end + 1);
        BloodDecalDiffusionX = BloodDecalsequenceX[num];


        BloodDecalsequenceX[num] = BloodDecalsequenceX[end];

        end--;

        return BloodDecalDiffusionX;
    }

    public float GetBloodDecalRandomY()
    {

        int end = 3 - 1;
        int num = Random.Range(0, end + 1);

        BloodDecalDiffusionY = BloodDecalsequenceY[num];


        BloodDecalsequenceY[num] = BloodDecalsequenceY[end];

        end--;
        return BloodDecalDiffusionY;
    }


}
