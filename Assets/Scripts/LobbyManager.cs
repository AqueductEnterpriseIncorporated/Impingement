using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private SavingWrapper _savingWrapper;
    [SerializeField] private GameObject _startGamePanel;
    [SerializeField] private GameObject _connectingPanel;
    [SerializeField] private GameObject _portalPrefab;
    [SerializeField] private GameObject _lobbyPanel;
    [SerializeField] private GameObject _roomPanel;
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private RoomItemView _roomItemPrefab;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private Transform _portalSpawnTransform;
    [SerializeField] private string _portalSceneToLoadName;
    private readonly List<RoomItemView> _roomListViews = new List<RoomItemView>();
    private readonly float _lobbyListUpdateCooldown = 1.5f;
    private float _nextUpdateTime;
    private bool _isMultiplayer;

    private void Start()
    {
        //PlayerPrefs.SetInt("forceQuit", 0);
    }

    public void StartSoloGame()
    {
        _startGamePanel.SetActive(false);
        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));
    }
    
    public void StartMultiplayerGame()
    {
        // _isMultiplayer = true;
        // _connectingPanel.SetActive(true);
        // PhotonNetwork.ConnectUsingSettings(); 
    }

    public override void OnConnectedToMaster()
    {

        if (_isMultiplayer)
        {
            _connectingPanel.SetActive(false);
        }
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (!_isMultiplayer)
        {
            CreateRoom();
        }
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName, null, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        if (_isMultiplayer)
        {
            _lobbyPanel.SetActive(false);
            _roomPanel.SetActive(true);
            _roomName.text = PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            //_savingWrapper.LoadLastScene();
            PhotonNetwork.Instantiate(_portalPrefab.name, _portalSpawnTransform.position, Quaternion.identity);
            //PhotonNetwork.LoadLevel("Dungeon2");
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
}
