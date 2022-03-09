using System;
using Impingement.Control;
using Impingement.enums;
using UnityEngine;

namespace Impingement.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Weapon _weapon = null;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Pickup(other.GetComponent<CombatController>());
            }
        }

        private void Pickup(CombatController combatController)
        {
            combatController.EquipWeapon(_weapon);  
            Destroy(gameObject);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.GetComponent<CombatController>());
            }

            return true;
        }

        public enumCursorType GetCursorType()
        {
            return enumCursorType.Pickup;
        }
    }
}