using System.Collections.Generic;
using System.Linq;
using Impingement.Control;
using Impingement.PhotonScripts;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    [SerializeField] private GameObject _startGamePanel;
    [SerializeField] private GameObject _connectingPanel;
    [SerializeField] private GameObject _portalPrefab;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private GameObject _connectingText;
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private RoomItemView _roomItemPrefab;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private Transform _portalSpawnTransform;
    [SerializeField] private string _portalSceneToLoadName;
    private readonly List<RoomItemView> _roomListViews = new List<RoomItemView>();
    private readonly float _lobbyListUpdateCooldown = 1.5f;
    private float _nextUpdateTime;
    private bool _isMultiplayer;
    
    public void StartSoloGame()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(_portalPrefab.name, _portalSpawnTransform.position, Quaternion.identity);
            return;
        }
        ConnectAndJoinLobby();
        _connectingText.SetActive(true);
    }
    
    public void StartMultiplayerGame()
    {
        _isMultiplayer = true;
        //_connectingPanel.SetActive(true);
        _connectingText.SetActive(true);
        ConnectAndJoinLobby();
    }

    private static void ConnectAndJoinLobby()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _connectingText.SetActive(false);

        if (!_isMultiplayer)
        {
            CreateRoom();
        }
        else
        {
            _lobbyPanel.SetActive(true);
        }
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, null, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        _startGamePanel.SetActive(false);
        _lobbyPanel.SetActive(false);

        if (_isMultiplayer)
        {
            //_roomPanel.SetActive(true);
            _roomName.text = PhotonNetwork.CurrentRoom.Name;
            if (!PhotonNetwork.IsMasterClient)
            {
                FindObjectOfType<NetworkManager>().SpawnPlayer();
            }
        }
        else
        {
            PhotonNetwork.Instantiate(_portalPrefab.name, _portalSpawnTransform.position, Quaternion.identity);
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

    public void CloseLobbyPanel()
    {
        _lobbyPanel.SetActive(false);
    }
}