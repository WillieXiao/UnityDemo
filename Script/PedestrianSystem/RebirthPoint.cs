using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebirthPoint : MonoBehaviour
{
    public GameObject[] carPrefab;
    public GameObject[] humanPrefab;
    public GameObject[] policePrefab;
    private PedestrianAI controller;
    private ArmedAI armed;
    private CarAI car;
    private WayStart wayStart;
    public bool isGeneralPeople,isSocialPeople,isCar,isPolice,isArmed;
    private bool spawnCar = true;
    private bool spawnPeople = true;
    public GameObject[] carResult;
    public GameObject[] peopleResult;
    public GameObject[] armedResult;
    public GameObject carObjPool;
    public GameObject peopleObjPool;
    public GameObject ArmedObjPool;
    public WayStart originMainRoad;
    public GameObject player;
    private int a = 0;

    private Dictionary<int, List<GameObject>> pool;
    private Dictionary<int, GameObject> prefabs;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        carResult = new GameObject[carPrefab.Length];
        peopleResult = new GameObject[humanPrefab.Length];
        armedResult = new GameObject[policePrefab.Length];
        wayStart = GetComponentInParent<WayStart>();
        if (isGeneralPeople)
        {
            for (int i = 0; i < humanPrefab.Length; i++)
            {
                controller = humanPrefab[i].GetComponent<PedestrianAI>();
                controller.pathPoint = new GameObject[wayStart.wayNode.Length];
                if (controller.generalPedestrian)
                {
                    controller.pathPoint = new GameObject[wayStart.wayNode.Length];
                    for (int j = 0; j < wayStart.wayNode.Length; j++)
                    {
                        if (wayStart.wayNode[j].GetComponent<PathPoint>().startPoint)
                        {
                            controller.a = j;
                            wayStart.startIndex = j;
                        }
                        controller.pathPoint[j] = wayStart.wayNode[j];
                    }
                    
                }
                peopleResult[i] = Instantiate<GameObject>(humanPrefab[i], wayStart.wayNode[wayStart.startIndex].transform.position, peopleObjPool.transform.rotation, peopleObjPool.transform);
                peopleResult[i].GetComponent<PedestrianAI>().originMainRoad = originMainRoad;
                peopleObjPool.transform.GetChild(i).gameObject.SetActive(false);
            }
            
        }
        if (isPolice)
        {
            for (int i = 0; i < policePrefab.Length; i++)
            {
                armed = policePrefab[i].GetComponent<ArmedAI>();
                armed.pathPoint = new GameObject[wayStart.wayNode.Length];
                if (armed.isPolice)
                {
                    armed.pathPoint = new GameObject[wayStart.wayNode.Length];
                    for (int j = 0; j < wayStart.wayNode.Length; j++)
                    {
                        if (wayStart.wayNode[j].GetComponent<PathPoint>().startPoint)
                        {
                            armed.a = j;
                            wayStart.startIndex = j;
                        }
                        armed.pathPoint[j] = wayStart.wayNode[j];
                    }

                }
                armedResult[i] = Instantiate<GameObject>(policePrefab[i], wayStart.wayNode[wayStart.startIndex].transform.position, ArmedObjPool.transform.rotation, ArmedObjPool.transform);
                armedResult[i].GetComponent<ArmedAI>().originMainRoad = originMainRoad;
                ArmedObjPool.transform.GetChild(i).gameObject.SetActive(false);
            }

        }
        else if (isSocialPeople)
        {
            for(int i = 0; i < humanPrefab.Length; i++)
            {
                peopleResult[i] = Instantiate<GameObject>(humanPrefab[0], transform.position, transform.rotation);
                controller = peopleResult[i].GetComponent<PedestrianAI>();
                controller.generalPedestrian = false;
                controller.socialPedestrian = true;
            }
        }
        else if (isCar)
        {
            
            for (int i = 0; i < carPrefab.Length; i++)
            {
                car = carPrefab[i].GetComponent<CarAI>();
                car.rebirth = wayStart.wayNode[0].GetComponent<RebirthPoint>();
                car.pathPoint = new GameObject[wayStart.wayNode.Length];
                for (int j = 0; j < wayStart.wayNode.Length; j++)
                {
                    if (wayStart.wayNode[j].GetComponent<PathPoint>().startPoint)
                    {
                        car.a = j;
                        wayStart.startIndex = j;
                    }
                    car.pathPoint[j] = wayStart.wayNode[j];
                }

                carResult[i] = Instantiate<GameObject>(carPrefab[i],wayStart.wayNode[wayStart.startIndex].transform.position,carObjPool.transform.rotation,carObjPool.transform);
                carResult[i].GetComponent<CarAI>().originMainRoad = originMainRoad;
                carObjPool.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGeneralPeople)
        {
            for(int i = 0; i < peopleResult.Length; i++)
            {
                if (Vector3.Distance(player.transform.position,peopleResult[i].transform.position) >= 70f&&peopleResult[i].activeSelf == true)
                {
                    peopleResult[i].GetComponent<PedestrianAI>().InitializationPeople();
                }
            }
        }

        if (isCar && spawnCar && a < carResult.Length && carObjPool != null)
        {
            StartCoroutine(GetCar());
            spawnCar = false;
            
        }
        else if (isGeneralPeople && spawnPeople && a < peopleResult.Length && peopleObjPool != null)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 100f)
            {
                StartCoroutine(GetPeople());
                spawnPeople = false;
            }
            
        }
        else if (isPolice && spawnPeople && a < armedResult.Length && ArmedObjPool != null)
        {
            StartCoroutine(GetPolice());
            spawnPeople = false;
        }
    }
    public IEnumerator GetCar()
    {
        yield return new WaitForSeconds(Random.Range(5f,8f));

        if (!carObjPool.transform.GetChild(a).gameObject.activeSelf)
        {
            Debug.Log("生產車輛");
            carObjPool.transform.GetChild(a).gameObject.SetActive(true);            
        }
        if (a >= carResult.Length - 1)
        {
            a = 0;
        }
        else
        {
            a++;
        }
        spawnCar = true;

    }

    public IEnumerator GetPeople()
    {
        yield return new WaitForSeconds(Random.Range(3f,5f));

        if (!peopleObjPool.transform.GetChild(a).gameObject.activeSelf)
        {
            peopleObjPool.transform.GetChild(a).gameObject.SetActive(true);                    
        }
        if (a >= peopleResult.Length - 1)
        {
            a = 0;
        }
        else
        {
            a++;
        }
        spawnPeople = true;
    }

    public IEnumerator GetPolice()
    {
        yield return new WaitForSeconds(Random.Range(1f, 3f));

        if (!ArmedObjPool.transform.GetChild(a).gameObject.activeSelf)
        {
            ArmedObjPool.transform.GetChild(a).gameObject.SetActive(true);
            if (a >= armedResult.Length - 1)
            {
                a = 0;
            }
            else
            {
                a++;
            }
        }
        spawnPeople = true;
    }
}
