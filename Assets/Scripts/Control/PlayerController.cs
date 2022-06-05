using System;
using System.Collections.Generic;
using Impingement.Core;
using Impingement.enums;
using Impingement.Attributes;
using Impingement.Combat;
using Impingement.Inventory;
using Impingement.Playfab;
using Impingement.Stats;
using Impingement.structs;
using Impingement.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Impingement.Control
{
    public class PlayerController : MonoBehaviourPunCallbacks, IAction
    {
        #region fields


        public AudioSource AudioSourceOnUse;
        [SerializeField] private GameObject _hud;
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
        [SerializeField] private AnimationController _animationController;
        [SerializeField] private StaminaController _staminaController;
        [SerializeField] private PlayfabPlayerDataController _playfabPlayerDataController;
        [SerializeField] private InventoryController _inventoryController;
        [SerializeField] private ItemDropper _itemDropper;
        [SerializeField] private ActionStore _actionStore;
        [SerializeField] private InputManager _inputManager;
        private readonly int _cameraYRotation = 45;

        #endregion

        #region getters

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

        public PhotonView GetPhotonView()
        {
            return _photonView;
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

        #endregion

        private void Start()
        {
            //_healthController.CharacterName = PhotonNetwork.NickName;
            SetCursor(enumCursorType.Movement);
        }

        private void Update()
        {
            
            if (!_photonView.IsMine && SceneManager.GetActiveScene().buildIndex != 2)
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

            if (!_animationController.IsPlaying())
            {
                if (ProcessMovement())
                {
                    return;
                }
                else
                {
                    _characterController.SimpleMove(Vector3.zero);
                }
            }
        }

        private void CheckSpecialAbilityKeys()
        {
            if (_inputManager.GetKeyDown("Активная1"))
            {
                _actionStore.Use(0, this);
            }

            if (_inputManager.GetKeyDown("Активная2"))
            {
                _actionStore.Use(1, this);
            }

            if (_inputManager.GetKeyDown("Активная3"))
            {
                _actionStore.Use(2, this);
            }

            if (_inputManager.GetKeyDown("Активная4"))
            {
                _actionStore.Use(3, this);
            }

            if (_inputManager.GetKeyDown("Активная5"))
            {
                _actionStore.Use(4, this);
            }

            if (_inputManager.GetKeyDown("Активная6"))
            {
                _actionStore.Use(5, this);
            }

            if (_inputManager.GetKeyDown("Активная7"))
            {
                _actionStore.Use(6, this);
            }

            if (_inputManager.GetKeyDown("Активная8"))
            {
                _actionStore.Use(7, this);
            }

            if (_inputManager.GetKeyDown("Активная9"))
            {
                _actionStore.Use(8, this);
            }
        }

        private void ProcessPlayerInput()
        {
            CheckSpecialAbilityKeys();

            if (_inputManager.GetKey("Атака") || _inputManager.GetKeyDown("Атака"))
            {
                InteractWithCombat();
            }
        }
        

        //todo: refactoring
        private void InteractWithCombat()
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

        private bool ProcessMovement()
        {
            var direction = GetDirection();

            if (_inputManager.GetKey("Передвижение"))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;

                // if (!Physics.Raycast(ray, out rayHit) ||
                //     !rayHit.transform.TryGetComponent<PlayerController>(out var playerController))
                {
                    _actionScheduleController.StartAction(this);
                    _characterController.Move(direction * (Time.deltaTime * _speed));
                    transform.eulerAngles = SetRotation(direction);

                    return true;
                }

                _characterController.SimpleMove(Vector3.zero);

                return false;
            }

            if (_inputManager.GetKeyUp("Передвижение"))
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
    }
}