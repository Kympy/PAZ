using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameObject LookTarget;
    private float smoothSpeed = 20f;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;

    public Vector3 CamPos;
    private Vector3 AltCamPos;
    private float mouseX = 0f;
    private float mouseRotation = 0f;
    private float mouseSensitity = 200f;

    private bool AltInput = false;
    private bool AltFirst = false;

    private void Start()
    {
        LookTarget = GameObject.Find("LookPos");
        mouseRotation += Input.GetAxis("Mouse X") * mouseSensitity;
        CamPos.Set(0f, 0.4f, -2.3f);
        mouseX = Quaternion.Euler(0f, transform.rotation.y, 0f).y;
    }
    private void Update()
    {
        AltFirst = Input.GetKeyDown(KeyCode.LeftAlt);
        AltInput = Input.GetKey(KeyCode.LeftAlt);
    }
    private void FixedUpdate()
    {
        desiredPosition = LookTarget.transform.TransformPoint(CamPos);
        //desiredRotation = Quaternion.LookRotation(LookTarget.transform.position - transform.position);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
        if(AltFirst)
        {
            
        }

        if(AltInput)
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSensitity * Time.deltaTime;

            transform.rotation = Quaternion.Euler(0f, mouseX, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, )
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(LookTarget.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
        }

    }
}
