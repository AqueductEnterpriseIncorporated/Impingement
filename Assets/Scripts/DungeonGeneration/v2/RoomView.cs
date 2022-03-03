using System;
using Photon.Pun;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class RoomView : MonoBehaviour
    {
        public RoomScriptableObject _roomVariant;
        private RoomTemplates _roomTemplates;

        public RoomScriptableObject RoomVariant
        {
            get => _roomVariant;
            set
            {
                _roomVariant = value; 
            }
        }

        private void Awake()
        {
            _roomTemplates = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<RoomTemplates>();
            _roomTemplates.Rooms.Add(gameObject);
        }

        public void StartRoomAction()
        {
            if(!PhotonNetwork.IsMasterClient) { return;}

            _roomVariant.RoomAction();
        }
        
    }
}