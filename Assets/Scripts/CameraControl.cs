using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private float azimuth = 0f; // theta
    private float elevation = 0f; // pi
    private float radius = 0f; // r
    [SerializeField]
    private float speed = 3f;

    private float t = 0f;
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;

    private Camera ThirdPersonCam;

    private void Awake()
    {
        ThirdPersonCam = GetComponentInChildren<Camera>();
        Debug.Log(ThirdPersonCam.name);
    }
    private void Start()
    {
        Vector3 pos = ThirdPersonCam.transform.localPosition;
        radius = pos.magnitude;
        azimuth = Mathf.Atan2(pos.z, pos.x); // pos z / pos x
        elevation = Mathf.Acos(pos.y / radius); // arc cosine
        UpdateCamera();
    }
    private void Update()
    {


        azimuth += Input.GetAxis("Mouse X") * speed * Time.deltaTime;

        elevation -= Input.GetAxis("Mouse Y")  * speed * Time.deltaTime;
        Debug.Log(elevation);
        elevation = Mathf.Clamp(elevation, 4.5f, 5.8f);
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        t = radius * Mathf.Sin(elevation);
        x = t * Mathf.Cos(azimuth);
        y = radius * Mathf.Cos(elevation);
        z = t * Mathf.Sin(azimuth);
        ThirdPersonCam.transform.localPosition = new Vector3(x, y, z);
        ThirdPersonCam.transform.LookAt(transform);
    }
    /*
    private GameObject LookTarget;
    private float smoothSpeed = 20f;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;

    public Vector3 CamPos;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float mouseSensitity = 200f;

    private bool AltInput = false;
    private bool AltFirst = false;

    private void Start()
    {
        LookTarget = GameObject.Find("LookPos");
        CamPos.Set(0f, 0.4f, -2.3f);
    }
    private void Update()
    {
        AltFirst = Input.GetKeyDown(KeyCode.LeftAlt);
        AltInput = Input.GetKey(KeyCode.LeftAlt);
    }
    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, new Vector3(transform.forward.x, 0f, transform.forward.z) * 5f, Color.red);
        //desiredRotation = Quaternion.LookRotation(LookTarget.transform.position - transform.position);

        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);

        if(AltFirst)
        {
            //mouseX = transform.eulerAngles.y;
            //mouseY = transform.eulerAngles.x;
            //transform.rotation = Quaternion.LookRotation(LookTarget.transform.position - transform.position);
        }
        if(AltInput)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitity;

            transform.RotateAround(LookTarget.transform.position, Vector3.up, mouseX * Time.deltaTime);
            transform.RotateAround(LookTarget.transform.position, transform.right, -mouseY * Time.deltaTime);
        }
        else
        {
            desiredPosition = LookTarget.transform.TransformPoint(CamPos);
            desiredRotation = Quaternion.LookRotation(LookTarget.transform.position - transform.position);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
        }

    }*/
}
