using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
    private ObjectPool<GameObject> pool = null;
    
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
        GameObject obj = Instantiate(ResourceDataObj.Instance.BulletHit);
        return obj;
    }
    // Activate Object
    public void GetObject(GameObject obj)
    {
        obj.SetActive(true);
    }
    // Deactivate Object
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
    // Destroy Object
    public void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }
}
