using System.Collections.Generic;
using UnityEngine;

namespace Impingement.Dungeon
{
    public class RoomBehaviour : MonoBehaviour
    {
        public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
        public GameObject[] doors;

        public int RandomlyGeneratedObjectSpawnsAmount;
        public List<string> RandomlyGeneratedObjectPrefabNamesList = new List<string>();

        public RoomModifierScriptableObject roomModifierVariant;
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
    }

}