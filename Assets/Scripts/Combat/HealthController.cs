using Impingement.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Impingement.Combat
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField]
        private float _healthPoints = 100f;

        private bool _isDead;

        public bool IsDead()
        {
            return _isDead;
        }

        //public event Death OnDeath;

        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            if (_healthPoints == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if(_isDead) { return; }
            //OnDeath?.Invoke(this, transform);
            GetComponent<AnimationController>().PlayTriggerAnimation("die");
            _isDead = true;
            //GetComponent<CapsuleCollider>().enabled = false;
            //GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    //public delegate void Death(object sender, Transform targetTransform);
}