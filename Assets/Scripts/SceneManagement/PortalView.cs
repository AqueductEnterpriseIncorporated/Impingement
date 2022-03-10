using System.Collections.Generic;
using Impingement.Dungeon;
using Impingement.Saving;
using Impingement.Stats;
using Playfab;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class PortalView : MonoBehaviour, ISaveable
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
                SavePlayerData(other.gameObject);

            }
        }

        private void ManageSceneChanging(GameObject player)
        {
            player.GetComponentInChildren<NavMeshAgent>().enabled = false;
            SceneManager.LoadSceneAsync("Dungeon");
            player.GetComponentInChildren<NavMeshAgent>().enabled = true;
        }

        private void SavePlayerData(GameObject player)
        {
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"Experience", player.GetComponent<ExperienceController>().GetExperiencePoints().ToString()}
            });
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            transform.position = ((SerializableVector3) state).ToVector();
        }
    }
}