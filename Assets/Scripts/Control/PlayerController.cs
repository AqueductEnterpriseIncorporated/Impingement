using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using UnityEngine;

namespace Impingement.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        
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
                    if (Input.GetMouseButtonDown(0))
                    {
                        GetComponent<CombatController>().SetTarget(combatTarget);
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
                    GetComponent<MovementController>().StartMoving(hit.point);
                }
                return true;
            }
            return false;
        }

        private Ray GetMouseRay()
        {
            return GetComponent<PlayerCameraController>().
                GetPlayerCamera().ScreenPointToRay(Input.mousePosition);
        }
    }
}