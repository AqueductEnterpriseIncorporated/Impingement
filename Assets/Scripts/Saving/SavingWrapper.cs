using System;
using System.Collections.Generic;
using Impingement.Attributes;
using Impingement.Dungeon;
using Impingement.Saving;
using Photon.Pun;
using Playfab;
using PlayFab.AdminModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using GetUserDataResult = PlayFab.ClientModels.GetUserDataResult;

public class SavingWrapper : MonoBehaviour
{
    private const string _defaultSaveFileName = "save";
    private SavingSystem _savingSystem;


    private void Start()
    {
        _savingSystem = GetComponent<SavingSystem>();
    }

    private void OnApplicationQuit()
    {
        if(SceneManager.GetActiveScene().name != "Dungeon") { return; }
        FindObjectOfType<PlayfabManager>().UploadData(new Dictionary<string, string>
        {
            {"ForceQuit", "true"}
        });
        FindObjectOfType<DungeonManager>().GenerateJson();
        //Save();
    }
    
    public void LoadLastScene()
    {
        _savingSystem.LoadLastScene(_defaultSaveFileName);
    }
    
    public void Save()
    {
        _savingSystem.Save(_defaultSaveFileName);
    }
    
    public void Load()
    {
        _savingSystem.Load(_defaultSaveFileName);
    }
}
