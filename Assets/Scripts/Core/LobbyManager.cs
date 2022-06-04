using System;
using Impingement.PhotonScripts;
using Photon.Pun;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
   // public event Action PlayerConntected;

    private void Start()
    {
        ConnectAndJoinLobby();
    }
    
    private void ConnectAndJoinLobby()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");

        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined lobby");
        //FindObjectOfType<NetworkManager>().SpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");

        if (!PhotonNetwork.IsMasterClient)
        {
            FindObjectOfType<NetworkManager>().SpawnPlayer();
        }
    }
}