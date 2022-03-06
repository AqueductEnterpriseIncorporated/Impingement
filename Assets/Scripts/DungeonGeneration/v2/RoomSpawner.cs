using System.Linq;
using Impingement.NavMesh;
using Photon.Pun;
using Playfab;
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
        private DungeonManager _dungeonManager;
        private bool _isForceQuit;

        private void Start()
        {
            _dungeonManager = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<DungeonManager>();
            if (_dungeonManager.IsDungBuilded)
            {
                return;
            }
            
            _isForceQuit = FindObjectOfType<PlayfabManager>().IsForceQuit;

            if (PhotonNetwork.IsMasterClient)
            {
                if (_isForceQuit)
                {
                    FindObjectOfType<SavingWrapper>().Load();
                    _dungeonManager.IsDungBuilded = true;
                }

                if (!_isForceQuit)
                {
                    Invoke(nameof(ProcessRoomSpawning), _spawnDelay);
                    Invoke(nameof(SelfDestroy), _destroyDelay);
                }
            }
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
                    var rooms = _dungeonManager.BottomRooms;
                    SpawnRoom(rooms);
                    break;
                }
                case 2:
                {
                    var rooms = _dungeonManager.TopRooms;
                    SpawnRoom(rooms);
                    break;
                }
                case 3:
                {
                    var rooms = _dungeonManager.LeftRooms;
                    SpawnRoom(rooms);
                    break;
                }
                case 4:
                {
                    var rooms = _dungeonManager.RightRooms;
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
            var sortedRoomVariants = _dungeonManager.RoomVariants.OrderBy(x => x.ChanceToSpawn).ToList();

            var localRoomPrefab = PhotonNetwork.Instantiate("Rooms/" + randomRoom.name, transform.position, Quaternion.identity);

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
                        foreach (var spawnedRooms in _dungeonManager.Rooms)
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
                    var localRoomView = localRoomPrefab.GetComponent<RoomView>();
                    var newRoomName = randomRoom.name.Replace("(Clone)", "");
                    localRoomView.RoomPrefabName = newRoomName;
                    localRoomView.RoomVariant = pickedRoomVariant;
                    localRoomView.StartRoomAction();
                        
                    break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _isForceQuit = FindObjectOfType<PlayfabManager>().IsForceQuit;
            if (_isForceQuit) { return; }
            print(_isForceQuit);
            print(2);
            
            if(!PhotonNetwork.IsMasterClient) { return; }
            
            if (other.CompareTag("EntryRoom"))
            {
                PhotonNetwork.Destroy(gameObject);
                return;
            }
            
            if (other.CompareTag("SpawnPoint"))
            {
                if (!other.GetComponent<RoomSpawner>()._isSpawned && !_isSpawned)
                {
                    _dungeonManager = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<DungeonManager>();
                    PhotonNetwork.Instantiate("Rooms/" + _dungeonManager.ClosedRoom.name, transform.position, Quaternion.identity);
                    PhotonNetwork.Destroy(gameObject);
                }
            
                _isSpawned = true;
            }
        }
    }
}