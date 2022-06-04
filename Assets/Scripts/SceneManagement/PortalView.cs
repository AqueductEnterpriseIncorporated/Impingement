using System;
using System.Linq;
using Impingement.Control;
using Impingement.Dungeon;
using Impingement.Playfab;
using Impingement.UI;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.SceneManagement
{
    public class PortalView : MonoBehaviour
    {
        [SerializeField] private bool _saveDungeonData;
        [SerializeField] private bool _completeLevelOnEnter;
        [SerializeField] private int _sceneIndexToLoad;
        [SerializeField] private GameObject _loadPanel;
        [SerializeField] private StartGamePanel _startGamePanel;
        private PlayfabManager _playfabManager;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Instantiate(_loadPanel);
                _playfabManager = FindObjectOfType<PlayfabManager>();

                if (_completeLevelOnEnter)
                {
                    FindObjectOfType<DungeonProgressionManager>().CompleteLevel();
                    _playfabManager.DungeonIsSaved = false;
                }

                // if (_saveDungeonData)
                // {
                //     FindObjectOfType<DungeonManager>().GenerateJson();
                //     _playfabManager.DungeonIsSaved = true;
                // }
                
                ManageSceneChanging();
            }
        }

        private void ManageSceneChanging()
        {
            // foreach (var player in FindObjectsOfType<PlayerController>().Select(p => p.gameObject.transform.parent))
            // {
            //     if (player.GetComponentInChildren<PhotonView>().IsMine)
            //     {
            //         try
            //         {
            //
            //
            //             PhotonNetwork.Destroy(player.gameObject);
            //         }
            //         catch (Exception e)
            //         {
            //             Debug.Log(e.Message);
            //         }
            //     }
            // }
            FindObjectOfType<PlayfabPlayerDataController>().SavePlayerData();

            if (_startGamePanel != null)
            {
                _startGamePanel.gameObject.SetActive(true);
                    //other.GetComponent<PlayfabPlayerDataController>().SavePlayerData();
            }
            else
            {
                PhotonNetwork.LoadLevel(_sceneIndexToLoad);
            }
            //SceneManager.LoadSceneAsync(_sceneToLoad);
        }

        public void LoadLevel(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_startGamePanel != null)
                {
                    _startGamePanel.gameObject.SetActive(false);
                }
            }
        }
    }
}