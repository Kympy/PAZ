using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : Singleton<BulletPool>
{
    private Queue<Bullet> bulletPool = new Queue<Bullet>();

    private int max = 20;

    public override void Awake()
    {
        InitPool();
    }
    private void InitPool()
    {
        bulletPool.Clear();
        Bullet temp = null;
        for (int i = 0; i < max; i++)
        {
            temp = Instantiate(ResourceDataObj.Instance.Bullet.GetComponent<Bullet>(), transform);
            temp.gameObject.SetActive(false);
            temp.transform.SetParent(this.transform);
            bulletPool.Enqueue(temp);
        }
    }
    public Bullet GetBullet()
    {
        Bullet obj = null;

        if (bulletPool.Count == 0)
        {
            obj = Instantiate(ResourceDataObj.Instance.Bullet.GetComponent<Bullet>(), transform.position, Quaternion.identity);
        }
        else
        {
            obj = bulletPool.Dequeue();
        }

        obj.transform.SetParent(null);
        obj.gameObject.SetActive(true);

        return obj;
    }
    public void ReturnBullet(Bullet obj)
    {
        obj.transform.SetParent(this.transform);
        obj.gameObject.SetActive(false);
    }
}
