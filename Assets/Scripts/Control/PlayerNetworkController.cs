using ExitGames.Client.Photon;
using Impingement.Attributes;
using Impingement.Playfab;
using Impingement.Serialization.SerializationClasses;
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
            if(SceneManager.GetActiveScene().buildIndex != 3){ return; }
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

        public void SelfDestroy()
        {
            if (PhotonNetwork.InRoom)
            {
                _photonView.RPC(nameof(RPCSelfDestroy), RpcTarget.OthersBuffered);
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            print("player are left...");
            foreach (var playerController in FindObjectsOfType<PlayerController>())
            {
                if (playerController.GetHealthController().CharacterName == otherPlayer.NickName)
                {
                    print(playerController.GetHealthController().CharacterName);
                    Destroy(playerController.gameObject.transform.parent.gameObject);
                    return;
                }
            }
        }

        [PunRPC]
        private void RPCSelfDestroy()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
            }
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

        // public override void OnConnectedToMaster()
        // {
        //     base.OnConnectedToMaster();
        //     Hashtable property = new Hashtable();
        //     property.Add("PlayfabId", FindObjectOfType<PlayfabManager>().MyPlayfabId);
        //     PhotonNetwork.LocalPlayer.SetCustomProperties(property);
        //     Debug.Log("Setting custom property");
        // }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("New player");

            var newPlayerPlayfabId = PhotonNetwork.LocalPlayer.CustomProperties["PlayfabId"];
            //FindObjectOfType<PlayfabPlayerDataController>().GetOtherPLayerData(newPlayerPlayfabId.ToString());
            Debug.Log("New Player PlayfabId:" + newPlayerPlayfabId);

            
            _playersPanel.PanelParent.SetActive(true);
            foreach (var playerController in FindObjectsOfType<PlayerController>())
            {
                if (playerController.GetPhotonView().ControllerActorNr == newPlayer.ActorNumber)
                {
                    playerController.GetHealthController().CharacterName = newPlayer.NickName;;
                }
            }
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