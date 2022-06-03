using System;
using Impingement.Control;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.UI
{
    public class ServerInfoPanel : MonoBehaviour
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PlayerPanelItem _playerPanelItem;
        [SerializeField] private GameObject _leaveButton;
        [SerializeField] private TMP_Text _serverNameText;

        private void Update()
        {
            if (_inputManager.GetKeyDown("Информация о сервере"))
            {
                _parentTransform.gameObject.SetActive(!_parentTransform.gameObject.activeSelf);
            }
            
            if (_parentTransform.gameObject.activeSelf)
            {
                UpdateInfo();
            }
        }

        private void UpdateInfo()
        {
            if (PhotonNetwork.InRoom)
            {
                _serverNameText.text = String.Concat("Текущая комната: ", PhotonNetwork.CurrentRoom.Name);
            }
            else
            {
                _serverNameText.text = "Вы не в комнате";
            }
            _leaveButton.SetActive(PhotonNetwork.InRoom);

            for (int i = 0; i < _contentTransform.childCount; i++)
            {
                Destroy(_contentTransform.GetChild(i).gameObject);
            }

            foreach (var player in PhotonNetwork.PlayerListOthers)
            {
                var newItem = Instantiate(_playerPanelItem, _contentTransform);
                newItem.SetupPlayer(FindPlayerByNumber(player.ActorNumber));
            }
        }

        private PlayerController FindPlayerByNumber(int playerActorNumber)
        {
            foreach (var playerController in FindObjectsOfType<PlayerController>())
            {
                if (playerController.GetPhotonView().ControllerActorNr == playerActorNumber)
                {
                    return playerController;
                }
            }

            return null;
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(1);
        }
    }
}