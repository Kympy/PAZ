using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private string loadSceneName = ""; // Which scene will load?


    // Properties
    public string LoadSceneName { get { return loadSceneName; } set { loadSceneName = value; } }

    public override void Awake()
    {
        base.Awake();
        DataManager.Instance.Awake();
        ZombiePool.Instance.Awake();
    }
}
