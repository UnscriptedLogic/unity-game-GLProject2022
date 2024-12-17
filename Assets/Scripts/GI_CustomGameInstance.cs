using System;
using Standalone;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class VolumeData
{
    public float master;
    public float bgm;
    public float towers;
    public float units;
    public float ui;
    public float other;
    
    public VolumeData()
    {
        master = 1;
        bgm = 1;
        towers = 1;
        units = 1;
        ui = 1;
        other = 1;
    }
}

public class SaveData
{
    public VolumeData volumeData;
    public float mouseSens;

    public SaveData()
    {
        volumeData = new VolumeData();
        mouseSens = 100f;
    }
}


public class GI_CustomGameInstance : UGameInstance
{
    [SerializeField] private TowerTreeSO allTowerListSO;
    [SerializeField] private List<TowerSO> selectedTowers = new List<TowerSO>();

    private SaveData saveData;
    private string loadedFolder;

    public SaveData SaveData => saveData;
    
    public TowerTreeSO AllTowerListSO => allTowerListSO;
    public List<TowerSO> SelectedTowers => selectedTowers;
    
    public static LayerMask DebriLayer => LayerMask.GetMask("Debri");

    public void SetSelectedTowers(List<TowerSO> towers)
    {
        selectedTowers.Clear();
        selectedTowers = new List<TowerSO>(towers);
    }
    
    protected override void Awake()
    {
        base.Awake();

        Application.wantsToQuit += WantsToQuit;
    }

    public void LoadGame<T>(string folder) where T : SaveData
    {
        loadedFolder = folder;
        if (!File.Exists(Application.persistentDataPath + $"/{loadedFolder}/save.json"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + $"/{loadedFolder}");
            saveData = Activator.CreateInstance<T>();

            Debug.Log("Creating new save...");

            SaveGame();
            return;
        }


        string json = File.ReadAllText(Application.persistentDataPath + $"/{loadedFolder}/save.json");
        saveData = JsonUtility.FromJson<T>(json);
    }

    public void SaveGame()
    {
        if (loadedFolder == string.Empty)
        {
            return;
        }

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(Application.persistentDataPath + $"/{loadedFolder}/save.json", json);

        Debug.Log("Game saved!");
    }

    private bool WantsToQuit()
    {
        SaveGame();
        return true;
    }

    public void ClearSave()
    {
        if (loadedFolder == string.Empty)
        {
            return;
        }

        File.Delete(Application.persistentDataPath + $"/{loadedFolder}/save.json");
        saveData = Activator.CreateInstance<SaveData>() as SaveData;
        SaveGame();
    }
}
