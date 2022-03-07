using System;
using System.Collections.Generic;
using System.Linq;
using Impingement.NavMesh;
using Impingement.Saving;
using Photon.Pun;
using RPG.Saving;
using SceneManagement;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class DungeonManager : MonoBehaviour, ISaveable
    {
        public RoomScriptableObject[] RoomVariants;
        public List<GameObject> Rooms = new List<GameObject>();
        public List<GameObject> Enemies = new List<GameObject>();
        public int CurrentMaxRooms { get; set; } = 10;
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private float _waitTime = 5f;
        [SerializeField] private GameObject[] _bottomRooms;
        [SerializeField] private GameObject[] _topRooms;
        [SerializeField] private GameObject[] _leftRooms;
        [SerializeField] private GameObject[] _rightRooms;
        [SerializeField] private GameObject _closedRoom;
        private bool _isBossSpawned;
        private bool _isDungBuilded;
        
        public bool IsDungBuilded
        {
            get => _isDungBuilded;
            set
            {
                if (value)
                {
                    SpawnBossAndCloseRooms();
                }

                _isDungBuilded = value;
            }
        }
        public GameObject[] BottomRooms => _bottomRooms;

        public GameObject[] TopRooms => _topRooms;

        public GameObject[] LeftRooms => _leftRooms;

        public GameObject[] RightRooms => _rightRooms;

        public GameObject ClosedRoom => _closedRoom;

        private void Update()
        {
            if (!IsDungBuilded)
            {
                return;
            }

            if (_isBossSpawned)
            {
                return;
            }

            //SpawnBossAndCloseRooms();
        }

        private void SpawnBossAndCloseRooms()
        {
            foreach (var room in Rooms.ToList())
            {
                var roomView = room.GetComponent<RoomView>();
                if (roomView.OpenedDirections.Count != 0)
                {
                    foreach (var spawnPont in roomView.SpawnPonts)
                    {
                        if (spawnPont.name == "SpawnPoint_" + roomView.OpenedDirections[0])
                        {
                            // print("spawning closed room at:" + spawnPont.gameObject.name + ", room: " + spawnPont.transform.parent.transform.parent.gameObject.name);
                            // var closedRoomPrefab = PhotonNetwork.Instantiate("Rooms/" + _closedRoom.name,
                            //     spawnPont.transform.position, Quaternion.identity);
                            // closedRoomPrefab.GetComponent<RoomView>().ConnectedRooms.Add(roomView);
                            // roomView.OpenedDirections.RemoveAt(0);
                            // break;
                        }
                    }

                    //print("possible opened room: " + room.gameObject.name + ", direction: ");
                    foreach (var openedDirection in roomView.OpenedDirections)
                    {
                        //print(openedDirection);
                    }
                }
            }

            PhotonNetwork.Instantiate(_bossPrefab.name, Rooms[Rooms.Count - 2].transform.position, Quaternion.identity);
            FindObjectOfType<NavigationBaker>().Bake();
            GameObject.Find("LoadPanel").SetActive(false);
            _isBossSpawned = true;
        }

        [Serializable]
        struct RoomsAndEnemyData
        {
            public List<SerializableVector3> RoomPositionList;
            public List<string> RoomNameList;
            public List<SerializableVector3> EnemyPositionList;
            public List<string> EnemyNameList;
        }

        public object CaptureState()
        {
            RoomsAndEnemyData roomsAndEnemyData = new RoomsAndEnemyData
            {
                RoomPositionList = new List<SerializableVector3>(), RoomNameList = new List<string>(),
                EnemyNameList = new List<string>(), EnemyPositionList = new List<SerializableVector3>()
            };
            foreach (var room in Rooms)
            {
                roomsAndEnemyData.RoomPositionList.Add(new SerializableVector3(room.transform.position));
                roomsAndEnemyData.RoomNameList.Add(room.GetComponent<RoomView>().RoomPrefabName);
            }
            foreach (var enemy in Enemies)
            {
                roomsAndEnemyData.EnemyPositionList.Add(new SerializableVector3(enemy.transform.position));
                var newName = enemy.gameObject.name.Replace("(Clone)", "");
                roomsAndEnemyData.EnemyNameList.Add(newName);
            }

            print("rooms and enemies captured");

            return roomsAndEnemyData;
        }

        public void RestoreState(object state)
        {
            RoomsAndEnemyData roomsAndEnemyData = (RoomsAndEnemyData) state;
            var roomNameList = roomsAndEnemyData.RoomNameList;
            var roomPositionList = roomsAndEnemyData.RoomPositionList;
            var enemyNameList = roomsAndEnemyData.EnemyNameList;
            var enemyPositionList = roomsAndEnemyData.EnemyPositionList;
            Rooms.Clear();
            for (int i = 0; i < roomNameList.Count; i++)
            {
                var localRoomPrefab = PhotonNetwork.Instantiate("Rooms/" + roomNameList[i], roomPositionList[i].ToVector(),
                    Quaternion.identity);
                Rooms.Add(localRoomPrefab);
            }
            
            for (int i = 0; i < enemyNameList.Count; i++)
            {
                var localRoomPrefab = PhotonNetwork.Instantiate("RoomSpawns/" + enemyNameList[i], enemyPositionList[i].ToVector(),
                    Quaternion.identity);
                Enemies.Add(localRoomPrefab);
            }
        }
    }
}