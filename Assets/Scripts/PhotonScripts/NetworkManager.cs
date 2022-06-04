using System;
using System.Collections.Generic;
using Impingement.Attributes;
using Impingement.Control;
using Impingement.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.PhotonScripts
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance;
        [SerializeField] private GameObject _playerPrfab;
        [SerializeField] private List<Transform> _spawnTransforms;

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
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                SpawnPlayer();
            }
        }

        public void SpawnPlayer()
        {
            if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Spawning player");

                for (int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    var player = PhotonNetwork.PlayerList[i];
                    if (!player.IsLocal)
                    {
                        continue;
                    }

                    PhotonNetwork.Instantiate("player/" + _playerPrfab.name,
                        _spawnTransforms.Count == 0
                            ? GameObject.FindGameObjectWithTag("PlayerSpawnPoint" + i).transform.position
                            : _spawnTransforms[i].position,
                        Quaternion.identity);
                }
            }
            else if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                Instantiate(_playerPrfab,
                    _spawnTransforms.Count == 0
                        ? GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform.position
                        : _spawnTransforms[0].position,
                    Quaternion.identity);
            }
        }
    }
}