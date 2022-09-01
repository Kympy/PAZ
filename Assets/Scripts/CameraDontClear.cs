using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDontClear : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Init();
    }
    private void Init()
    {
        cam.clearFlags = CameraClearFlags.Color;
    }
    private void OnPostRender()
    {
        cam.clearFlags = CameraClearFlags.Nothing;
    }
}
