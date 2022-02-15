using Impingement.Core;
using Impingement.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Combat
{
    public class CombatController : NetworkBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        private MovementController _movementController;
        private Transform _targetTransform;
        private float _timeSinceLastAttack;
        private float _weaponDamage = 5f;
        private void Start()
        {
            _movementController = GetComponent<MovementController>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_targetTransform == null) { return; }
            if (!GetIsInRange())
            {
                _movementController.Move(_targetTransform.position);
            }
            else
            {
                _movementController.Stop();
                AttackBehavior();
            }
        }

        private void AttackBehavior()
        {
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                GetComponent<PlayerAnimationController>().PlayAttackAnimation();
                _timeSinceLastAttack = 0;
            }
        }
        
        /// <summary>
        /// Animation event
        /// </summary>
        private void Hit()
        {
            DealDamage();
        }

        private void DealDamage()
        {
            if (_targetTransform == null) { return; }

            _targetTransform.GetComponent<HealthController>().TakeDamage(_weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _targetTransform.position) < _weaponRange;
        }

        public void SetTarget(CombatTarget target)
        {
            GetComponent<ActionScheduleController>().StartAction(this);
            _targetTransform = target.transform;
        }

        private void RemoveTarget()
        {
            _targetTransform = null;
        }

        public void Cancel()
        {
            RemoveTarget();
        }
    }
}