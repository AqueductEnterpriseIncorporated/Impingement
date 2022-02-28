using System;
using Photon.Realtime;

namespace Impingement.PhotonScripts
{
    using Photon.Pun;
    using UnityEngine;

    public class ConnectToPhoton : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _loadScreen;
        [SerializeField] private string _defaultRoomName = "room";

        private void Start()
        {
            _loadScreen.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            _loadScreen.SetActive(false);
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            _defaultRoomName += Random.Range(0, 10);
        }

        public void CreateRoom()
        {
            PhotonNetwork.JoinOrCreateRoom("qwe", null, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("Scenes/SampleScene/SampleScene");
        }
    }
}