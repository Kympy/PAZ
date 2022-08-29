using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ZombieData
{
    public string name;
    public int attackPower;
    public int MaxHP;
}

public class DataManager
{
    private static DataManager _inst;
    public static DataManager Inst
    {
        get
        {
            if(_inst == null)
            {
                _inst = new DataManager();
            }
            return _inst;
        }
    }
    private const string CRLF = "\r\n";
    private string[] tempColumns;

    private List<ZombieData> ZombieDataList = new List<ZombieData>();

    private ZombieData ZD = new ZombieData();

    public void LoadGameData()
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

            ZD.name = tempColumns[0];
            ZD.attackPower = int.Parse(tempColumns[1]);
            ZD.MaxHP = int.Parse(tempColumns[2]);

            ZombieDataList.Add(ZD);
        }
    }
    public ZombieData GetZombieData(string name)
    {
        return ZombieDataList.Find(ZombieData => ZombieData.name == name);
    }
    #endregion
}
