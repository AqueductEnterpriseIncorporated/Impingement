using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Dungeon
{
    public class RoomBehaviour : MonoBehaviour
    {
        public Transform PortalSpawnPoint;
        public Transform HideoutPortalSpawnPoint;
        public Transform ItemSpawnPoint;
        public BoxCollider BoxCollider;
        public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
        public GameObject[] doors;
        public int RandomlyGeneratedObjectSpawnsAmount;
        public List<string> RandomlyGeneratedObjectPrefabNamesList = new List<string>();
        public List<int> EnemiesToRemove = new List<int>();
        public RoomModifierScriptableObject roomModifierVariant;
        [SerializeField] private Collider _collider;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private List<GameObject> _enemies;
        [SerializeField] private int _enemiesMininmum;
        [SerializeField] private int _enemiesMaximum;
        private DungeonManager _dungeonManager;

        public void UpdateRoom(bool[] status)
        {
            for (int i = 0; i < status.Length; i++)
            {
                doors[i].SetActive(status[i]);
                walls[i].SetActive(!status[i]);
            }
        }

        public RoomModifierScriptableObject RoomModifierVariant
        {
            get => roomModifierVariant;
            set { roomModifierVariant = value; }
        }

        public void StartRoomAction()
        {
            roomModifierVariant.RoomAction();
        }

        public void ManageEnemyAmount(bool noEnemyRoom, bool isLoaded)
        {
            if (noEnemyRoom)
            {
                foreach (var enemy in _enemies)
                {
                    PhotonNetwork.Destroy(enemy);
                }
                return;
            }
            
            if(_enemies.Count == 0 ) { return; }

            if (!isLoaded)
            {
                int amount = Random.Range(_enemiesMininmum, _enemiesMaximum);
                var arrayCount = _enemies.Count;
                for (int i = 0; i < arrayCount - amount; i++)
                {
                    int randomIndex = Random.Range(0, _enemies.Count - 1);
                    PhotonNetwork.Destroy(_enemies[randomIndex]);
                    _enemies.RemoveAt(randomIndex);
                    EnemiesToRemove.Add(randomIndex);
                }
            }
            else
            {
                // for (int i = 0; i < _enemies.Count; i++)
                // {
                //     Destroy(_enemies[EnemiesToRemove[0]]);
                //     _enemies.RemoveAt(EnemiesToRemove[0]);
                // }
            }
        }

        public void SetRoomAction()
        {
            roomModifierVariant.SetRoomAction();
        }

        public void CleanUp()
        {
            PhotonNetwork.Destroy(_photonView);
            //Destroy(_collider);
        }
    }
}