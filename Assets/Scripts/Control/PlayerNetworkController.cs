using System;
using Impingement.Attributes;
using Impingement.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.Control
{
    public class PlayerNetworkController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _hud;
        [SerializeField] private PlayersPanel _playersPanel;

        private PhotonView _photonView;
        
        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if(!_photonView.IsMine) { return; }

            GetComponent<HealthController>().CharacterName = PhotonNetwork.NickName;
        }

        private void FixedUpdate()
        {
            DisablePlayerObject();
        }
        
        public override void OnJoinedRoom()
        {
            photonView.RPC(nameof(RPCSyncPlayers), RpcTarget.AllBufferedViaServer);
            UpdatePlayersPanel();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (PhotonNetwork.InRoom)
            {
                SetupPlayer();
            }
        }

        [PunRPC]
        private void RPCSyncPlayers()
        {
            SetupPlayer();
        }

        private static void SetupPlayer()
        {
            var players = FindObjectsOfType<PlayerController>();

            foreach (var playerController in players)
            {
                if (!playerController.GetPhotonView().IsMine)
                {
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player.ActorNumber ==
                            playerController.GetPhotonView().ControllerActorNr)
                        {
                            playerController.GetHealthController().CharacterName = player.NickName;
                        }
                    }

                    // var id = playerController.GetPhotonView().Controller.NickName;
                    // playerController.GetPlayfabPlayerDataController().SetupPlayer(id);
                    // playerController._playfabManager.LoadData(id, OnDataReceived);
                }
            }
        }

        private void UpdatePlayersPanel()
        {
            foreach (var playerController in FindObjectsOfType<PlayerController>())
            {
                if (!playerController.GetPhotonView().IsMine)
                {
                    _playersPanel.AddPlayer(playerController);
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            _playersPanel.PanelParent.SetActive(true);
            foreach (var playerController in FindObjectsOfType<PlayerController>())
            {
                if (playerController.GetPhotonView().ControllerActorNr == newPlayer.ActorNumber)
                {
                    playerController.GetHealthController().CharacterName = newPlayer.NickName;;
                }
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(1);
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