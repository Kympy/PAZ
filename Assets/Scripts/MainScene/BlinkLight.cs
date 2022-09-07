using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkLight : MonoBehaviour // To blink light in the main scene
{
    private Light myLight;  // my light (Point light)
    private float myIntensity = 0f; // Intensity
    private float maxIntensity; // 1f

    private bool Up = true; // Up and down boolean
    private void Awake()
    {
        myLight = GetComponent<Light>();
        maxIntensity = myLight.intensity; // Max = current intensity = 1f
    }
    private void Start()
    {
        StartCoroutine(Blink()); // Start blink
    }
    private IEnumerator Blink()
    {
        myLight.intensity = 0f; // Start with turn off the light
        Up = true; // Go up intensity
        while (true)
        {
            if(Up) // Plus intensity
            {
                myIntensity += 0.05f; // Plus
                if (myIntensity > maxIntensity) // If intensity is max value,
                {
                    myIntensity = maxIntensity; // Set max value
                }
                myLight.intensity = myIntensity; // Set
                if(myLight.intensity == maxIntensity) // End Up intensity
                {
                    Up = false; // Now, start minus
                }
            }
            else // Minus intensity
            {
                myIntensity -= 0.05f;
                if(myIntensity < 0) // Less than min value
                {
                    myIntensity = 0f; // Set minimum
                }
                myLight.intensity = myIntensity;
                if(myLight.intensity == 0f)
                {
                    Up = true; // Now, start plus
                }
            }
            yield return null;
        }
    }
}
