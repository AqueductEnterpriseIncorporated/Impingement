using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Impingement.Control
{
    public class PlayerNetworkController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _hud;
        private PhotonView _photonView;
        
        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void FixedUpdate()
        {
            DisablePlayerObject();
        }

        public bool IsHost()
        {
            return PhotonNetwork.IsMasterClient;
        }

        private void DisablePlayerObject()
        {
            if (!_photonView.IsMine)
            {
                _camera.gameObject.SetActive(false);
                _hud.gameObject.SetActive(false);
            }
        }
    }
}