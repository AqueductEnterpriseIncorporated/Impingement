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
            FindObjectOfType<PlayfabManager>().IsForceQuit = false;
            SceneManager.LoadScene("Hideout");
        }
    }
}