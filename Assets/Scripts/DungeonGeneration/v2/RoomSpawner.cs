using System.Linq;
using Impingement.NavMesh;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Impingement.DungeonGeneration
{
    public class RoomSpawner : MonoBehaviour
    {
        [SerializeField] private int _openingDirection;
        [SerializeField] private Transform _roomParentTransform;
        private float _spawnDelay = 0.1f;
        private float _destroyDelay = 4f;
        private bool _isSpawned;
        private RoomTemplates _roomTemplates;

        private void Awake()
        {
            _roomTemplates = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<RoomTemplates>();
        }

        private void Start()
        {
            if(!PhotonNetwork.IsMasterClient) { return;}
            //ProcessRoomSpawning();
            //Destroy(gameObject);
            Invoke(nameof(ProcessRoomSpawning), _spawnDelay);
            Invoke(nameof(SelfDestroy), _destroyDelay);
            FindObjectOfType<NavigationBaker>().Bake();
        }

        private void SelfDestroy()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        private void ProcessRoomSpawning()
        {
            if (_isSpawned) { return; }

            switch (_openingDirection)
            {
                case 1:
                {
                    var rooms = _roomTemplates.BottomRooms;
                    SpawnRoom(rooms);
                    break;
                }
                case 2:
                {
                    var rooms = _roomTemplates.TopRooms;
                    SpawnRoom(rooms);
                    break;
                }
                case 3:
                {
                    var rooms = _roomTemplates.LeftRooms;
                    SpawnRoom(rooms);
                    break;
                }
                case 4:
                {
                    var rooms = _roomTemplates.RightRooms;
                    SpawnRoom(rooms);
                    break;
                }
            }

            _isSpawned = true;
        }

        private void SpawnRoom(GameObject[] rooms)
        {
            //get random room prefab and spawn
            var randomNumber = Random.Range(0, rooms.Length);
            var randomRoom = rooms[randomNumber];
            
            var spawnChanceValue = Random.Range(0f, 1f);
            var sortedRoomVariants = _roomTemplates.RoomVariants.OrderBy(x => x.ChanceToSpawn).ToList();

            var localRoomPrefab = PhotonNetwork.Instantiate(randomRoom.name, transform.position, Quaternion.identity);

            //check each chance to spawn room type 
            foreach (var roomVariant in sortedRoomVariants)
            {
                //room variant spawn  chances:
                //itemsRare - 1% (ChanceToSpawn)
                //items - 10% (ChanceToSpawn)
                //enemy - 100% (ChanceToSpawn)


                //      0.1             10
                //      0.05             0.01
                if (spawnChanceValue <= roomVariant.ChanceToSpawn)
                {
                    var roomVariantsWithSameChanceToSpawn =
                        sortedRoomVariants.FindAll(element => element.ChanceToSpawn == roomVariant.ChanceToSpawn);
                    var pickedRoomVariant =
                        roomVariantsWithSameChanceToSpawn[Random.Range(0, roomVariantsWithSameChanceToSpawn.Count)];
                    
                    //check room's type spawn limit
                    if (pickedRoomVariant.MaximumRoomSpawns > -1)
                    {
                        int spawnCount = 0;
                        foreach (var spawnedRooms in _roomTemplates.Rooms)
                        {
                            if (spawnedRooms.GetComponent<RoomView>()._roomVariant == pickedRoomVariant)
                            {
                                spawnCount++;
                            }
                        }
                        
                        if (spawnCount >= pickedRoomVariant.MaximumRoomSpawns)
                        {
                            break;
                            //break;
                        }
                    }
                    
                    //spawn room type
                    pickedRoomVariant.SetRoom(localRoomPrefab.GetComponent<RoomView>().gameObject);
                    localRoomPrefab.GetComponent<RoomView>().RoomVariant = pickedRoomVariant;
                    localRoomPrefab.GetComponent<RoomView>().StartRoomAction();
                        
                    break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EntryRoom"))
            {
                PhotonNetwork.Destroy(gameObject);
                return;
            }
            
            if (other.CompareTag("SpawnPoint"))
            {
                if (!other.GetComponent<RoomSpawner>()._isSpawned && !_isSpawned)
                {
                    PhotonNetwork.Instantiate(_roomTemplates.ClosedRoom.name, transform.position, Quaternion.identity);
                    PhotonNetwork.Destroy(gameObject);
                }

                _isSpawned = true;
            }
        }
    }
}