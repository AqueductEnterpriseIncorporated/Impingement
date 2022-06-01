using System;
using Impingement.Control;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.PhotonScripts
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance;
        [SerializeField] private GameObject _playerPrfab;
        [SerializeField] private Transform _spawnTransform;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        
        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        }

        private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (SceneManager.GetActiveScene().name == "Tests3")
            {
                SpawnPlayer();
            }
        }

        public void SpawnPlayer()
        {
            Debug.Log("Spawning player");
            if (_spawnTransform == null)
            {
                _spawnTransform = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
            }
            
            PhotonNetwork.Instantiate("player/" + _playerPrfab.name, _spawnTransform.position, Quaternion.identity);
            //PhotonNetwork.AutomaticallySyncScene = true;
            //throw new Exception("spawning");
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
        }
    }
}