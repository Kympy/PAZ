using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class, new()
{
    private static T _instance = null;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;

                lock(_lock)
                {
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
                        _instance = obj.GetComponent<T>();
                    }
                }
            }
            return _instance;
        }
    }
    public virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
