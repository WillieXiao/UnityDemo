using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public TransformVariable lockonTransform;
    public float positionSmoothTime = 1.2f;
    Vector3 positionSmoothVelocity;
    public bool lockOn;

    float yaw;
    float pitch;

    public float mouseSensitivity = 10;
    public Transform camera1Pos, camera2Pos,camera3Pos,camera4Pos;
    public Transform Target;
    public float dstFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = 1.2f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    Vector3 cameraPosition;
    public float currentZoom;
    public Vector2 ZoomMinMax = new Vector2(1, 5);
    float zoomSpeed = 5;


 

    PlayerController player;
    HealthScript health;
    Animator animator;
    public Transform realCamera;
    private float hitStopTime = 0.1f;
    Vector3 pos;
    public float waitForTime = 0.005f;

    private float shakeTimeRemaining, shakePower1, shakeFadeTime,shakeRotation;
    public float rotationMultiplier = 8f;

    // Start is called before the first frame update
    void Start()
    {
        lockonTransform.value = null;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (player.fallDown == true)
        {
            Debug.Log("55151");
            Target = camera2Pos;
        }
        else if (player.fightSystem.aiming)
        {
            Target = camera3Pos;
        }
        else if (!player.fightSystem.aiming&&!lockOn)
        {
            Target = camera1Pos;
        }
        else if (lockOn)
        {
            Target = camera1Pos;
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        //鏡頭震動
        
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

        if (lockonTransform.value != null)
        {
            Vector3 targetDir = lockonTransform.value.position - transform.position;
            targetDir.Normalize();
            //targetDir.y = Mathf.Clamp(targetDir.y, -0.2f, 0.5f);
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.5f);
            transform.position = Vector3.Slerp(transform.position, (Target.position - transform.forward * dstFromTarget * currentZoom),0.5f);

            yaw = transform.eulerAngles.y;
            pitch = transform.eulerAngles.x;           
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, ZoomMinMax.x, ZoomMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;
            transform.position = Target.position - transform.forward * dstFromTarget * currentZoom;
        }
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
