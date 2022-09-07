using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingText : MonoBehaviour // To give a text moving effect
{
    private RectTransform rect; // Text background's rect transform
    private Vector3 origin; // Text background's original position
    private void Awake()
    {
        rect = GetComponent<RectTransform>(); // Get
        origin = rect.transform.position; // Save origin
    }
    private void FixedUpdate() // Move X, Y Position in random range(+-10)
    {
        rect.transform.position = new Vector3(
            Random.Range(origin.x - 10, origin.x + 10), // X
            Random.Range(origin.y - 10, origin.y + 10), // Y
            0f); // Z
    }

}
