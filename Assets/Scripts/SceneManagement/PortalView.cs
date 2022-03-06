using System.Collections.Generic;
using Impingement.DungeonGeneration;
using Impingement.Saving;
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
        private SavingWrapper _savingWrapper;
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
                if (SceneManager.GetActiveScene().name == "Dungeon2")
                {
                    FindObjectOfType<PlayfabManager>().IsForceQuit = false;
                    GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<DungeonManager>().IsDungBuilded = false;
                }
                other.GetComponentInChildren<NavMeshAgent>().enabled = false;
                SceneManager.LoadSceneAsync("Dungeon2"); 
                other.GetComponentInChildren<NavMeshAgent>().enabled = true;
            }
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