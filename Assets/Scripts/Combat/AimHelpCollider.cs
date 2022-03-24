using Impingement.Attributes;
using UnityEngine;

namespace Impingement.Combat
{
    public class AimHelpCollider : MonoBehaviour
    {
        [SerializeField] private Projectile _projectile;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<HealthController>(out var target))
            {
                _projectile.Aim(target);
            }
        }
    }
}