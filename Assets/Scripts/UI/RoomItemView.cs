using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomItemView : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _roomName;

    public void SetRoomName(string roomName)
    {
        _roomName.text = roomName;
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_roomName.text);

        //PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        //PhotonNetwork.JoinRoom(_roomName.text);
    }
}
