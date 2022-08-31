using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxControl : MonoBehaviour
{
    private ParticleSystem[] myParticle;
    private void Awake()
    {
        myParticle = GetComponentsInChildren<ParticleSystem>();
    }
    private void OnEnable()
    {
        foreach(ParticleSystem p in myParticle)
        {
            p.Play();
        }
        StartCoroutine(Check());
    }
    private IEnumerator Check()
    {
        while(true)
        {
            if(myParticle[0].isStopped == true)
            {
                EffectPool.Instance.ReturnEffect(this.gameObject, "Blood");
                yield break;
            }
            yield return null;
        }
    }
}
