using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ResourceDataObj", menuName = "ScriptableObjects/ResourceDataObj", order = 1)]
public class ResourceDataObj : Singleton<ResourceDataObj>
{
    public GameObject Player = null;

    public GameObject NormalZombie = null;
    public GameObject GhoulZombie = null;
    public GameObject FatZombie = null;

    public GameObject Blood = null;

    public GameObject Muzzle = null;

    public GameObject HealthPack = null;

    public override void Awake()
    {
        base.Awake();
        Player = Resources.Load<GameObject>("PlayerPrefab/Player");
        NormalZombie = Resources.Load<GameObject>("NormalZombiePrefab/NormalZombie");

        Blood = Resources.Load<GameObject>("Blood");
    }
}
