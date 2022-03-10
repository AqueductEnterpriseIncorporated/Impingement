using System;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using PlayFab.ClientModels;

namespace Playfab
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

        public Dictionary<string, string> File { get; set; }
        public event Action<Dictionary<string, string>> FileLoaded = delegate(Dictionary<string, string> dictionary) {  };
        private string _entityId;
        private string _entityType;
        private string _activeUploadFileName;
        private readonly Dictionary<string, string> _entityFileJson = new Dictionary<string, string>();
        private bool _isForceQuit;
        const string _fileName = @"C:\Users\vadim\AppData\LocalLow\AqueductEnterpriseIncorporated\Impingement\save.sav";
        
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

        public void LoadFile()
        {
            
        }
        
        private void OnLoginSuccess(LoginResult result)
        {    
            _entityId = result.EntityToken.Entity.Id;
            _entityType = result.EntityToken.Entity.Type;
            Debug.Log("Login Success");

            //LoadData(OnDataReceivedPlayerExp);
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