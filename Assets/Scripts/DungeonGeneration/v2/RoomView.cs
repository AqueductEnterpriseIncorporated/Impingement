using System;
using Impingement.Saving;
using Photon.Pun;
using RPG.Saving;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class RoomView : MonoBehaviour
    {
        public RoomScriptableObject _roomVariant;
        private DungeonManager _dungeonManager;
        public string RoomPrefabName { get; set; }

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
            _dungeonManager = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<DungeonManager>();
            _dungeonManager.Rooms.Add(gameObject);
            print("Room added");
        }

        public void StartRoomAction()
        {
            _roomVariant.RoomAction();
        }
    }
}