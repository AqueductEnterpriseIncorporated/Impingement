using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

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
                var localPrefab = PhotonNetwork.Instantiate("RoomSpawns/" + _spawnPrefabs[Random.Range(0, _spawnPrefabs.Length)].name,
                    GetRandomPosition(), Quaternion.identity);
                FindObjectOfType<DungeonManager>().Enemies.Add(localPrefab);
                //localPrefab.transform.parent = _room.transform;
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

            Vector3 randomPosition = new Vector3(Random.Range(-cubeSize.x / 2, cubeSize.x / 2),0, Random.Range(-cubeSize.z / 2, cubeSize.z / 2));
            return cubeCenter + randomPosition;
        }
    }
}