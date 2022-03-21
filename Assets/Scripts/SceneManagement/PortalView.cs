using System.Collections.Generic;
using Impingement.Combat;
using Impingement.Control;
using Impingement.Dungeon;
using Impingement.Playfab;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class PortalView : MonoBehaviour
    {
        [SerializeField] private bool _saveDungeonData;
        [SerializeField] private bool _savePlayerData;
        [SerializeField] private bool _completeLevelOnEnter;
        [SerializeField] private string _sceneToLoad = "";
        [SerializeField] private GameObject _loadPanel;
        private PlayfabManager _playfabManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Instantiate(_loadPanel);
                _playfabManager = FindObjectOfType<PlayfabManager>();
                
                if (_completeLevelOnEnter)
                {
                    FindObjectOfType<DungeonProgressionManager>().CompleteLevel();
                }
                
                _playfabManager.IsForceQuit = _saveDungeonData;

                ManageSceneChanging();
                if (_savePlayerData)
                {
                    other.GetComponent<PlayfabPlayerDataController>().SavePlayerData();
                }
            }
        }

        private void ManageSceneChanging()
        {
            SceneManager.LoadSceneAsync(_sceneToLoad);
        }
    }
}