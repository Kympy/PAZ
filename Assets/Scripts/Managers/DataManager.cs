using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZombieData
{
    public string Name;
    public float AttackPower;
    public float MaxHP;
    public float AttackRange;
    public float ViewRadius;
    public float ViewAngle;
    public float WalkSpeed;
    public float RunSpeed;
}

public class DataManager : Singleton<DataManager>
{
    private const string CRLF = "\r\n";
    private string[] tempColumns;

    private List<ZombieData> ZombieDataList = new List<ZombieData>();

    private ZombieData ZD = new ZombieData();

    public override void Awake()
    {
        Debug.LogWarning("@@ : Data Manager Awake");
        base.Awake();
        LoadGameData();
    }
    private void LoadGameData()
    {
        LoadZombieData();
    }
    #region Zombie Data
    public void LoadZombieData()
    {
        TextAsset asset = Resources.Load<TextAsset>("Data/ZombieData");
        string[] lines = asset.text.Split(CRLF);
        
        for(int i = 1; i < lines.Length - 1; i++)
        {
            tempColumns = lines[i].Split(',');

            ZD.Name = tempColumns[0];
            ZD.AttackPower = float.Parse(tempColumns[1]);
            ZD.MaxHP = float.Parse(tempColumns[2]);
            ZD.AttackRange = float.Parse(tempColumns[3]);
            ZD.ViewRadius = float.Parse(tempColumns[4]);
            ZD.ViewAngle = float.Parse(tempColumns[5]);
            ZD.WalkSpeed = float.Parse(tempColumns[6]);
            ZD.RunSpeed = float.Parse(tempColumns[7]);

            ZombieDataList.Add(ZD);
        }
    }
    public ZombieData GetZombieData(string name)
    {
        // Normal
        // Ghoul
        // Power
        return ZombieDataList.Find(ZombieData => ZombieData.Name == name);
    }
    #endregion
}
