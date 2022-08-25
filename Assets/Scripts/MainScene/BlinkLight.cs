using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkLight : MonoBehaviour
{
    private Light myLight;
    private float myIntensity = 0f;
    private float maxIntensity;

    private bool Up = true;
    private void Awake()
    {
        myLight = GetComponent<Light>();
        maxIntensity = myLight.intensity;
    }
    private void Start()
    {
        StartCoroutine(Blink());
    }
    private IEnumerator Blink()
    {
        myLight.intensity = 0f;
        Up = true;
        while (true)
        {
            if(Up)
            {
                myIntensity += 0.05f;
                if (myIntensity > maxIntensity)
                {
                    myIntensity = maxIntensity;
                }
                myLight.intensity = myIntensity;
                if(myLight.intensity == maxIntensity)
                {
                    Up = false;
                }
            }
            else
            {
                myIntensity -= 0.05f;
                if(myIntensity < 0)
                {
                    myIntensity = 0f;
                }
                myLight.intensity = myIntensity;
                if(myLight.intensity == 0f)
                {
                    Up = true;
                }
            }
            yield return null;
        }
    }
}
