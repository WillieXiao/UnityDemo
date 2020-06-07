using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public float slowDownFactor = 0.2f;
    public float slowDownLength = 2f;
    private float hitStopTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

    }
    public void DoSlowMotion()
    {
        StartCoroutine(Slowmotion());
    }
    public IEnumerator Slowmotion()
    {
        Time.timeScale = slowDownFactor;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }

    public void HitStopTimeOn()
    {
        StartCoroutine("HitStopTime");
    }
    public IEnumerator HitStopTime()
    {

        if (Time.timeScale == 1.0)
            Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = 1;
    }
}
