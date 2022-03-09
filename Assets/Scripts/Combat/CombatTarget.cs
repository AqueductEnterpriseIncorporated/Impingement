using Impingement.Control;
using Impingement.enums;
using Impingement.Resources;
using UnityEngine;

namespace Impingement.Combat
{
    [RequireComponent(typeof(HealthController))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetComponent<CombatController>().CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<CombatController>().SetTarget(gameObject);
            }

            return true;
        }

        public enumCursorType GetCursorType()
        {
            return enumCursorType.Combat;
        }
    }
}