using Impingement.SceneManagement;
using UnityEngine;

namespace Impingement.UI
{
    public class StartGamePanel : MonoBehaviour
    {
        [SerializeField] private int _arenaIndex;
        [SerializeField] private int _dungeonIndex;
        
        public void LoadToDungeon()
        {
            FindObjectOfType<PortalView>().LoadLevel(_dungeonIndex);
        }

        public void LoadToArena()
        {
            FindObjectOfType<PortalView>().LoadLevel(_arenaIndex);
        }
    }
}