using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    public class RoomTemplates : MonoBehaviour
    {
        public RoomScriptableObject[] RoomVariants;
        public List<GameObject> Rooms = new List<GameObject>();
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private float _waitTime = 2f;
        [SerializeField] private GameObject[] _bottomRooms;
        [SerializeField] private GameObject[] _topRooms;
        [SerializeField] private GameObject[] _leftRooms;
        [SerializeField] private GameObject[] _rightRooms;
        [SerializeField] private GameObject _closedRoom;
        private bool _isBossSpawned;

        private void Update()
        {
            if(_isBossSpawned) { return; }
            if (_waitTime <= 0)
            {
                PhotonNetwork.Instantiate(_bossPrefab.name, Rooms[Rooms.Count - 1].transform.position, Quaternion.identity);
                _isBossSpawned = true;
            }
            else
            {
                _waitTime -= Time.deltaTime;
            }
        }

        public GameObject[] BottomRooms => _bottomRooms;

        public GameObject[] TopRooms => _topRooms;

        public GameObject[] LeftRooms => _leftRooms;

        public GameObject[] RightRooms => _rightRooms;

        public GameObject ClosedRoom => _closedRoom;
    }
}