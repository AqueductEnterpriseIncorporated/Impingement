using System;
using System.Collections.Generic;
using Photon.Pun;
using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

namespace Impingement.Playfab
{
    public class PlayfabManager : MonoBehaviour
    {
        public event Action<bool> ValueSyncedAndConnected = delegate(bool b) {  };

        public bool DungeonIsSaved
        {
            get => _dungeonIsSaved;
            set
            {
                if (value != _dungeonIsSaved)
                {
                    UploadData(new Dictionary<string, string>
                    {
                        {"DungeonIsSaved", value.ToString()}
                    });
                }

                _dungeonIsSaved = value;
            }
        }
        
        private bool _dungeonIsSaved;
        private string _nickName;

        private void OnApplicationQuit()
        {
            if (SceneManager.GetActiveScene().name == "Hideout" || SceneManager.GetActiveScene().name == "Dungeon")
            {
                FindObjectOfType<PlayfabPlayerDataController>().SavePlayerData();
            }
            
            // if (SceneManager.GetActiveScene().name != "Dungeon") { return; }
            //
            // UploadData(new Dictionary<string, string>
            // {
            //     {"DungeonIsSaved", "true"}
            // });
            // FindObjectOfType<DungeonManager>().GenerateJson();
        }
        
        public void Login(string login)
        {
            var request = new LoginWithCustomIDRequest {CustomId = login, CreateAccount = true};
            _nickName = login;
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
        
        public void LoadData(string playFabId, Action<GetUserDataResult> OnDataReceived)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(){PlayFabId = playFabId}, OnDataReceived, null);
        }
        
        public void UploadJson(string name, string json)
        {
            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    {name, json}
                }
            };
            PlayFabClientAPI.UpdateUserData(request, null, null);
        }

        public void LoadJson(Action<GetUserDataResult> OnDataReceived)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, null);
        }

        public void SendReport(string bugTheme, string bugMessage)
        {
            Debug.Log("Sending report");
            var request = new ExecuteCloudScriptRequest()
            {
                FunctionName = "sendReport",
                FunctionParameter = new
                {
                    theme = bugTheme,
                    message = bugMessage
                }
            };
            
            PlayFabClientAPI.ExecuteCloudScript(request, OnReportSuccess, OnReportFailure);
        }

        private void OnReportFailure(PlayFabError error)
        {            
            Debug.LogError("Report error: " + error.GenerateErrorReport());

        }

        private void OnReportSuccess(ExecuteCloudScriptResult obj)
        {
            Debug.Log("Report Success");
        }

        private void OnLoginSuccess(LoginResult result)
        {
            Debug.Log("Login Success");
            PhotonNetwork.NickName = _nickName;
            LoadData(OnDataReceivedDungeonIsSaved);
        }

        private void OnLoginFailure(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
        
        private void OnDataReceivedDungeonIsSaved(GetUserDataResult result)
        {
            if (result.Data != null && result.Data.ContainsKey("DungeonIsSaved"))
            {
                DungeonIsSaved = Convert.ToBoolean(result.Data["DungeonIsSaved"].Value);
            }
            ValueSyncedAndConnected?.Invoke(DungeonIsSaved);
        }
    }
}