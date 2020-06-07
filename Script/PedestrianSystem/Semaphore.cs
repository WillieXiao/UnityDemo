using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semaphore : MonoBehaviour
{
    public TrafficLight[] crossTrafficLight;
    int a = 0;
    float time = 0;
    public bool crossControll;
    private bool positive;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (crossControll)
        {
            if (crossTrafficLight[a].currentLightIndex == 2)
            {
                positive = false;
            }
            if (crossTrafficLight[a].currentLightIndex == 0)
            {
                positive = true;
            }
            if (Time.time > time)
            {
                if (a != 0)
                {
                    crossTrafficLight[a-1].currentLightIndex = crossTrafficLight[a-1].currentLightIndex-1;
                }
                else
                {
                    if (crossTrafficLight[crossTrafficLight.Length-1].currentLightIndex != 0)
                    {
                        crossTrafficLight[crossTrafficLight.Length - 1].currentLightIndex = crossTrafficLight[crossTrafficLight.Length - 1].currentLightIndex - 1;
                    }
                }
                crossTrafficLight[a].currentLightIndex = crossTrafficLight[a].currentLightIndex + ((positive) ? 1 : -1);
                if (crossTrafficLight[a].currentLightIndex == 1)
                {
                    time = Time.time + 5f;
                }
                else
                {
                    time = Time.time + 15f;
                    a++;
                }               
            }
            if (a > crossTrafficLight.Length - 1)
            {
                a = 0;
            }
        }       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Car"))
        {
            if (other.transform.GetComponentInParent<CarAI>())
            {
                CarAI car = other.GetComponentInParent<CarAI>();
                car.INSIDE = true;
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Car"))
        {
            if (other.transform.GetComponentInParent<CarAI>())
            {
                CarAI car = other.GetComponentInParent<CarAI>();
                car.INSIDE = false;
            }
        }
    }
}
