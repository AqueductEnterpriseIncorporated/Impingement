using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Impingement.UI
{
    public class RoomListElementView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text _roomNameText;
        
        private string _roomName;
        
        public void SetRoomName(string roomName)
        {
            _roomName = roomName;
            _roomNameText.text = roomName;
        }

        public void JoinRoomButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRoom(_roomName);
        }
    }
}