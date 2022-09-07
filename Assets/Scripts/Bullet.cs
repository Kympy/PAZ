using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour // Only Use For Effect
{
    private void OnEnable()
    {
        Invoke(nameof(DestroyThis), 0.5f); // Destroy this after 0.5 seconds
    }
    private void FixedUpdate()
    {
        transform.Translate(50f * Time.deltaTime * Vector3.forward); // Fly forward
    }
    private void DestroyThis()
    {
        BulletPool.Instance.ReturnBullet(this); // Return bullet to the pool
    }
}
