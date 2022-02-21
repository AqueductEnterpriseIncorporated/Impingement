using System;
using UnityEngine;

namespace Impingement.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon = null;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CombatController>().EquipWeapon(_weapon);
                Destroy(gameObject);
            }
        }
    }
}