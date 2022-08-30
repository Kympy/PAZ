using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceDataObj", menuName = "ScriptableObjects/ResourceDataObj", order = 1)]
public class ResourceDataObj : ScriptableObject
{
    private static ResourceDataObj _instance = null;
    public static ResourceDataObj Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = ScriptableObject.CreateInstance<ResourceDataObj>();
            }
            return _instance;
        }
    }
    public GameObject Player = null;

    public GameObject NormalZombie = null;
    public GameObject GhoulZombie = null;
    public GameObject FatZombie = null;

    public GameObject Blood = null;

    public GameObject Muzzle = null;

    public GameObject HealthPack = null;

    private void Awake()
    {
        Player = Resources.Load<GameObject>("PlayerPrefab/Player");
        NormalZombie = Resources.Load<GameObject>("NormalZombiePrefab/NormalZombie");

        Blood = Resources.Load<GameObject>("Blood");
    }
}
