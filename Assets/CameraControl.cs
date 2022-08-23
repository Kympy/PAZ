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
    private float mouseRotation = 0f;
    private float mouseSensitity = 5f;

    private void Start()
    {
        LookTarget = GameObject.Find("LookPos");
        mouseRotation += Input.GetAxis("Mouse X") * mouseSensitity;
        CamPos.Set(0f, 0.4f, -2.3f);
    }
    private void FixedUpdate()
    {
        desiredPosition = LookTarget.transform.TransformPoint(CamPos);
        desiredRotation = Quaternion.LookRotation(LookTarget.transform.position - transform.position);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }
}
