using Impingement.Dungeon;
using Impingement.Playfab;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.Core
{
    public class RespawnController : MonoBehaviour
    {
        [SerializeField] private GameObject _parent;
        [SerializeField] private GameObject _loadPanel;

        public void Respawn()
        {
            Instantiate(_loadPanel);
            _parent.SetActive(false);
            FindObjectOfType<PlayfabManager>().DungeonIsSaved = false;
            FindObjectOfType<DungeonProgressionManager>().Reset();
            FindObjectOfType<PlayfabPlayerDataController>().SavePlayerData();
            SceneManager.LoadScene("Hideout");
        }
    }
}