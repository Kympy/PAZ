using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : Singleton<ZombiePool>
{
    private Queue<ZombieBase> NormalPool = new Queue<ZombieBase>();
    
    private NormalZombie NormalPrefab = null;

    private int NormalMax = 10;
    public override void Awake()
    {
        Debug.LogWarning("@@ : Zombie Pool Awake");
        NormalPrefab = ResourceDataObj.Instance.NormalZombie.GetComponent<NormalZombie>();

        InitPool();
    }
    private void InitPool()
    {
        NormalPool.Clear();
        ZombieBase temp = null;
        for(int i = 0; i < NormalMax; i++)
        {
            temp = Instantiate(NormalPrefab, transform);
            temp.gameObject.SetActive(false);
            temp.transform.SetParent(this.transform);
            NormalPool.Enqueue(temp);
        }
    }
    public ZombieBase GetNormalZombie()
    {
        ZombieBase obj = null;

        if(NormalPool.Count == 0)
        {
            obj = Instantiate(NormalPrefab, transform);
        }
        else
        {
            obj = NormalPool.Dequeue();
        }

        obj.transform.SetParent(null, false);
        obj.gameObject.SetActive(true);

        return obj;
    }
    public void ReturnZombie(ZombieBase obj)
    {
        NormalPool.Enqueue(obj);
        obj.transform.SetParent(this.transform);
        obj.gameObject.SetActive(false);
    }
}
