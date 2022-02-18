using Impingement.Core;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Combat
{
    public class HealthController : NetworkBehaviour
    {
        [SerializeField]
        private float _healthPoints = 100f;

        private NetworkVariable<bool> _isDead = new NetworkVariable<bool>();
        //private bool _isDead;
        

        public bool IsDead()
        {
            return _isDead.Value;
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
            if(_isDead.Value) { return; }
            //OnDeath?.Invoke(this, transform);
            GetComponent<AnimationController>().PlayTriggerAnimation("die");
            ChangeIsDeadValueServerRpc(true);
            GetComponent<ActionScheduleController>().CancelCurrentAction();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeIsDeadValueServerRpc(bool value)
        {
            _isDead.Value = value;
        }
    }

    //public delegate void Death(object sender, Transform targetTransform);
}