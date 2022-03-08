using System;
using System.IO;
using Playfab;
using TMPro;
using UnityEngine.SceneManagement;

namespace Impingement.Core
{
    using UnityEngine;

    public class ConnectToPlayfab : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;

        private void Start()
        {
            FindObjectOfType<PlayfabManager>().ValueSyncedAndConnected += OnValueSyncedAndConnected;
        }

        private void OnValueSyncedAndConnected(bool value)
        {
            SceneManager.LoadSceneAsync("Hideout");
        }

        public void LoadHideout()
        {
            if (_inputField.text == null) { return; }
            FindObjectOfType<PlayfabManager>().Login(_inputField.text);
        }
    }
}