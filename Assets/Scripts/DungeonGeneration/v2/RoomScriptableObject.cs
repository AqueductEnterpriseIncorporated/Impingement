using Photon.Pun;
using UnityEngine;

namespace Impingement.DungeonGeneration
{
    [CreateAssetMenu(fileName = "Room", menuName = "Rooms/New room", order = 1)]
    public class RoomScriptableObject : ScriptableObject
    {
        public float ChanceToSpawn => _chanceToSpawn;

        [Header("Spawn objects")]
        [Tooltip("-1 as no limit")]
        public float MaximumRoomSpawns = -1;
        [SerializeField] private GameObject[] _spawnPrefabs;
        [SerializeField] private int _minimumSpawns;
        [SerializeField] private int _maximumSpawns;
        // [Header("SpawnPosition")]
        // [SerializeField] private float _mininmumX;
        // [SerializeField] private float _maximumX;
        // [SerializeField] private float _mininmumZ;
        // [SerializeField] private float _maximumZ;
        // [SerializeField] private float _y;
        [Header("Room chance spawn")]
        [Range(0,1)]
        [SerializeField] private float _chanceToSpawn = .5f;
        private GameObject _room;

        public void SetRoom(GameObject room)
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
        
        private void Spawn()
        {
            int currentEnemyCount = 0;
            int spawnAmount = Random.Range(_minimumSpawns, _maximumSpawns);
            while (currentEnemyCount < spawnAmount)
            {
                var localPrefab = PhotonNetwork.Instantiate(_spawnPrefabs[Random.Range(0, _spawnPrefabs.Length)].name,
                    GetRandomPosition(), Quaternion.identity);
                localPrefab.transform.parent = _room.transform;
                //localPrefab.transform.position = GetRandomPosition();
                currentEnemyCount++;
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

            Vector3 randomPosition = new Vector3(Random.Range(-cubeSize.x / 3, cubeSize.x / 3),0, Random.Range(-cubeSize.z / 3, cubeSize.z / 3));
            return cubeCenter + randomPosition;
        }
    }
}