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
            SceneManager.LoadScene("Hideout");
        }

        public void LoadHideout()
        {
            if (_inputField.text == null) { return; }
            
            FindObjectOfType<PlayfabManager>().Login(_inputField.text);

            
            
            // byte[] bytes = File.ReadAllBytes(@"C:\Users\vadim\AppData\LocalLow\AqueductEnterpriseIncorporated\Impingement\saveReaded3.sav");
            //
            // string base64 = Convert.ToBase64String(bytes);
            // byte[] bytes2 = Convert.FromBase64String(base64);
            //
            // File.WriteAllText(@"C:\Users\vadim\AppData\LocalLow\AqueductEnterpriseIncorporated\Impingement\saveText.sav", base64);
            // File.WriteAllBytes(@"C:\Users\vadim\AppData\LocalLow\AqueductEnterpriseIncorporated\Impingement\saveBytes2.sav", bytes2);
        }
    }
}