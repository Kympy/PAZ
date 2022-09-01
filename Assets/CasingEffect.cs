using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingEffect : MonoBehaviour
{
    private ParticleSystem casing;
    private void Awake()
    {
        casing = GetComponent<ParticleSystem>();
    }
    private void OnEnable()
    {
        casing.Play();
    }
}
