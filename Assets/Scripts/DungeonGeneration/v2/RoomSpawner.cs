using System.Collections.Generic;
using System.Linq;
using enums;
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
        [SerializeField] private RoomView _myRoomView;
        [SerializeField] private RoomView _spawningRoomView;
        [SerializeField] private enumRoomTypes _spawningRoomType;
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
                    if (_dungeonManager.Rooms.Count < _dungeonManager.CurrentMaxRooms)
                    {
                        Invoke(nameof(ProcessRoomSpawning), _spawnDelay);
                    }
                    else
                    {
                        print("End3");
                        _dungeonManager.IsDungBuilded = true;
                    }
                    //Invoke(nameof(SelfDestroy), _destroyDelay);
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
                    SpawnRoom(rooms, enumRoomTypes.B);
                    break;
                }
                case 2:
                {
                    var rooms = _dungeonManager.TopRooms;
                    SpawnRoom(rooms, enumRoomTypes.T);
                    break;
                }
                case 3:
                {
                    var rooms = _dungeonManager.LeftRooms;
                    SpawnRoom(rooms, enumRoomTypes.L);
                    break;
                }
                case 4:
                {
                    var rooms = _dungeonManager.RightRooms;
                    SpawnRoom(rooms, enumRoomTypes.R);
                    break;
                }
            }

            _isSpawned = true;
        }

        private void SpawnRoom(GameObject[] rooms, enumRoomTypes roomType)
        {
            _spawningRoomType = roomType;
            //get random room prefab and spawn
            var randomNumber = Random.Range(0, rooms.Length);
            var randomRoom = rooms[randomNumber];
            
            var spawnChanceValue = Random.Range(0f, 1f);
            var sortedRoomVariants = _dungeonManager.RoomVariants.OrderBy(x => x.ChanceToSpawn).ToList();

            var localRoomPrefab = PhotonNetwork.Instantiate("Rooms/" + randomRoom.name, transform.position, Quaternion.identity);
            _spawningRoomView = localRoomPrefab.GetComponent<RoomView>();
            var newRoomName = randomRoom.name.Replace("(Clone)", "");
            _spawningRoomView.RoomPrefabName = newRoomName;
            _myRoomView.ConnectedRooms.Add(_spawningRoomView);
            _spawningRoomView.ConnectedRooms.Add(_myRoomView);
            _myRoomView.OpenedDirections.Remove(InvertRoomType(_spawningRoomType));
            _spawningRoomView.OpenedDirections.Remove(_spawningRoomType);

            if (_spawningRoomView.OpenedDirections.Count == 0 && _myRoomView.OpenedDirections.Count == 0)
            {
                _dungeonManager.IsDungBuilded = true;
            }

            if (_dungeonManager.Rooms.Count >= _dungeonManager.CurrentMaxRooms - 1)
            {
                return;
            }

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
                    _spawningRoomView.RoomVariant = pickedRoomVariant;
                    _spawningRoomView.StartRoomAction();
                        
                    break;
                }
            }
        }

        private enumRoomTypes InvertRoomType(enumRoomTypes roomType)
        {
            switch (roomType)
            {
                    case enumRoomTypes.T:
                    {
                        return enumRoomTypes.B;
                    }
                    case enumRoomTypes.B:
                    {
                        return enumRoomTypes.T;
                    }case enumRoomTypes.R:
                    {
                        return enumRoomTypes.L;
                    }case enumRoomTypes.L:
                    {
                        return enumRoomTypes.R;
                    }
            }

            return enumRoomTypes.ClosedRoom;
        }

        private void OnTriggerEnter(Collider other)
        {
            _isForceQuit = FindObjectOfType<PlayfabManager>().IsForceQuit;
            if (_isForceQuit) { return; }

            if(!PhotonNetwork.IsMasterClient) { return; }
            
            if (other.CompareTag("EntryRoom"))
            {
                //PhotonNetwork.Destroy(gameObject);
                return;
            }
            
            if (other.CompareTag("ClosedRoom"))
            {
                PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
                return;
            }

            if (other.CompareTag("SpawnPoint"))
            {
                if (!other.GetComponent<RoomSpawner>()._isSpawned && !_isSpawned)
                {

                    _dungeonManager = GameObject.FindGameObjectWithTag("RoomTemplates").GetComponent<DungeonManager>();
                    if (_myRoomView.OpenedDirections.Count == 0 && other.GetComponent<RoomSpawner>()._myRoomView.OpenedDirections.Count == 0)
                    {
                        //PhotonNetwork.Destroy(gameObject);
                        _isSpawned = true;
                        return;
                    }

                //     print("trigger casted by room: " + transform.parent.transform.parent.gameObject.name + ", by:" + gameObject.name + "; with: " +
                //           other.transform.parent.transform.parent.gameObject.name+ ", by:" + other.gameObject.name);
                //     print(_myRoomView.OpenedDirections.Count + " - " + other.GetComponent<RoomSpawner>()._myRoomView.OpenedDirections.Count);
                //
                //     
                //     if (_isSpawned && other.GetComponent<RoomSpawner>()._isSpawned)
                //     {
                //         print("removing opened directions and spawning closed room at:" + other.transform.parent.transform.parent.gameObject.name+ ", by:" + other.gameObject.name);
                //         var roomTypes = _spawningRoomView.RoomTypes.ToList();
                //         roomTypes.Remove(InvertRoomType(_spawningRoomType));
                //         other.GetComponent<RoomSpawner>()._myRoomView.OpenedDirections.Remove(roomTypes[0]);
                //     }
                //     else
                //     {
                //         PhotonNetwork.Instantiate("Rooms/" + _dungeonManager.ClosedRoom.name, transform.position,
                //             Quaternion.identity);
                //         _myRoomView.OpenedDirections.RemoveAt(0);
                //         var otherSpawnPointRoom = other.transform.parent.transform.parent;
                //         otherSpawnPointRoom.GetComponent<RoomView>().OpenedDirections.RemoveAt(0);
                //     }
                //
                //     //PhotonNetwork.Instantiate("Rooms/" + _dungeonManager.ClosedRoom.name, transform.position, Quaternion.identity);
                }
                _isSpawned = true;
            }
        }
    }
}