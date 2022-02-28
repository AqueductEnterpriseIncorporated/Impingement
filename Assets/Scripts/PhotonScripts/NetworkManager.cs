using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Impingement.PhotonScripts
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _playerPrfab;
        [SerializeField] private Transform _spawnTransform;
    
        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.Instantiate(_playerPrfab.name, _spawnTransform.position, Quaternion.identity);
        }
    }
}