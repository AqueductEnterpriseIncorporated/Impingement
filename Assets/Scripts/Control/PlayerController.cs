using System;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using Impingement.Resources;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Impingement.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Camera _camera;
        private PhotonView _photonView;
        private CombatController _combatController;
        private MovementController _movementController;
        private PlayerCameraController _playerCameraController;
        private HealthController _healthController;

        private void Awake()
        {
            _combatController = GetComponent<CombatController>();
            _movementController = GetComponent<MovementController>();
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
            if(!_photonView.IsMine) { return;}
            if (_healthController.IsDead()) { return; }
            if (ProcessCombat()) { return; }
            if (ProcessMovement()) { return; }        
        }

        private bool ProcessCombat()
        {
            var hits = Physics.RaycastAll(GetMouseRay());
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<CombatTarget>(out var combatTarget))
                {
                    if(!_combatController.CanAttack(combatTarget.gameObject)) { continue; }
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        _combatController.SetTarget(combatTarget.gameObject);
                    }
                    return true;
                }

            }
            return false;
        }

        private bool ProcessMovement()
        {
            if (Physics.Raycast(GetMouseRay(), out var hit, Mathf.Infinity, _layerMask))
            {
                if (Input.GetMouseButton(0))
                { 
                    _movementController.StartMoving(hit.point, 1);
                }
                return true;
            }
            return false;
        }

        private Ray GetMouseRay()
        {
            return _playerCameraController.
                GetPlayerCamera().ScreenPointToRay(Input.mousePosition);
        }
    }
}