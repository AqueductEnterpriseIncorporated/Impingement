using System;
using Impingement.Control;
using Impingement.Playfab;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.UI
{
    public class InRoomPlayersInfoPanel : MonoBehaviour
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PlayerPanelItem _playerPanelItem;
        [SerializeField] private TMP_Text _statusText;

        private void Update()
        {
            if (_inputManager.GetKeyDown("Информация о игроках в комнате"))
            {
                _parentTransform.gameObject.SetActive(!_parentTransform.gameObject.activeSelf);
                
                if (_parentTransform.gameObject.activeSelf)
                {
                    if (!PhotonNetwork.InRoom)
                    {
                        _statusText.text = "Нет игроков";
                    }
                    else
                    {
                        _statusText.text = $"Комната: {PhotonNetwork.CurrentRoom.Name}";

                    }
                    UpdateInfo();
                }
            }
        }

        private void UpdateInfo()
        {
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
    }
}