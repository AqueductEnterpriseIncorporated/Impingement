using System;
using System.Collections.Generic;
using Impingement.Attributes;
using Impingement.Control;
using Impingement.PhotonScripts;
using Impingement.Playfab;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Impingement.UI
{
    public class LobbyInfoPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject _loadPanel;
        [SerializeField] private GameObject _parent;
        [SerializeField] private GameObject _createButton;
        [SerializeField] private GameObject _loadHideoutButton;
        [SerializeField] private Slider _maxPlayerSlider;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private TMP_Text _createButtonText;
        [SerializeField] private TMP_Text _maxPlayersText;
        [SerializeField] private RoomItemView _roomItemPrefab;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PlayfabPlayerDataController _playfabPlayerDataController;
        [SerializeField] private PlayerNetworkController _playerNetworkController;
        private readonly List<RoomItemView> _roomListViews = new List<RoomItemView>();
        private readonly float _lobbyListUpdateCooldown = 1f;
        private float _nextUpdateTime;
        private GameObject _localLoadPanel;


        private void Update()
        {
            if (_inputManager.GetKeyDown("Данные лобби"))
            {
                
                _parent.SetActive(!_parent.activeSelf);
            }
            
            _loadHideoutButton.SetActive(SceneManager.GetActiveScene().name == "Arena");
            _createButton.SetActive(!PhotonNetwork.InRoom && SceneManager.GetActiveScene().name == "Arena");
            _maxPlayerSlider.gameObject.SetActive(!PhotonNetwork.InRoom && SceneManager.GetActiveScene().name == "Arena");

            if (PhotonNetwork.IsConnected)
            {
                _statusText.text = "Вы подключены к серверу Photon";
            }
            else
            {
                _statusText.text = "Вы не подключены к серверу Photon";
            }

            if (PhotonNetwork.InLobby)
            {
                _statusText.text = "Вы находитесь в лобби";
            }

            if (PhotonNetwork.InRoom)
            {
                _createButtonText.text = "Покинуть комнату";
                _statusText.text = $"Вы находитесь в комнате игрока: {PhotonNetwork.CurrentRoom.Name}";
            }
            else
            {
                _createButtonText.text = "Создать комнату";
            }
        }

        public void CreateRoom()
        {
            if (!PhotonNetwork.InRoom)
            {
                foreach (var roomItem in _roomListViews)
                {
                    Destroy(roomItem.gameObject);
                }

                _roomListViews.Clear();
                PhotonNetwork.CreateRoom(PhotonNetwork.NickName,
                    new RoomOptions() { MaxPlayers = (byte)_maxPlayerSlider.value });

            }
            else
            {
                LeaveRoom();
            }
        }

        public override void OnConnectedToMaster()
        {
            if (_localLoadPanel != null)
            {
                Destroy(_localLoadPanel);
            }
        }

        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                _localLoadPanel = Instantiate(_loadPanel);
                _playerNetworkController.SelfDestroy();
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(3);
            }
        }
        
        public void LoadHideoutScene()
        {
            _localLoadPanel = Instantiate(_loadPanel);
            _playerNetworkController.SelfDestroy();
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }

            SceneManager.LoadSceneAsync(1);
        }

        
        
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (Time.time >= _nextUpdateTime)
            {
                UpdateRoomList(roomList);
                _nextUpdateTime = Time.time + _lobbyListUpdateCooldown;
            }
        }

        private void UpdateRoomList(List<RoomInfo> roomList)
        {
            foreach (var roomItem in _roomListViews)
            {
                Destroy(roomItem.gameObject);
            }
            _roomListViews.Clear();
        
            foreach (var room in roomList)
            {
                if (room.RemovedFromList)
                {
                    return;
                }
                var newRoom = Instantiate(_roomItemPrefab, _contentTransform);
                newRoom.SetupRoom(room);
                _roomListViews.Add(newRoom);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            foreach (var healthController in FindObjectsOfType<HealthController>())
            {
                healthController.IsInvulnerable = true;
            }
        }

        public void OnSliderValueChanged(float value)
        {
            _maxPlayersText.text = $"Максимум игроков {_maxPlayerSlider.value}";
        }
    }
}