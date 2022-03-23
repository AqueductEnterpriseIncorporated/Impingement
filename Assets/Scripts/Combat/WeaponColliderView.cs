using UnityEngine;

namespace Impingement.Combat
{
    public class WeaponColliderView : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Projectile _projectile;

        private void OnTriggerEnter(Collider other)
        {
            print(other.name);
            if (_weapon != null)
            {
                _weapon.DetectCollider(other);
            }
            else
            {
                //_projectile.DetectCollider(other);
            }
        }
    }
}