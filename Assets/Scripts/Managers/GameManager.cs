using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private string loadSceneName = ""; // Which scene will load?

    private PlayerController player1 = null;
    // Properties
    public PlayerController Player1 { get { return player1; } }
    public string LoadSceneName { get { return loadSceneName; } set { loadSceneName = value; } }

    public override void Awake()
    {
        base.Awake();
        DataManager.Instance.Awake();
        ZombiePool.Instance.Awake();
        EffectPool.Instance.Awake();
    }
}
