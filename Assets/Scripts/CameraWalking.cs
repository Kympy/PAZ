using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWalking : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up * 4f * Time.deltaTime);
    }
}
