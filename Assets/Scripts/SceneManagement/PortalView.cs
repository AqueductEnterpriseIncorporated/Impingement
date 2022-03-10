using System.Collections.Generic;
using Impingement.DungeonGeneration;
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
        private SavingWrapper _savingWrapper;
        private GameObject _incomingPlayer = null;
        //private NetworkManager _networkManager;

        private void Start()
        {
            _savingWrapper = FindObjectOfType<SavingWrapper>();
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
                _incomingPlayer = other.gameObject;
                ManageSceneChanging();
            }
        }

        private void ManageSceneChanging()
        {
            //save player's exp
            _playfabManager.UploadData(new Dictionary<string, string>()
            {
                {"Experience", _incomingPlayer.GetComponent<ExperienceController>().GetExperiencePoints().ToString()}
            });

            if (SceneManager.GetActiveScene().name == "Dungeon3")
            {
                _playfabManager.IsForceQuit = false;
            }

            _incomingPlayer.GetComponentInChildren<NavMeshAgent>().enabled = false;
            SceneManager.LoadSceneAsync("Dungeon");
            _incomingPlayer.GetComponentInChildren<NavMeshAgent>().enabled = true;
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