using System;
using System.Collections.Generic;
using Impingement.Control;
using Impingement.Dungeon;
using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

namespace Impingement.Playfab
{
    public class PlayfabManager : MonoBehaviour
    {
        public event Action<bool> ValueSyncedAndConnected = delegate(bool b) {  };

        public bool IsForceQuit
        {
            get => _isForceQuit;
            set
            {
                if (value != _isForceQuit)
                {
                    UploadData(new Dictionary<string, string>
                    {
                        {"ForceQuit", value.ToString()}
                    });
                }

                _isForceQuit = value;
            }
        }

        private bool _isForceQuit;

        private void OnApplicationQuit()
        {
            if (SceneManager.GetActiveScene().name == "Hideout" || SceneManager.GetActiveScene().name == "Dungeon")
            {
                FindObjectOfType<PlayfabPlayerDataController>().SavePlayerData();
            }
            
            if (SceneManager.GetActiveScene().name != "Dungeon") { return; }

            UploadData(new Dictionary<string, string>
            {
                {"ForceQuit", "true"}
            });
            FindObjectOfType<DungeonManager>().GenerateJson();
        }
        
        public void Login(string login)
        {
            var request = new LoginWithCustomIDRequest {CustomId = login, CreateAccount = true};
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
        
        public void UploadData(Dictionary<string, string> data)
        {
            var request = new UpdateUserDataRequest()
            {
                Data = data
            };
            PlayFabClientAPI.UpdateUserData(request, null, null);
        }

        public void LoadData(Action<GetUserDataResult> OnDataReceived)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, null);
        }
        
        public void UploadJson(string json)
        {

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    {"DungeonData", json}
                }
            };
            PlayFabClientAPI.UpdateUserData(request, null, null);
        }

        public void LoadJson(Action<GetUserDataResult> OnDataReceived)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, null);
        }
        
        
        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Login Success");
            LoadData(OnDataReceivedIsForceQuit);
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        
        private void OnDataReceivedIsForceQuit(GetUserDataResult result)
        {
            if (result.Data != null && result.Data.ContainsKey("ForceQuit"))
            {
                IsForceQuit = Convert.ToBoolean(result.Data["ForceQuit"].Value);
            }
            ValueSyncedAndConnected?.Invoke(IsForceQuit);
        }
    }
}