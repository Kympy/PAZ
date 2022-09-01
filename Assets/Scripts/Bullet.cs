using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 desired;
    private void OnEnable()
    {
        desired = transform.position;
        Invoke(nameof(DestroyThis), 0.5f);
    }
    private void FixedUpdate()
    {
        transform.Translate(50f * Time.deltaTime * Vector3.forward);
    }
    private void DestroyThis()
    {
        BulletPool.Instance.ReturnBullet(this);
    }
}
