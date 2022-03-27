using System;
using Impingement.Core;
using Impingement.enums;
using Impingement.Attributes;
using Impingement.Combat;
using Impingement.structs;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Impingement.Control
{
    public class PlayerController : MonoBehaviour, IAction
    {
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
        private readonly int _cameraYRotation = 45;

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

        private void Start()
        {
            if (!_photonView.IsMine)
            {
                _camera.gameObject.SetActive(false);
            }
            PhotonNetwork.NickName = "player" + Random.Range(0,10);
            SetCursor(enumCursorType.Movement);
        }

        private void Update()
        {
            if (!_photonView.IsMine) { return; }

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
                InteractWithCombat();
            }
        }

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
            return _playerCameraController.
                GetPlayerCamera().ScreenPointToRay(Input.mousePosition);
        }

        public void Cancel()
        {
            _characterController.SimpleMove(Vector3.zero);
        }
    }
}