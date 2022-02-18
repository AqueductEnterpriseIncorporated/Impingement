using System;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using UnityEngine;

namespace Impingement.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;

        private CombatController _combatController;
        private MovementController _movementController;
        private PlayerCameraController _playerCameraController;

        private void Start()
        {
            _combatController = GetComponent<CombatController>();
            _movementController = GetComponent<MovementController>();
            _playerCameraController = GetComponent<PlayerCameraController>();
        }

        private void Update()
        {
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
                    if(!_combatController.CanAttack(combatTarget)) { continue; }
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        _combatController.SetTarget(combatTarget);
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
                    _movementController.StartMoving(hit.point);
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