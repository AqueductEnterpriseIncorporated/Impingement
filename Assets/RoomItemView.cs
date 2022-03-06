using Photon.Pun;
using TMPro;
using UnityEngine;

public class RoomItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;

    public void SetRoomName(string roomName)
    {
        _roomName.text = roomName;
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_roomName.text);
    }
}
