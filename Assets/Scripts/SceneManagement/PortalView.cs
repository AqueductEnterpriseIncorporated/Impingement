using System.Collections.Generic;
using Impingement.Combat;
using Impingement.Control;
using Impingement.Dungeon;
using Impingement.Playfab;
using Impingement.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class PortalView : MonoBehaviour
    {
        [SerializeField] private string _sceneToLoad = "";
        [SerializeField] private GameObject _loadPanel;
        private PlayfabManager _playfabManager;
        private void Start()
        {
            //_networkManager = FindObjectOfType<NetworkManager>();
        }
        
        public void SetSceneToLoad(string sceneName)
        {
            _sceneToLoad = sceneName;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Instantiate(_loadPanel);
                _playfabManager = FindObjectOfType<PlayfabManager>();
                
                if (SceneManager.GetActiveScene().name == "Dungeon")
                {
                    FindObjectOfType<DungeonProgressionManager>().CompleteLevel();
                    _playfabManager.IsForceQuit = false;
                }
                
                ManageSceneChanging(other.gameObject);
                other.GetComponent<PlayfabPlayerDataController>().SavePlayerData();
            }
        }

        private void ManageSceneChanging(GameObject player)
        {
            SceneManager.LoadSceneAsync("Dungeon");
        }
    }
}