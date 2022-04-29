using Impingement.Control;
using Photon.Pun;
using UnityEngine;

namespace Impingement.PhotonScripts
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _playerPrfab;
        [SerializeField] private Transform _spawnTransform;

        private void Start()
        {
            //SpawnPlayer();
        }

        public void SpawnPlayer()
        {
            if (_spawnTransform == null)
            {
                _spawnTransform = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
            }

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork
                .Instantiate("player/" + _playerPrfab.name, _spawnTransform.position, Quaternion.identity);
        }
    }
}