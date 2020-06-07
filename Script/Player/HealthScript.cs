using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class HealthScript : MonoBehaviour
{
    public float health = 100f;

    private NavMeshAgent agent;
    private StairDismount stair;
    ThirdPersonCamera personCamera;
    Animator animator;
    PlayerController player;
    BossAI boss;
    CarAI car;
    PedestrianAI people;
    ThirdPersonCamera camera;
    public Slider hP_slider;
    private bool characterDied;
    public bool is_Player;
    public bool is_Enemy,is_Car;
    public bool is_Boss;
    public bool hurt;


    public GameObject brokenCar;
    void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("Camera").GetComponent<ThirdPersonCamera>();
        if (is_Enemy)
        {
            people = GetComponent<PedestrianAI>();
            stair = GetComponent<StairDismount>();
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            personCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<ThirdPersonCamera>();
        }
        else if (is_Boss)
        {
            SetHealthBar();
            boss = GetComponent<BossAI>();
            stair = GetComponent<StairDismount>();
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            personCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<ThirdPersonCamera>();
        }
        else if (is_Car)
        {
            car = GetComponent<CarAI>();
        }
        else if (is_Player)
        {
            SetHealthBar();
            player = GetComponent<PlayerController>();
        }
    }
    public void ApplyDamage(float damage)
    {
        if (characterDied)
            return;
        health -= damage;
        if (is_Boss||is_Player)
        {
            SetHealthBar();
        }

        //StartCoroutine(Hurt());
        if (health <= 0f)
        {
            characterDied = true;
            if (is_Player)
            {
                player.dead = true;
            }
            else if (is_Enemy)
            {

                StartCoroutine(returnPeopleBody());
            }
            else if (is_Boss)
            {
                boss.dead = true;
            }
            else if (is_Car)
            {
                InstantiateBCar();
                car.InitializationCar();
            }
        }
        else
        {
            if (is_Boss)
            {
                

            }
            if (is_Player)
            {

            }
        }       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealthBar()
    {
        hP_slider.value = health;
    }


    void CharacterDied()
    {
        
    }
    public IEnumerator returnPeopleBody()
    {        
        yield return new WaitForSeconds(3f);
        health = 100;
        people.InitializationPeople();
    }

    public void InstantiateBCar()
    {
        Instantiate(brokenCar, transform.position, transform.rotation);
    }
    public void Dead()
    {
        animator.enabled = false;
        player.enabled = false;
    }
}
