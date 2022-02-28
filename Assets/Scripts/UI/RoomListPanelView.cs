using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Impingement.UI
{
    public sealed class RoomListPanelView : MonoBehaviour
    {
        [SerializeField] private RoomListElementView _roomListElementPrefab;
        [SerializeField] private Transform _elementsRoot;
        private readonly List<RoomListElementView> _roomListElements = new List<RoomListElementView>();

        public void SetRooms(List<RoomInfo> roomList)
        {
            var i = 0;
            for (; i < _roomListElements.Count && i < roomList.Count; ++i)
            {
                _roomListElements[i].SetRoomName(roomList[i].Name);
            }

            if (_roomListElements.Count < roomList.Count)
            {
                for (var j = i; j < roomList.Count; ++j)
                {
                    var newElement = Instantiate(_roomListElementPrefab, _elementsRoot);
                    newElement.SetRoomName(roomList[j].Name);
                    _roomListElements.Add(newElement);
                }
            }
            else if (_roomListElements.Count > roomList.Count)
            {
                for (var j = i; j < _roomListElements.Count; ++j)
                {
                    var roomListElement = _roomListElements[j];
                    _roomListElements.Remove(roomListElement);
                    Destroy(roomListElement.gameObject);
                }
            }
        }
    }
}