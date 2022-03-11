using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Dungeon
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "Room Modifiers/New modifier", order = 1)]
    public class RoomModifierScriptableObject : ScriptableObject
    {
        public float ChanceToSpawn => _chanceToSpawn;
        [Header("Spawn objects")]
        [Tooltip("-1 as no limit")]
        public float MaximumModifierSpawns = -1;


        [SerializeField] private GameObject[] _spawnPrefabs;
        [SerializeField] private int _minimumObjectSpawns;
        [SerializeField] private int _maximumObjectSpawns;
        [SerializeField] private bool _isEnemy;
        [SerializeField] private bool _isPickup;

        [Header("Modifier's spawn chance")]
        [Range(0,1)]
        [SerializeField] private float _chanceToSpawn = .5f;

        private RoomBehaviour _room;

        public void SetRoom(RoomBehaviour room)
        {
            _room = room;
        }
        
        public void RoomAction()
        {
            if (_spawnPrefabs != null)
            {
                Spawn();
            }
        }
        
        public void SetRoomAction()
        {
            if (_spawnPrefabs != null)
            {
                Load();
            }
        }
        
        private void Spawn()
        {
            int currentObjectCount = 0;
            _room.RandomlyGeneratedObjectSpawnsAmount = Random.Range(_minimumObjectSpawns, _maximumObjectSpawns);
            while (currentObjectCount < _room.RandomlyGeneratedObjectSpawnsAmount)
            {
                var randomPrefabNumber = Random.Range(0, _spawnPrefabs.Length);
                var randomPrefab = _spawnPrefabs[randomPrefabNumber].name;
                _room.RandomlyGeneratedObjectPrefabNamesList.Add(randomPrefab);
                var localPrefab = PhotonNetwork.Instantiate("RoomSpawns/" + randomPrefab,
                    GetRandomPosition(), Quaternion.identity);

                if (_isEnemy)
                {
                    FindObjectOfType<DungeonManager>().Enemies.Add(localPrefab);
                }
                if (_isPickup)
                {
                    FindObjectOfType<DungeonManager>().Pikcups.Add(localPrefab);
                }

                //localPrefab.transform.parent = _room.transform;
                currentObjectCount++;
            }
        }

        public void Load()
        {
            int currentObjectCount = 0;
            while (currentObjectCount < _room.RandomlyGeneratedObjectSpawnsAmount)
            {
                var randomPrefab = _room.RandomlyGeneratedObjectPrefabNamesList[currentObjectCount];
                var localPrefab = PhotonNetwork.Instantiate("RoomSpawns/" + randomPrefab,
                    GetRandomPosition(), Quaternion.identity);

                if (_isEnemy)
                {
                    FindObjectOfType<DungeonManager>().Enemies.Add(localPrefab);
                }
                if (_isPickup)
                {
                    FindObjectOfType<DungeonManager>().Pikcups.Add(localPrefab);
                }

                currentObjectCount++;
            }
        }
        
        private Vector3 GetRandomPosition()
        {
            _room.TryGetComponent<BoxCollider>(out var boxCollider);
            Vector3 cubeSize;
            Vector3 cubeCenter;
            Transform cubeTrans = boxCollider.GetComponent<Transform>();
            cubeCenter = cubeTrans.position;
            cubeSize.x = cubeTrans.localScale.x * boxCollider.size.x;
            cubeSize.z = cubeTrans.localScale.z * boxCollider.size.z;

            Vector3 randomPosition = new Vector3(Random.Range(-cubeSize.x / 2, cubeSize.x / 2),0, Random.Range(-cubeSize.z / 2, cubeSize.z / 2));
            return cubeCenter + randomPosition;
        }
    }
}