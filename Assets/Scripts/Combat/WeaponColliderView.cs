using UnityEngine;

namespace Impingement.Combat
{
    public class WeaponColliderView : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;

        private void OnTriggerEnter(Collider other)
        {
            _weapon.DetectCollider(other);
        }
    }
}