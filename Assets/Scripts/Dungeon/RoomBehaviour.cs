using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Dungeon
{
    public class RoomBehaviour : MonoBehaviour
    {
        public Transform PortalSpawnPoint;
        public Transform HideoutPortalSpawnPoint;
        public BoxCollider BoxCollider;
        public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
        public GameObject[] doors;
        public int RandomlyGeneratedObjectSpawnsAmount;
        public List<string> RandomlyGeneratedObjectPrefabNamesList = new List<string>();
        public RoomModifierScriptableObject roomModifierVariant;
        [SerializeField] private Collider _collider;
        [SerializeField] private PhotonView _photonView;
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