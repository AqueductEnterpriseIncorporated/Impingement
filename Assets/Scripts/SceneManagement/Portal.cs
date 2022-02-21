using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private string _sceneToLoad = "";
        private NetworkManager _networkManager;

        private void Start()
        {
            _networkManager = FindObjectOfType<NetworkManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _networkManager.SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single);
            }
        }
    }
}