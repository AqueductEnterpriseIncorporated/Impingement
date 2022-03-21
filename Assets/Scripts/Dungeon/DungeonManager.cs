using System;
using System.Collections.Generic;
using System.Linq;
using Impingement.NavMesh;
using Impingement.PhotonScripts;
using Impingement.Playfab;
using Impingement.SerializationAPI;
using Impingement.Stats;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
using Impingement.Serialization.SerializationClasses;

namespace Impingement.Dungeon
{
    public class DungeonManager : MonoBehaviour
    {
        public RoomModifierScriptableObject[] RoomVariants;
        public List<RoomBehaviour> Rooms = new List<RoomBehaviour>();
        public List<GameObject> Enemies = new List<GameObject>();
        public List<GameObject> Pikcups = new List<GameObject>();
        public SerializableDungeonData LoadedDungeonData = new SerializableDungeonData();
        [SerializeField] private NavigationBaker _navigationBaker;
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private GameObject _portalPrefab;
        [SerializeField] private GameObject _hideoutPortalPrefab;
        [SerializeField] private GameObject _spawnPoint;
        private DungeonProgressionManager _dungeonProgressionManager;
        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager = FindObjectOfType<NetworkManager>();
            _dungeonProgressionManager = FindObjectOfType<DungeonProgressionManager>();
        }

        public void Manage(bool dungeonIsSaved)
        {
            _navigationBaker.Bake();

            if (dungeonIsSaved)
            {
                SetAreaLevel();
                SetRoomModifiers();
            }
            else
            {
                AddRoomModifiers();
            }
            SetEnemyLevel();
            //SpawnBoss();
            SpawnPortals();
            SetSpawnPoint();
            _networkManager.Spawn();
            CleanUp();
            Destroy(GameObject.Find("LoadPanel"));
        }

        private void CleanUp()
        {
            foreach (var room in Rooms)
            {
                room.CleanUp();
            }
        }

        private void SetAreaLevel()
        {
            _dungeonProgressionManager.AreaLevel = Convert.ToInt32(LoadedDungeonData.AreaLevel);
        }

        private void SetEnemyLevel()
        {
            foreach (var enemy in Enemies)
            {
                enemy.GetComponent<BaseStats>().SetLevel(_dungeonProgressionManager.AreaLevel);
            }
        }

        private void AddRoomModifiers()
        {
            for (int i = 2; i < Rooms.Count - 1; i++)
            {
                AddModifier(Rooms[i]);
            }
        }
        
        private void SetRoomModifiers()
        {
            SetModifier();
        }

        private void SetSpawnPoint()
        {
            PhotonNetwork.Instantiate("RoomSpawns/" + _spawnPoint.name, Rooms[0].transform.position, Quaternion.identity);
        }

        private void SpawnBoss()
        {
            PhotonNetwork.Instantiate(_bossPrefab.name, Rooms[Rooms.Count - 1].transform.position, Quaternion.identity);
        }
        
        private void SpawnPortals()
        {
            var room = Rooms[Rooms.Count - 1];
            PhotonNetwork.Instantiate(_portalPrefab.name, room.PortalSpawnPoint.position, room.PortalSpawnPoint.rotation);
            PhotonNetwork.Instantiate(_hideoutPortalPrefab.name, room.HideoutPortalSpawnPoint.position, room.HideoutPortalSpawnPoint.rotation);
        }
        
        private void SetModifier()
        {
            for (int i = 2; i < LoadedDungeonData.RoomModifiers.Count - 1; i++)
            {
                RoomModifierScriptableObject pickedRoomVariant = null;
                foreach (var roomVariant in RoomVariants)
                {
                    if (roomVariant.ToString() == LoadedDungeonData.RoomModifiers[i].ModifierName)
                    {
                        pickedRoomVariant = roomVariant;
                        break;
                    }
                }
                pickedRoomVariant.SetRoom(Rooms[i]);
                pickedRoomVariant.SetDungeonManager(this);
                Rooms[i].RoomModifierVariant = pickedRoomVariant;
                Rooms[i].SetRoomAction();
            }
        }
        
        private void AddModifier(RoomBehaviour room)
        {
            var spawnChanceValue = Random.Range(0f, 1f);

            var sortedRoomVariants = RoomVariants.OrderBy(x => x.ChanceToSpawn).ToList();
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
                    
                    if (pickedRoomVariant.MaximumModifierSpawns > -1)
                    {
                        int spawnCount = 0;
                        foreach (var spawnedRooms in Rooms)
                        {
                            if (spawnedRooms.roomModifierVariant == pickedRoomVariant)
                            {
                                spawnCount++;
                            }
                        }
                        
                        if (spawnCount >= pickedRoomVariant.MaximumModifierSpawns)
                        {
                            continue;
                            //break;
                        }
                    }
                    
                    
                    //spawn room type
                    pickedRoomVariant.SetRoom(room);
                    pickedRoomVariant.SetDungeonManager(this);
                    room.RoomModifierVariant = pickedRoomVariant;
                    room.StartRoomAction();
                        
                    break;
                }
            }
        }

        [Serializable]
        public struct DungeonData
        {
            public List<string> RoomModifiersList;
            public List<string> EnemyPositionList;
            public List<string> EnemyNameList;
            public List<string> PickupsPositionList;
            public List<string> PickupsNameList;
            public List<DungeonGenerator.Cell> board;
            public string DungeonSize;
            public string AreaLevel;
        }
        
        public void GenerateJson()
        {
            SerializableDungeonData dungeonData = new SerializableDungeonData
            {
                Board = new List<DungeonGenerator.Cell>(),
                Enemies = new List<Enemies>(),
                Pickups = new List<Pickups>(),
                DungeonSize = String.Empty,
                AreaLevel = String.Empty,
                RoomModifiers = new List<RoomModifier>()
            };
            
            dungeonData.Board = FindObjectOfType<DungeonGenerator>().Board;

            foreach (var enemy in Enemies)
            {
                dungeonData.Enemies.Add(new Enemies
                {
                    EnemyName = enemy.gameObject.name.Replace("(Clone)", ""),
                    EnemyPosition = enemy.transform.position.ToString()
                });
            }
            
            foreach (var pickup in Pikcups)
            {
                dungeonData.Pickups.Add(new Pickups
                {
                    PickupName = pickup.gameObject.name.Replace("(Clone)", ""),
                    PickupPosition = pickup.transform.position.ToString()
                });
            }
            
            var progressionManager = FindObjectOfType<DungeonProgressionManager>();
            dungeonData.DungeonSize = progressionManager.GetDungeonSize().ToString();
            dungeonData.AreaLevel = progressionManager.AreaLevel.ToString();
            
            foreach (var room in Rooms)
            {
                if (room.RoomModifierVariant == null)
                {
                    dungeonData.RoomModifiers.Add(new RoomModifier
                    {
                        ModifierName = String.Empty,
                        RandomlyGeneratedObjectSpawnsAmount = String.Empty,
                        RandomlyGeneratedObjectPrefabNamesList = null
                    });
                }
                else
                {
                    dungeonData.RoomModifiers.Add(new RoomModifier
                    {
                        ModifierName = room.RoomModifierVariant.ToString(),
                        RandomlyGeneratedObjectSpawnsAmount =
                            room.RandomlyGeneratedObjectSpawnsAmount.ToString(),
                        RandomlyGeneratedObjectPrefabNamesList =
                            room.RandomlyGeneratedObjectPrefabNamesList
                    });
                }
            }
            
            var data = StringSerializationAPI.Serialize(typeof(SerializableDungeonData),  dungeonData);
            FindObjectOfType<PlayfabManager>().UploadJson("DungeonData", data);
        }

        public SerializableDungeonData GetData(string data)
        {
            return (SerializableDungeonData) StringSerializationAPI.Deserialize(typeof(SerializableDungeonData), data);
        }
    }
}