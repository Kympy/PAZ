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
        ZombiePool.Instance.InitPool();
        EffectPool.Instance.InitPool();
    }
    public void StartGame()
    {
        UIManager.Instance.InitUI();
        
        Starting = GameObject.FindGameObjectWithTag("StartPosition").transform;
        Instantiate(ResourceDataObj.Instance.Player, Starting.position, Quaternion.identity);
        player1 = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (willLoadData)
        {
            player1.transform.position = new Vector3(loadedData.Value.lastPosX, loadedData.Value.lastPosY, loadedData.Value.lastPosZ);
            player1.currentHP = loadedData.Value.currentHP;
            UIManager.Instance.UpdateBar(player1.currentHP, player1.MaxHP);
            player1.bulletCount = loadedData.Value.currentBullet;
            player1.haveBulletCount = loadedData.Value.haveBullet;
            UIManager.Instance.SetBulletUI(player1.bulletCount, player1.haveBulletCount);
            player1.itemCount = loadedData.Value.itemCount;
            UIManager.Instance.UpdateItemCount(player1.itemCount);
            player1.keyCount = loadedData.Value.keyCount;
            player1.HasGun = loadedData.Value.hasGun;
            if(player1.HasGun)
            {
                player1.GunMode();
            }
        }
        else
        {
            UIManager.Instance.UpdateBar(player1.currentHP, player1.MaxHP);
            UIManager.Instance.SetBulletUI(player1.bulletCount, player1.haveBulletCount);
            UIManager.Instance.UpdateItemCount(player1.itemCount);
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
        saved.lastPosX = player1.transform.position.x;
        saved.lastPosY = player1.transform.position.y;
        saved.lastPosZ = player1.transform.position.z;
        SaveSystem.Save(saved, "SavedData");
    }
    public void GoToLoad()
    {
        loadedData = SaveSystem.Load("SavedData");
        //Debug.Log(loadedData.Value.lastPosX);
        //Debug.Log(loadedData.Value.lastPosY);
        //Debug.Log(loadedData.Value.lastPosZ);
        //Debug.Log(loadedData.Value.currentHP);
        //Debug.Log(loadedData.Value.haveBullet);
        //Debug.Log(loadedData.Value.hasGun);
        if(loadedData == null)
        {
            Debug.LogError("No data to load");
            return;
        }
        willLoadData = true;
        loadSceneName = "GameScene01";
        Time.timeScale = 1f;
        SceneManager.LoadScene("LoadingScene");
    }
    public void GoToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
    public void Test()
    {
        Time.timeScale = 1f;
        loadSceneName = "GameScene01";
        SceneManager.LoadScene("LoadingScene");
    }
}
