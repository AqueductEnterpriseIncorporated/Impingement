using UnityEngine;

namespace Impingement.Dungeon
{
    public class RoomBehaviour : MonoBehaviour
    {
        public GameObject[] walls; // 0 - Up 1 -Down 2 - Right 3- Left
        public GameObject[] doors;

        public RoomModifierScriptableObject roomModifierVariant;
        private DungeonManager _dungeonManager;
        public string RoomPrefabName { get; set; }

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
    }

}