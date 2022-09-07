using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : Singleton<EffectPool>
{
    private Queue<GameObject> normalEffect = new Queue<GameObject>();
    private Queue<GameObject> bloodEffect = new Queue<GameObject>();

    private int maxCount = 20;
    public override void Awake()
    {
        base.Awake();
    }
    public void InitPool()
    {
        Debug.LogWarning("@@ : EffectPool Awake");
        normalEffect.Clear();
        bloodEffect.Clear();

        GameObject temp = null;
        for (int i = 0; i < maxCount; i++)
        {
            temp = Instantiate(ResourceDataObj.Instance.BulletHit, transform);
            temp.gameObject.SetActive(false);
            temp.transform.SetParent(this.transform);

            normalEffect.Enqueue(temp);

            temp = Instantiate(ResourceDataObj.Instance.ChunkBlood, transform);
            temp.gameObject.SetActive(false);
            temp.transform.SetParent(this.transform);

            bloodEffect.Enqueue(temp);
        }
    }
    public GameObject GetEffect(string Name)
    {
        GameObject obj = null;

        if (Name == "Normal")
        {
            if (normalEffect.Count == 0)
            {
                obj = Instantiate(ResourceDataObj.Instance.BulletHit, transform.position, Quaternion.identity);
            }
            else
            {
                obj = normalEffect.Dequeue();
            }

            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
        }
        else if (Name == "Blood")
        {
            if (bloodEffect.Count == 0)
            {
                obj = Instantiate(ResourceDataObj.Instance.ChunkBlood, transform.position, Quaternion.identity);
            }
            else
            {
                obj = bloodEffect.Dequeue();
            }

            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
        }
        return obj;
    }
    public void ReturnEffect(GameObject obj, string name)
    {
        if(name == "Normal")
        {
            normalEffect.Enqueue(obj);
        }
        else if(name == "Blood")
        {
            bloodEffect.Enqueue(obj);
        }

        obj.transform.SetParent(this.transform);
        obj.gameObject.SetActive(false);
    }
}
