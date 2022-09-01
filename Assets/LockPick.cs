using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : MonoBehaviour
{
    private GameObject keyBody = null;
    private GameObject key = null;

    private int answer = 0;
    private float mouseX = 0f;
    private float rotationZ = 0f;
    private void Awake()
    {
        keyBody = GameObject.Find("KeyBody");
        key = GameObject.Find("Key");

        key.transform.rotation = Quaternion.identity;
        answer = Random.Range(-90, 90);
        rotationZ = keyBody.transform.eulerAngles.z;
    }
    private void Update()
    {
        mouseX -= Input.GetAxis("Mouse X");
        key.transform.eulerAngles = new Vector3(0f, 0f, mouseX);

        if(Input.GetKey(KeyCode.D))
        {
            rotationZ = keyBody.transform.eulerAngles.z;
            rotationZ = rotationZ > 180 ? rotationZ - 360 : rotationZ;

            // Check Answer - current key degree - > 180
            // Change 90 degree -> Answer / 2  - current / 2
            // I can rotate 90 - (Answer - current) degree
            if(rotationZ < -90f)
            {
                return;
            }
            else keyBody.transform.Rotate(0f, 0f, -240f * Time.deltaTime);
        }
        else
        {
            rotationZ = keyBody.transform.eulerAngles.z;
            rotationZ = rotationZ > 180 ? rotationZ - 360 : rotationZ;

            if (rotationZ > 0f)
            {
                return;
            }
            else
            {
                keyBody.transform.Rotate(0f, 0f, 280f * Time.deltaTime);
            }
        }
    }
}
