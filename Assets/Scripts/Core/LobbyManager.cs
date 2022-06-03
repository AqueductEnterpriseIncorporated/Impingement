using System;
using System.Collections.Generic;
using System.Linq;
using Impingement.Control;
using Impingement.PhotonScripts;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _startGamePanel;
    [SerializeField] private GameObject _connectingPanel;
    [SerializeField] private GameObject _portalPrefab;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _connectingText;
    [SerializeField] private GameObject _leaveRoomButton;
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private RoomItemView _roomItemPrefab;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private Transform _portalSpawnTransform;
    [SerializeField] private string _portalSceneToLoadName;
    [SerializeField] private InputManager _inputManager;
    private readonly byte MaxPlayers = 4;
    private readonly List<RoomItemView> _roomListViews = new List<RoomItemView>();
    private readonly float _lobbyListUpdateCooldown = 1f;
    private float _nextUpdateTime;
    private bool _isMultiplayer = true;
    private bool _isPortalCreated;

    public event Action PlayerConntected;

    private void Start()
    {
        PlayerConntected?.Invoke();
        //StartMultiplayerGame();
    }

    private void Update()
    {
        if (_inputManager.GetKeyDown("Данные о лобби"))
        {
            _lobbyPanel.SetActive(!_lobbyPanel.activeSelf);
        }

        _leaveRoomButton.SetActive(_isMultiplayer);
        
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
            _statusText.text = $"Вы находитесь в комнате игрока: {PhotonNetwork.CurrentRoom.Name}";
        }
    }

    public void StartSoloGame()
    {
        _isMultiplayer = false;
        if (PhotonNetwork.IsConnected && !_isPortalCreated)
        {
            Instantiate(_portalPrefab, _portalSpawnTransform.position, Quaternion.identity);
            _isPortalCreated = true;
            _connectingText.SetActive(false);
            return;
        }
        ConnectAndJoinLobby();
        _connectingText.SetActive(true);
    }
    
    public void StartMultiplayerGame()
    {
        _isMultiplayer = true;
        //_connectingPanel.SetActive(true);
        if (!PhotonNetwork.IsConnected)
        {
            _connectingText.SetActive(true);
        }

        ConnectAndJoinLobby();
    }

    private void ConnectAndJoinLobby()
    {
        //GameObject.Find("LoadPanel").GetComponentInChildren<TMP_Text>().text = "Подключение к Photon...";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        _connectingText.SetActive(false);
        
        if (_isMultiplayer)
        {
            _lobbyPanel.SetActive(true);
        }
        else
        {
            CreateRoom(false);
        }
        //CreateRoom();
    }

    public void CreateRoom(bool visible = true)
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, new RoomOptions(){MaxPlayers = MaxPlayers, IsVisible = visible}, TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        //PlayerConntected?.Invoke();
        _startGamePanel.SetActive(false);
        _lobbyPanel.SetActive(false);


        //_roomPanel.SetActive(true);
        _roomName.text = PhotonNetwork.CurrentRoom.Name;
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(_portalPrefab, _portalSpawnTransform.position, Quaternion.identity);
        }
        else
        {
            //PhotonNetwork.LoadLevel(1);
            //SceneManager.LoadScene("Tests3");
            FindObjectOfType<NetworkManager>().SpawnPlayer();
        }
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
        Debug.Log(roomList.Count);
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
            newRoom.SetRoomName(room.Name);
            _roomListViews.Add(newRoom);
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void CloseLobbyPanel()
    {
        _lobbyPanel.SetActive(false);
    }
}