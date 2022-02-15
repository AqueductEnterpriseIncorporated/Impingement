using UnityEngine;

namespace Impingement.Combat
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField]
        private float _health = 100f;

        public void TakeDamage(float damage)
        {
            _health = Mathf.Max(_health - damage, 0);
            print(_health);
        }
    }
}