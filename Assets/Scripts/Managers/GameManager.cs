using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private string loadSceneName = ""; // Which scene will load?

    private PlayerController player1 = null;

    private Transform Starting = null;
    public int keyCount = 0;
    public bool willLoadData = false;
    public SaveSystem.SaveData? loadedData = new SaveSystem.SaveData();
    // Properties
    public PlayerController Player1 { get { return player1; } }
    public string LoadSceneName { get { return loadSceneName; } set { loadSceneName = value; } }

    public override void Awake()
    {
        base.Awake();
    }
    public void StartGame()
    {
        Starting = GameObject.FindGameObjectWithTag("StartPosition").transform;
        Instantiate(ResourceDataObj.Instance.Player, Starting.position, Quaternion.identity);
        player1 = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (willLoadData)
        {
            player1.transform.position = loadedData.Value.lastPos.position;
            player1.transform.rotation = loadedData.Value.lastPos.rotation;
            player1.currentHP = loadedData.Value.currentHP;
            player1.bulletCount = loadedData.Value.currentBullet;
            player1.haveBulletCount = loadedData.Value.haveBullet;
            player1.keyCount = loadedData.Value.keyCount;
        }
    }
    public void GoToSave()
    {
        SaveSystem.SaveData saved = new SaveSystem.SaveData();
        saved.currentBullet = player1.bulletCount;
        saved.haveBullet = player1.haveBulletCount;
        saved.hasGun = player1.HasGun;
        saved.currentHP = player1.currentHP;
        saved.keyCount = player1.keyCount;
        saved.lastPos = player1.transform;
        SaveSystem.Save(saved, "SavedData");
    }
    public void GoToLoad()
    {
        loadedData = SaveSystem.Load("SavedData");
        if(loadedData == null)
        {
            Debug.LogError("No data to load");
            return;
        }
    }
    public void GoToMain()
    {
        loadSceneName = "GameScene01";
        SceneManager.LoadScene("LoadingScene");
    }
}
