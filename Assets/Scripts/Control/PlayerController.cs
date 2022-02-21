using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Control
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        private GameObject _spawnPoint;
        private CombatController _combatController;
        private MovementController _movementController;
        private PlayerCameraController _playerCameraController;
        private HealthController _healthController;
        
        private void Start()
        {
            _spawnPoint = GameObject.FindWithTag("SpawnPoint");
            if (_spawnPoint != null)
            {
                transform.position = _spawnPoint.transform.position;
            }
            _combatController = GetComponent<CombatController>();
            _movementController = GetComponent<MovementController>();
            _playerCameraController = GetComponent<PlayerCameraController>();
            _healthController = GetComponent<HealthController>();
        }
        
        private void Update()
        {
            if(!IsOwner) { return; }
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