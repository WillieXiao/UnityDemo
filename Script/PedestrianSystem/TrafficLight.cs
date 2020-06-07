using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public int currentLightIndex = 0;
    private bool positive;
    float time = 5;
    public bool canWalk;
    public bool carCan;
    public int howManyInMe;
    private bool flicker;
    public float lightSecond = 15f;
    public bool isCross;
    public int HOW_MANY
    {
        get { return howManyInMe; }
        private set { }
    }
    public bool FLICKER
    {
        get { return flicker; }
        set { flicker = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLightIndex == 1)
        {
            flicker = true;
        }

        if (currentLightIndex == 2)
        {
            canWalk = false;
            carCan = true;
            positive = false;
            flicker = false;
        }
        if (currentLightIndex == 0)
        {
            canWalk = true;
            carCan = false;
            positive = true;
            flicker = false;
        }
        if (!isCross)
        {            
            if (Time.time > time)
            {
                currentLightIndex = currentLightIndex + ((positive) ? 1 : -1);
                if (currentLightIndex == 1)
                {
                    time = Time.time + (lightSecond * 0.3f);
                }
                else
                {
                    time = Time.time + lightSecond;
                }

            }
        }       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("People"))
        {
            howManyInMe++;
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

        if (other.CompareTag("People"))
        {
            howManyInMe--;
        }
    }
}
