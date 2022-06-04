using Impingement.Control;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomItemView : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _roomName;
    private RoomInfo _room;

    public void SetupRoom(RoomInfo room)
    {
        _room = room;
        _roomName.text = string.Concat(room.Name, " ", room.PlayerCount, "/", room.MaxPlayers);
    }

    public void JoinRoom()
    {
        //FindObjectOfType<PlayerNetworkController>().SelfDestroy();
        PhotonNetwork.JoinRoom(_room.Name);
        //PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinRoom(_roomName.text);
    }
}
