using System;
using Impingement.Core;
using Impingement.enums;
using Impingement.Movement;
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
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Camera _camera;
        [SerializeField] private CursorMapping[] _cursorMappings;
        [SerializeField] private float _raycastRadius = 1f;
        [SerializeField] private CombatController _combatController;
        [SerializeField] private MovementController _movementController;
        private PhotonView _photonView;
        private PlayerCameraController _playerCameraController;
        private HealthController _healthController;

        public CombatController GetCombatController()
        {
            return _combatController;
        }
        
        public MovementController GetMovementController()
        {
            return _movementController;
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
        }
        
        private void Update()
        {
            if (!_photonView.IsMine) { return; }

            if(InteractWithUI()) { return; }

            if (_healthController.IsDead())
            {
                SetCursor(enumCursorType.None);
                return;
            }

            if (InteractWithComponent()) { return; }
            if (InteractWithMovement()) { return; }    
            
            SetCursor(enumCursorType.None);
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
                        SetCursor(raycastable.GetCursorType());
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
                SetCursor(enumCursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            bool hasHit = RaycastNavMesh(out var target);
            Physics.Raycast(GetMouseRay(), out var hit, Mathf.Infinity, _layerMask);
            if (!_movementController.CanMoveTo(target)) { return false; }
            
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                { 
                    _movementController.StartMoving(hit.point, 1);
                }
                SetCursor(enumCursorType.Movement);
                return true;
            }
            return false;
        }

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