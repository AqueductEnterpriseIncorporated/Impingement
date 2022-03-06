using System;
using System.Collections.Generic;
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
        public bool IsDungBuilded;
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private float _waitTime = 2f;
        [SerializeField] private GameObject[] _bottomRooms;
        [SerializeField] private GameObject[] _topRooms;
        [SerializeField] private GameObject[] _leftRooms;
        [SerializeField] private GameObject[] _rightRooms;
        [SerializeField] private GameObject _closedRoom;
        private bool _isBossSpawned;
        
        public GameObject[] BottomRooms => _bottomRooms;

        public GameObject[] TopRooms => _topRooms;

        public GameObject[] LeftRooms => _leftRooms;

        public GameObject[] RightRooms => _rightRooms;

        public GameObject ClosedRoom => _closedRoom;
        
        private void Update()
        {
            if(_isBossSpawned) { return; }
            if (_waitTime <= 0)
            {
                PhotonNetwork.Instantiate(_bossPrefab.name, Rooms[Rooms.Count - 1].transform.position, Quaternion.identity);
                GameObject.Find("LoadPanel").SetActive(false);
                _isBossSpawned = true;
            }
            else
            {
                _waitTime -= Time.deltaTime;
            }
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