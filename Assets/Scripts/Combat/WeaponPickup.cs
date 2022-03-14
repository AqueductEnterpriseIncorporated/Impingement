using System;
using Impingement.Attributes;
using Impingement.Control;
using Impingement.enums;
using Impingement.Movement;
using UnityEngine;

namespace Impingement.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private float _healthToRestore = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            if (weaponConfig != null)
            {
                subject.GetComponent<CombatController>().EquipWeapon(weaponConfig);
            }

            if (_healthToRestore > 0)
            {
                subject.GetComponent<HealthController>().Heal(_healthToRestore);
            }
            Destroy(gameObject);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetMovementController().StartMoving(transform.position, 1f);
            }

            return true;
        }

        public enumCursorType GetCursorType()
        {
            return enumCursorType.Pickup;
        }
    }
}