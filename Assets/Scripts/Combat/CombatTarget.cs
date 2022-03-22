using Impingement.Control;
using Impingement.enums;
using Impingement.Attributes;
using UnityEngine;

namespace Impingement.Combat
{
    [RequireComponent(typeof(HealthController))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        [SerializeField] private HealthController _healthController;
        [SerializeField] private Transform _aimPoint;
        public bool HandleRaycast(PlayerController callingController)
        {
            //if (!callingController.GetCombatController().CanAttack(_healthController))
            {
                //return false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetCombatController().SetTarget(_healthController);
            }

            return true;
        }

        public enumCursorType GetCursorType()
        {
            return enumCursorType.Combat;
        }

        public Transform GetAimPoint()
        {
            return _aimPoint;
        }
    }
}