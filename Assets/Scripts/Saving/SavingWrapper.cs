using System.Collections.Generic;
using Impingement.Saving;
using Playfab;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if(SceneManager.GetActiveScene().name != "Dungeon2") { return; }
        FindObjectOfType<PlayfabManager>().UploadData(new Dictionary<string, string>
        {
            {"ForceQuit", "true"}
        });
        Save();
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
