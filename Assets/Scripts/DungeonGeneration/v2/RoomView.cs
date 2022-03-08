using System;
using System.Collections.Generic;
using System.Linq;
using Impingement.enums;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class RoomView : MonoBehaviour
    {
        public enumRoomTypes[] RoomTypes;
        public List<enumRoomTypes> OpenedDirections;
        public List<enumRoomTypes> RoomTypesToClose = new List<enumRoomTypes>();
        public List<GameObject> SpawnPonts = new List<GameObject>();
        public List<RoomView> ConnectedRooms;
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
            OpenedDirections = RoomTypes.ToList();
            _dungeonManager = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<DungeonManager>();
            _dungeonManager.Rooms.Add(gameObject);
        }

        public void StartRoomAction()
        {
            _roomVariant.RoomAction();
        }
    }
}