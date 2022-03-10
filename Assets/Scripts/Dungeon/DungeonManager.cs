using System;
using System.Collections.Generic;
using System.Linq;
using Impingement.NavMesh;
using Impingement.PhotonScripts;
using Impingement.Saving;
using Impingement.Stats;
using Photon.Pun;
using RPG.Saving;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Impingement.Dungeon
{
    public class DungeonManager : MonoBehaviour, ISaveable
    {
        public RoomModifierScriptableObject[] RoomVariants;
        public List<RoomBehaviour> Rooms = new List<RoomBehaviour>();
        public List<GameObject> Enemies = new List<GameObject>();
        [SerializeField] private NavigationBaker _navigationBaker;
        [SerializeField] private GameObject _bossPrefab;
        [SerializeField] private GameObject _spawnPoint;
        private DungeonProgressionManager _dungeonProgressionManager;
        
        public void Manage()
        {
            _dungeonProgressionManager = FindObjectOfType<DungeonProgressionManager>();
            _navigationBaker.Bake();
            for (int i = 1; i < Rooms.Count-2; i++)
            {
                AddModifier(Rooms[i]);
            }

            foreach (var enemy in Enemies)
            {
                enemy.GetComponent<BaseStats>().SetLevel(_dungeonProgressionManager.AreaLevel);
            }
            SpawnBoss();
            
            SetSpawnPoint();
            FindObjectOfType<NetworkManager>().Spawn();
            
            Destroy(GameObject.Find("LoadPanel"));
        }

        private void SetSpawnPoint()
        {
            PhotonNetwork.Instantiate("RoomSpawns/" + _spawnPoint.name, Rooms[0].transform.position, Quaternion.identity);
        }

        private void SpawnBoss()
        {
            PhotonNetwork.Instantiate(_bossPrefab.name, Rooms[Rooms.Count - 1].transform.position, Quaternion.identity);
        }
        
        public void AddModifier(RoomBehaviour room)
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
                            if (spawnedRooms.GetComponent<RoomBehaviour>().roomModifierVariant == pickedRoomVariant)
                            {
                                spawnCount++;
                            }
                        }
                        
                        if (spawnCount >= pickedRoomVariant.MaximumModifierSpawns)
                        {
                            break;
                            //break;
                        }
                    }
                    
                    
                    //spawn room type
                    pickedRoomVariant.SetRoom(room.gameObject);
                    room.RoomModifierVariant = pickedRoomVariant;
                    room.StartRoomAction();
                        
                    break;
                }
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
                roomsAndEnemyData.RoomNameList.Add(room.GetComponent<RoomBehaviour>().RoomPrefabName);
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
                Rooms.Add(localRoomPrefab.GetComponent<RoomBehaviour>());
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