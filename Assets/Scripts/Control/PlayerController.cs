using System;
using System.Collections.Generic;
using Impingement.Core;
using Impingement.enums;
using Impingement.Attributes;
using Impingement.Combat;
using Impingement.Currency;
using Impingement.Inventory;
using Impingement.Playfab;
using Impingement.Stats;
using Impingement.structs;
using Impingement.UI;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Impingement.Control
{
    public class PlayerController : MonoBehaviourPunCallbacks, IAction
    {
        [SerializeField] private GameObject _hud;
        [SerializeField] private float _raycastRadius = 1f;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _speed = 3;
        [SerializeField] private Camera _camera;
        [SerializeField] private CursorMapping[] _cursorMappings;
        [SerializeField] private CombatController _combatController;
        [SerializeField] private ActionScheduleController _actionScheduleController;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private HealthController _healthController;
        [SerializeField] private TargetHealthDisplay _targetHealthDisplay;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private PlayerCameraController _playerCameraController;
        [SerializeField] private StaminaController _staminaController;
        [SerializeField] private PlayerCurrencyController _playerCurrencyController;
        [SerializeField] private PlayersPanel _playersPanel;
        [SerializeField] private PlayfabPlayerDataController _playfabPlayerDataController;
        [SerializeField] private PlayfabManager _playfabManager;
        [SerializeField] private ExperienceController _experienceController;
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private ItemDropper _itemDropper;
        [SerializeField] private List<WeaponConfig> _availableWeapon;
        private readonly int _cameraYRotation = 45;

        public PlayfabPlayerDataController GetPlayfabPlayerDataController()
        {
            return _playfabPlayerDataController;
        }

        public CombatController GetCombatController()
        {
            return _combatController;
        }

        public TargetHealthDisplay GetTargetHealthDisplay()
        {
            return _targetHealthDisplay;
        }

        public HealthController GetHealthController()
        {
            return _healthController;
        }

        public StaminaController GetStaminaController()
        {
            return _staminaController;
        }

        public PlayerCurrencyController GetPlayerCurrencyController()
        {
            return _playerCurrencyController;
        }

        public PhotonView GetPhotonView()
        {
            return _photonView;
        }

        public PlayersPanel GetPlayersPanel()
        {
            return _playersPanel;
        }

        public GameObject GetHUD()
        {
            return _hud;
        }

        public InventoryController GetInventoryController()
        {
            return _inventoryController;
        }
        
        public ItemDropper GetItemDropper()
        {
            return _itemDropper;
        }

        private void Awake()
        {
            _playfabManager = FindObjectOfType<PlayfabManager>();
        }

        private void Start()
        {
            if (!_photonView.IsMine)
            {
                _camera.gameObject.SetActive(false);
            }

            SetCursor(enumCursorType.Movement);
        }

        private void Update()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            if (InteractWithUI())
            {
                _characterController.SimpleMove(Vector3.zero);
                return;
            }

            if (_healthController.IsDead())
            {
                //SetCursor(enumCursorType.None);
                return;
            }

            ProcessPlayerInput();

            if (InteractWithComponent())
            {
                // _characterController.SimpleMove(Vector3.zero);
                // return;
            }

            if (InteractWithMovement())
            {
                return;
            }
            else
            {
                _characterController.SimpleMove(Vector3.zero);
            }
        }

        private void ProcessPlayerInput()
        {
            if (Input.GetKey(KeyCode.Q) || Input.GetKeyDown(KeyCode.Q))
            {
                if (PhotonNetwork.IsConnected)
                {
                    _photonView.RPC(nameof(RPCInteractWithCombat), RpcTarget.All);
                }
                else
                {
                    RPCInteractWithCombat();
                }
            }
        }

        [PunRPC]
        private void RPCInteractWithCombat()
        {
            var currentWeapon = _combatController.GetCurrentWeapon();

            if (_combatController.TimeSinceLastAttack > _combatController.TimeBetweenAttacks)
            {
                if (currentWeapon.GetUseStamina())
                {
                    if (_staminaController.GetCurrentStaminaPoints() >= currentWeapon.GetStaminaPointsToSpend())
                    {
                        _staminaController.SpendStamina(currentWeapon.GetStaminaPointsToSpend());
                        ProcessCombat();
                    }
                }
                else
                {
                    ProcessCombat();
                }
            }
        }

        private void ProcessCombat()
        {
            var direction = GetDirection();
            _characterController.SimpleMove(Vector3.zero);
            transform.eulerAngles = SetRotation(direction);
            _combatController.SetDirection(direction);
            _combatController.AttackBehavior();
        }


        private bool InteractWithComponent()
        {
            var hits = RaycastAllSorted();
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<IRaycastable>(out var raycastable))
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        //SetCursor(raycastable.GetCursorType());
                        //return true;
                    }
                }
            }

            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            var hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                //SetCursor(enumCursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            var direction = GetDirection();

            if (Input.GetMouseButton(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;

                if (!Physics.Raycast(ray, out rayHit) ||
                    !rayHit.transform.TryGetComponent<PlayerController>(out var playerController))
                {
                    _actionScheduleController.StartAction(this);
                    _characterController.Move(direction * (Time.deltaTime * _speed));
                    transform.eulerAngles = SetRotation(direction);

                    return true;
                }

                _characterController.SimpleMove(Vector3.zero);

                return false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _characterController.SimpleMove(Vector3.zero);

                return false;
            }

            return false;
        }

        private Vector3 SetRotation(Vector3 direction)
        {
            var targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction),
                _rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
            return new Vector3(0, transform.eulerAngles.y, 0);
        }

        private Vector3 GetDirection()
        {
            var playerScreenPosition = _camera.WorldToScreenPoint(_characterController.transform.position);
            var cursorPosition = Input.mousePosition;
            var direction = cursorPosition - playerScreenPosition;
            (direction.z, direction.y) = (direction.y, direction.z);
            direction.Normalize();
            direction = Quaternion.Euler(0, _cameraYRotation, 0) * direction;
            return direction;
        }

        private void SetCursor(enumCursorType cursorType)
        {
            CursorMapping cursorMapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(cursorMapping.texture, cursorMapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(enumCursorType type)
        {
            foreach (var cursorMapping in _cursorMappings)
            {
                if (cursorMapping.Type == type)
                {
                    return cursorMapping;
                }
            }

            return _cursorMappings[0];
        }

        private Ray GetMouseRay()
        {
            return _playerCameraController.GetPlayerCamera().ScreenPointToRay(Input.mousePosition);
        }

        public void Cancel()
        {
            _characterController.SimpleMove(Vector3.zero);
        }

        public override void OnJoinedRoom()
        {
            // if (PhotonNetwork.IsMasterClient)
            // {
            //     return;
            // }

            photonView.RPC(nameof(RPCSyncPlayers), RpcTarget.Others);

            UpdatePlayersPanel();
        }

        private void UpdatePlayersPanel()
        {
            foreach (var playerController in FindObjectsOfType<PlayerController>())
            {
                if (!playerController.GetPhotonView().IsMine)
                {
                    playerController.GetHUD().SetActive(false);
                    _playersPanel.AddPlayer(playerController);
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            _playersPanel.PanelParent.SetActive(true);
        }

        [PunRPC]
        private void RPCSyncPlayers()
        {
            var players = FindObjectsOfType<PlayerController>();
 
            foreach (var playerController in players)
            {
                if (!playerController.GetPhotonView().IsMine)
                {
                    playerController.GetHUD().SetActive(false);
                    _playersPanel.AddPlayer(playerController);
                    //playerController.GetPlayersPanel().AddPlayer(this);
                    // _playersPanel.AddPlayer(playerController);
                    // var id = playerController.GetPhotonView().Controller.NickName;
                    // playerController.GetPlayfabPlayerDataController().SetupPlayer(id);
                    
                    //Destroy(playerController._hud);
                    // var id = playerController.GetPhotonView().Controller.NickName;
                    // playerController._playfabManager.LoadData(id, OnDataReceived);
                }
            }
        }

        private void OnDataReceived(GetUserDataResult getUserDataResult)
        {
            if (getUserDataResult == null) { return; }

            if (getUserDataResult.Data.ContainsKey("Experience"))
            {
                _experienceController.GainExperience(Convert.ToInt32(getUserDataResult.Data["Experience"].Value));
            }
            if (getUserDataResult.Data.ContainsKey("Currency"))
            {
                _playerCurrencyController.MyCurrency = Convert.ToInt32(getUserDataResult.Data["Currency"].Value);
            }
            if (getUserDataResult.Data.ContainsKey("Weapon"))
            {
                foreach (var weaponConfig in _availableWeapon)
                {
                    if (weaponConfig.name == getUserDataResult.Data["Weapon"].Value)
                    {
                        _combatController.EquipWeapon(weaponConfig);
                        break;
                    }
                }
            }
        }
    }
}