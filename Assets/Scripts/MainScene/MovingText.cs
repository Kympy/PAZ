using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingText : MonoBehaviour
{
    private RectTransform rect;
    private Vector3 origin;
    private Vector3 offsetY = new Vector3(0f, -30f, 0f);
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        origin = rect.transform.position;
    }
    private void FixedUpdate()
    {
        rect.transform.position = new Vector3(Random.Range(origin.x - 10, origin.x + 10), 
            Random.Range(origin.y - 10, origin.y + 10), 0f);
    }

}
