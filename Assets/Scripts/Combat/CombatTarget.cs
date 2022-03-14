using Impingement.Control;
using Impingement.enums;
using Impingement.Attributes;
using UnityEngine;

namespace Impingement.Combat
{
    [RequireComponent(typeof(HealthController))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.GetCombatController().CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetCombatController().SetTarget(gameObject);
            }

            return true;
        }

        public enumCursorType GetCursorType()
        {
            return enumCursorType.Combat;
        }
    }
}