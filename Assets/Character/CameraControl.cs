using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameObject LookTarget;
    private float smoothSpeed = 20f;
    private Vector3 desiredPosition;
    private Quaternion desiredRotation;
    private Vector3 offset = new Vector3(0f, 0f, 0f);
    private float mouseRotation = 0f;
    private float mouseSensitity = 5f;

    private void Start()
    {
        LookTarget = GameObject.Find("LookPos");
        mouseRotation += Input.GetAxis("Mouse X") * mouseSensitity;
    }
    private void FixedUpdate()
    {
        desiredPosition = LookTarget.transform.TransformPoint(0f, 0.4f, -2.3f);
        desiredRotation = Quaternion.LookRotation(LookTarget.transform.position + offset - transform.position);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, smoothSpeed * Time.deltaTime);
    }
}
