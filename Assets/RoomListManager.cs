namespace Impingement.UI
{
    using System.Collections.Generic;
    using Photon.Pun;
    using Photon.Realtime;
    using TMPro;
    using UnityEngine;

    public class RoomListManager : MonoBehaviourPunCallbacks, ILobbyCallbacks
    {
        [SerializeField] private GameObject _roomListCanvas;
        [SerializeField] private RoomListPanelView _roomListPanelView;
        

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _roomListCanvas.SetActive(true);
            _roomListPanelView.SetRooms(roomList);
            _roomListCanvas.SetActive(false);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideList();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ShowList();
            }
        }
        
        private void ShowList()
        {
            _roomListCanvas.SetActive(true);
        }
        
        private void HideList()
        {
            _roomListCanvas.SetActive(false);
        }
    }

}