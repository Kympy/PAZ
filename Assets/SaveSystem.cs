using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
	public struct SaveData // Data which i will save
    {
		public float lastPosX;
		public float lastPosY;
		public float lastPosZ;
		public int currentBullet;
		public int haveBullet;
		public float currentHP;
		public int itemCount;
		public int keyCount;
		public bool hasGun;
    }
	private static string SavePath => Application.persistentDataPath + "/Saves/";

	public static void Save(SaveData saveData, string saveFileName)
	{
		if (Directory.Exists(SavePath) == false) // There's no directory
		{
			Directory.CreateDirectory(SavePath);
		}

		string saveJson = JsonUtility.ToJson(saveData); // Text to Json

		string saveFilePath = SavePath + saveFileName + ".json";
		File.WriteAllText(saveFilePath, saveJson);
		Debug.Log("Save Success: " + saveFilePath);
	}

	public static SaveData? Load(string saveFileName)
	{
		string saveFilePath = SavePath + saveFileName + ".json";

		if (File.Exists(saveFilePath) == false) // There's no file
		{
			Debug.LogError("No such saveFile exists");
			SaveData? save = null;
			return save;
		}
		// Json to text
		string saveFile = File.ReadAllText(saveFilePath);
		SaveData saveData = JsonUtility.FromJson<SaveData>(saveFile);
		return saveData;
	}
}
