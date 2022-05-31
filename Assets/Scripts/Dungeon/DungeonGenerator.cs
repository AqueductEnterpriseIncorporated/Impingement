using System;
using System.Collections.Generic;
using Impingement.Playfab;
using Impingement.Serialization.SerializationClasses;
using Photon.Pun;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Impingement.Dungeon
{
    [RequireComponent(typeof(PhotonView))]
    public class DungeonGenerator : MonoBehaviour, IPunObservable
    {
        [SerializeField] private DungeonManager _dungeonManager;
        
        public class Cell
        {
            public bool visited = false;
            public bool[] status = new bool[4];
        }

        [Serializable]
        public class Rule
        {
            public GameObject room;
            public Vector2Int minPosition;
            public Vector2Int maxPosition;

            public bool obligatory;

            public int ProbabilityOfSpawning(int x, int y)
            {
                // 0 - cannot spawn 1 - can spawn 2 - HAS to spawn

                if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
                {
                    return obligatory ? 2 : 1;
                }

                return 0;
            }

        }

        public Vector2Int size;
        public int startPos = 0;
        public Rule[] rooms;
        public Vector2 offset;

        public List<Cell> Board;
        private SerializableDungeonData _dungeonData;
        private int _syncingDungeonData;

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            var dungeonIsSaved = FindObjectOfType<PlayfabManager>().DungeonIsSaved;

            if (dungeonIsSaved)
            {
                FindObjectOfType<PlayfabManager>().LoadJson(OnDataReceived);
            }
            //else
            {

                try
                {
                    Generate();
                }
                catch (Exception e)
                {
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    print(e.Message);
                }

            }
        }

        private void Generate()
        {
            var dungeonSize = FindObjectOfType<DungeonProgressionManager>().GetDungeonSize();
            size = dungeonSize;
            MazeGenerator();
            _syncingDungeonData = Random.Range(0, 99);
            //GenerateDungeon();
            //SyncData();
            //_dungeonManager.Manage(false);
        }

        private void SyncData()
        {
            //_syncingDungeonData = _dungeonManager.GenerateJson();
        }

        private void OnDataReceived(GetUserDataResult result)
        {
            if (result != null && result.Data.ContainsKey("DungeonData"))
            {
                var json = result.Data["DungeonData"].Value;
                var dungeonManager = FindObjectOfType<DungeonManager>();
                _dungeonData = dungeonManager.GetData(json);
                dungeonManager.LoadedDungeonData = _dungeonData;
                Board = _dungeonData.Board;
                size = StringToVector2(_dungeonData.DungeonSize);
                GenerateDungeon();
                _dungeonManager.Manage(true);
            }
        }

        private void GenerateDungeon()
        {
            foreach (var room in rooms)
            {
                room.maxPosition = size;
            }
            
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Cell currentCell = Board[(i + j * size.x)];
                    if (currentCell.visited)
                    {
                        int randomRoom = -1;
                        List<int> availableRooms = new List<int>();

                        for (int k = 0; k < rooms.Length; k++)
                        {
                            int p = rooms[k].ProbabilityOfSpawning(i, j);

                            if (p == 2)
                            {
                                randomRoom = k;
                                break;
                            }
                            else if (p == 1)
                            {
                                availableRooms.Add(k);
                            }
                        }

                        if (randomRoom == -1)
                        {
                            if (availableRooms.Count > 0)
                            {
                                randomRoom = availableRooms[Random.Range(0, availableRooms.Count)];
                            }
                            else
                            {
                                randomRoom = 0;
                            }
                        }


                        var newRoom = PhotonNetwork
                            .Instantiate("Rooms/" + rooms[randomRoom].room.name,
                                new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity)
                            .GetComponent<RoomBehaviour>();
                        newRoom.UpdateRoom(currentCell.status);
                        newRoom.name += " " + i + "-" + j;
                        if (_dungeonData != null)
                        {
                            if (_dungeonData
                                .RoomModifiers[_dungeonManager.Rooms.Count].RandomlyGeneratedObjectSpawnsAmount != "")
                            {
                                newRoom.RandomlyGeneratedObjectSpawnsAmount = Convert.ToInt32(_dungeonData
                                    .RoomModifiers[_dungeonManager.Rooms.Count].RandomlyGeneratedObjectSpawnsAmount);
                                newRoom.RandomlyGeneratedObjectPrefabNamesList = _dungeonData
                                    .RoomModifiers[_dungeonManager.Rooms.Count].RandomlyGeneratedObjectPrefabNamesList;
                            }
                        }
                        if (PhotonNetwork.IsMasterClient)
                        {
                            newRoom.ManageEnemyAmount(false, _dungeonData != null);
                        }

                        _dungeonManager.Rooms.Add(newRoom);
                    }
                }
            }
        }

        private void MazeGenerator()
        {
            Board = new List<Cell>();

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Board.Add(new Cell());
                }
            }

            int currentCell = startPos;

            Stack<int> path = new Stack<int>();

            int k = 0;

            while (k < 1000)
            {
                k++;

                Board[currentCell].visited = true;

                if (currentCell == Board.Count - 1)
                {
                    break;
                }

                //Check the cell's neighbors
                List<int> neighbors = CheckNeighbors(currentCell);

                if (neighbors.Count == 0)
                {
                    if (path.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        currentCell = path.Pop();
                    }
                }
                else
                {
                    path.Push(currentCell);

                    int newCell = neighbors[Random.Range(0, neighbors.Count)];

                    if (newCell > currentCell)
                    {
                        //down or right
                        if (newCell - 1 == currentCell)
                        {
                            Board[currentCell].status[2] = true;
                            currentCell = newCell;
                            Board[currentCell].status[3] = true;
                        }
                        else
                        {
                            Board[currentCell].status[1] = true;
                            currentCell = newCell;
                            Board[currentCell].status[0] = true;
                        }
                    }
                    else
                    {
                        //up or left
                        if (newCell + 1 == currentCell)
                        {
                            Board[currentCell].status[3] = true;
                            currentCell = newCell;
                            Board[currentCell].status[2] = true;
                        }
                        else
                        {
                            Board[currentCell].status[0] = true;
                            currentCell = newCell;
                            Board[currentCell].status[1] = true;
                        }
                    }

                }

            }
        }

        private List<int> CheckNeighbors(int cell)
        {
            List<int> neighbors = new List<int>();

            //check up neighbor
            if (cell - size.x >= 0 && !Board[(cell - size.x)].visited)
            {
                neighbors.Add((cell - size.x));
            }

            //check down neighbor
            if (cell + size.x < Board.Count && !Board[(cell + size.x)].visited)
            {
                neighbors.Add((cell + size.x));
            }

            //check right neighbor
            if ((cell + 1) % size.x != 0 && !Board[(cell + 1)].visited)
            {
                neighbors.Add((cell + 1));
            }

            //check left neighbor
            if (cell % size.x != 0 && !Board[(cell - 1)].visited)
            {
                neighbors.Add((cell - 1));
            }

            return neighbors;
        }
        
        private Vector2Int StringToVector2(string sVector)
        {
            sVector = sVector.Substring(sVector.IndexOf("(") + 1, sVector.IndexOf(")") - 1);
            
            string[] sArray = sVector.Split(',');
 
            Vector2Int result = new Vector2Int(
                int.Parse(sArray[0]),
                int.Parse(sArray[1]));

            return result;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                Debug.Log(_syncingDungeonData);
                stream.SendNext(_syncingDungeonData);
            }
            else
            {
                _syncingDungeonData = (int)stream.ReceiveNext();
                Debug.Log(_syncingDungeonData);

                if (_syncingDungeonData != null)
                {
                    // var dungeonManager = FindObjectOfType<DungeonManager>();
                    // _dungeonData = dungeonManager.GetData(_syncingDungeonData);
                    // dungeonManager.LoadedDungeonData = _dungeonData;
                    // Board = _dungeonData.Board;
                    // size = StringToVector2(_dungeonData.DungeonSize);
                    // GenerateDungeon();
                    // _dungeonManager.Manage(true);
                }
            }
        }
    }
}