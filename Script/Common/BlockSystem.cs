using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSystem : MonoBehaviour
{
    FightSystem fightSystem;
    public Slider slider, slider2;
    public float currentValue, currentValue2;
    public float returnTimer;
    public bool blocking;
    public bool buff;
    public bool coolDown;
    // Start is called before the first frame update
    void Start()
    {
        fightSystem = GetComponent<FightSystem>();
    }

    // Update is called once per frame
    void Update()
    {       
        if (blocking&&slider.value>0)
        {
            returnTimer += Time.deltaTime;
        }
        else
        {
            returnTimer = 0;
        }
        if(returnTimer >= 10f&& slider.value > 0)
        {
            ReduceBlockValue();
            ReduceFatigueValue();
            returnTimer = 0;
        }
        if (slider.value == 100&&!coolDown)
        {
            coolDown = true;
            buff = true;
        }
        if (coolDown && slider.value > 0)
        {
            slider.value = 0;
            coolDown = false;
        }

        if (slider2.value == 100 && !coolDown)
        {
            coolDown = true;
            StartCoroutine(NoBlock());
        }
        if (coolDown && slider2.value > 0)
        {
            slider2.value = 0;
            coolDown = false;
        }
    }
    public void SetBlockValue(float addValue)
    {
        if (slider.value < 100&&!buff)
        {
            blocking = false;
            slider.value += addValue;
            blocking = true;
        }
    }
    public void SetFatigueValue(float addValue)
    {
        if (slider2.value < 100 && !buff)
        {
            blocking = false;
            slider2.value += addValue;
            blocking = true;
        }       
    }
    public void ReduceBlockValue()
    {
        slider.value -= 10f;
    }
    public void ReduceFatigueValue()
    {
        slider2.value -= 10f;
    }

    public IEnumerator NoBlock()
    {
        fightSystem.noBlock = true;
        yield return new WaitForSecondsRealtime(3f);
        fightSystem.noBlock = false;
    }
}
