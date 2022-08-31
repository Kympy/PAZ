using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxNormalControl : MonoBehaviour
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
                EffectPool.Instance.ReturnEffect(this.gameObject, "Normal");
                yield break;
            }
            yield return null;
        }
    }
}
