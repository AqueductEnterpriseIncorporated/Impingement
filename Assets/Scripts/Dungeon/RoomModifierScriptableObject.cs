using Photon.Pun;
using UnityEngine;

namespace Impingement.Dungeon
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "Room Modifiers/New modifier", order = 1)]
    public class RoomModifierScriptableObject : ScriptableObject
    {
        public float ChanceToSpawn => _chanceToSpawn;
        [Header("SpawnPlayer objects")]
        [Tooltip("-1 as no limit")]
        public float MaximumModifierSpawns = -1;

        [SerializeField] private string _itemFolder = "Resources/ItemPickups/";
        [SerializeField] private GameObject[] _spawnPrefabs;
        [SerializeField] private int _minimumObjectSpawns;
        [SerializeField] private int _maximumObjectSpawns;
        [SerializeField] private bool _isEnemy;
        [SerializeField] private bool _isPickup;

        [Header("Modifier's spawn chance")]
        [Range(0,1)]
        [SerializeField] private float _chanceToSpawn = .5f;

        private RoomBehaviour _room;
        private DungeonManager _dungeonManager;

        public void SetDungeonManager(DungeonManager dungeonManager)
        {
            _dungeonManager = dungeonManager;
        }

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
                var randomPrefab = _spawnPrefabs[randomPrefabNumber];
                _room.RandomlyGeneratedObjectPrefabNamesList.Add(randomPrefab);
                

                if (_isEnemy)
                {
                    var localPrefab = Instantiate(randomPrefab,
                        GetRandomPosition(), Quaternion.identity);
                    _dungeonManager.Enemies.Add(localPrefab);
                }
                if (_isPickup)
                {
                    var localPrefab = Instantiate(randomPrefab,
                        _room.ItemSpawnPoint.position, Quaternion.identity);
                    _dungeonManager.Pikcups.Add(localPrefab);
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

                if (_isEnemy)
                {
                    var localPrefab = PhotonNetwork.Instantiate("RoomSpawns/" + randomPrefab,
                        GetRandomPosition(), Quaternion.identity);
                    _dungeonManager.Enemies.Add(localPrefab);
                }
                if (_isPickup)
                {
                    var localPrefab = PhotonNetwork.Instantiate("RoomSpawns/" + randomPrefab,
                        _room.ItemSpawnPoint.position, Quaternion.identity);
                    _dungeonManager.Pikcups.Add(localPrefab);
                }

                currentObjectCount++;
            }
        }
        
        private Vector3 GetRandomPosition()
        {
            Vector3 cubeSize;
            Vector3 cubeCenter;
            Transform cubeTrans = _room.BoxCollider.transform;
            cubeCenter = cubeTrans.position;
            cubeSize.x = cubeTrans.localScale.x * _room.BoxCollider.size.x;
            cubeSize.z = cubeTrans.localScale.z * _room.BoxCollider.size.z;

            Vector3 randomPosition = new Vector3(Random.Range(-cubeSize.x / 2, cubeSize.x / 2),0, Random.Range(-cubeSize.z / 2, cubeSize.z / 2));
            return cubeCenter + randomPosition;
        }
    }
}