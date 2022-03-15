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
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _raycastRadius = 1f;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _speed = 3;
        [SerializeField] private Camera _camera;
        [SerializeField] private CursorMapping[] _cursorMappings;
        [SerializeField] private CombatController _combatController;
        [SerializeField] CharacterController _characterController;
        private PhotonView _photonView;
        private PlayerCameraController _playerCameraController;
        private HealthController _healthController;
        
        public CombatController GetCombatController()
        {
            return _combatController;
        }
        
        public HealthController GetHealthController()
        {
            return _healthController;
        }

        private void Awake()
        {
            _playerCameraController = GetComponent<PlayerCameraController>();
            _healthController = GetComponent<HealthController>();
            _photonView = GetComponent<PhotonView>();
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

            //if (InteractWithComponent()) { return; }
            InteractWithComponent();

            if (InteractWithMovement()) { return; }    


            // if (Input.GetMouseButtonDown(0))
            // {
            //     // var aimDistance = 10.0;
            //     // RaycastHit rch;
            //     // Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
            //     //
            //     // if ( Physics.Raycast( ray, rch, aimDistance ) ) {
            //     //     aimpoint = rch.point;
            //     // } else {
            //     //     aimpoint = ray.origin + ray.direction * aimDistance;
            //     // }
            //     // RaycastHit hit;
            //     // if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit, 100))
            //     // {
            //     //     // When click on the terreno, record the position.
            //     //     destination = hit.point;
            //     // }
            //     Vector3 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            //     destination = worldPosition;
            // }

            //if (InteractWithMovement()) { return; }    

            //SetCursor(enumCursorType.None);
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
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            var hits = Physics.SphereCastAll(GetMouseRay(), _raycastRadius);
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


        // private void Move()

        // {

        //     var moveVector = _playerModel.Transform.right * _horizontal + _playerModel.Transform.forward * _vertical;

        //     moveVector = Vector3.ClampMagnitude(moveVector, MAX_VECTOR_LENGTH);

        //     var speed = _playerModel.IsCrouching ? _crouchSpeed : _moveSpeed;

        //

        //     _characterController.Move(moveVector * (speed * _deltaTime));

        // }


        private bool InteractWithMovement()
        {
            var playerScreenPosition = _camera.WorldToScreenPoint(_characterController.transform.position);
            var cursorPosition = Input.mousePosition;

            var direction = cursorPosition - playerScreenPosition;
            (direction.z, direction.y) = (direction.y, direction.z);
            direction.Normalize();
            direction = Quaternion.Euler(0, 45, 0) * direction;

            // if (_characterController.velocity.magnitude > 0)
            // {
            //     var rotation = Quaternion.LookRotation(direction + transform.position);
            //     transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);
            // }

            if (Input.GetMouseButton(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                
                if (!Physics.Raycast(ray, out rayHit) ||
                    !rayHit.transform.TryGetComponent<PlayerController>(out var playerController))
                {
                    _characterController.Move(direction * (Time.deltaTime * _speed));
                    var targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), _rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    
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


            // bool hasHit = RaycastNavMesh(out var target);
            // Physics.Raycast(GetMouseRay(), out var hit, Mathf.Infinity, _layerMask);
            // if (!_movementController.CanMoveTo(target)) { return false; }
            //
            // if (hasHit)
            // {
            //     if (Input.GetMouseButton(0))
            //     { 
            //         _movementController.StartMoving(hit.point, 1);
            //     }
            //     SetCursor(enumCursorType.Movement);
            //     return true;
            // }
            // return false;
        }
        
        // private Vector3 ToEuler(Quaternion quaternion) {
        //     Vector4 q = new Vector4(quaternion.x, quaternion.y, quaternion.z);
        //     double3 res;
        //
        //     double sinr_cosp = +2.0 * (q.w * q.x + q.y * q.z);
        //     double cosr_cosp = +1.0 - 2.0 * (q.x * q.x + q.y * q.y);
        //     res.x = math.atan2(sinr_cosp, cosr_cosp);
        //
        //     double sinp = +2.0 * (q.w * q.y - q.z * q.x);
        //     if (math.abs(sinp) >= 1) {
        //         res.y = math.PI / 2 * math.sign(sinp);
        //     } else {
        //         res.y = math.asin(sinp);
        //     }
        //
        //     double siny_cosp = +2.0 * (q.w * q.z + q.x * q.y);
        //     double cosy_cosp = +1.0 - 2.0 * (q.y * q.y + q.z * q.z);
        //     res.z = math.atan2(siny_cosp, cosy_cosp);
        //
        //     return (float3) res;
        // }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out var hit);
            if (!hasHit) { return false; }

            var hasCastToNavMesh = UnityEngine.AI.NavMesh.SamplePosition(hit.point, out var navMeshHit,
                Mathf.Infinity,
                UnityEngine.AI.NavMesh.AllAreas);
            if (!hasCastToNavMesh) { return false;}
            target = navMeshHit.position;
            
            return true;
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
    }
}