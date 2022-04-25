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

        public void ManageEnemyAmount(bool noEnemyRoom)
        {
            if (noEnemyRoom)
            {
                foreach (var enemy in _enemies)
                {
                    Destroy(enemy);
                }
                return;
            }
            
            if(_enemies.Count == 0 ) { return; }
            
            int amount = Random.Range(_enemiesMininmum, _enemiesMaximum);
            var arrayCount = _enemies.Count;
            for (int i = 0; i < arrayCount - amount; i++)
            {
                int randomIndex = Random.Range(0, _enemies.Count-1);
                Destroy(_enemies[randomIndex]);
                _enemies.RemoveAt(randomIndex);
            }
        }

        public void SetRoomAction()
        {
            roomModifierVariant.SetRoomAction();
        }

        public void CleanUp()
        {
            Destroy(_photonView);
            Destroy(_collider);
        }
    }
}