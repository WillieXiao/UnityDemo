using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float shakeTimeRemaining = 0f, shakePower, shakeFadeTime, shakeRotation;
    public float rotationMultiplier = 8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;

            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(xAmount, yAmount, 0f), 0.5f);
            
            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);
            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, Vector3.zero, 1f);
        }
        transform.localRotation = Quaternion.Euler(0, 0, shakeRotation * Random.Range(-1f, 1f));
    }

    public void StartShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;
        shakeFadeTime = power / length;
        shakeRotation = power * rotationMultiplier;
    }

    public void HitStopTimeOn()
    {
        StartCoroutine("HitStopTime");
    }
    public IEnumerator HitStopTime()
    {

        if (Time.timeScale == 1.0)
            Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }
}
