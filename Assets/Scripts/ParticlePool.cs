using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
    private ObjectPool<GameObject> pool;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if(pool == null)
            {
                pool = new ObjectPool<GameObject>(Create, GetObject, ReturnObject, DestroyObject);
            }
            return pool;
        }
    }
    // Create Object
    private GameObject Create()
    {
        GameObject prefab = null;
        GameObject obj = Instantiate(prefab);
        return obj;
    }
    // Activate Object
    private void GetObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    // Deactivate Object
    private void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    // Destroy Object
    private void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }
}
