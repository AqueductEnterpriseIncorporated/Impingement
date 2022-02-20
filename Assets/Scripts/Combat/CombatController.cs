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
        [SerializeField] private float _weaponDamage = 5f;
        [SerializeField] private float _rotateSpeed = 5f;
        private MovementController _movementController;
        private AnimationController _animationController;
        private HealthController _target;
        private float _timeSinceLastAttack = Mathf.Infinity;
        
        private void Start()
        {
            _movementController = GetComponent<MovementController>();
            _animationController = GetComponent<AnimationController>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null) { return; }

            if (_target.IsDead())
            {
                _target = null;
                return;
            }
            
            if (!GetIsInRange())
            {
                _movementController.Move(_target.transform.position, 1);
            }
            else
            {
                _movementController.Stop();
                LookAtTarget();
                AttackBehavior();
            }
        }

        //bug: client doesn't rotate
        private void LookAtTarget()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);
                SubmitLookAtTargetRequestClientRpc();
            }
            else
            {
                SubmitLookAtTargetRequestServerRpc();
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            return !combatTarget.GetComponent<HealthController>().IsDead() || combatTarget != null;
        }
        
        private void AttackBehavior()
        {
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                _animationController.ResetTriggerAnimation("cancelAttack");
                _animationController.PlayTriggerAnimation("attack");
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
            if (_target == null) { return; }

            _target.TakeDamage(_weaponDamage);
        }

        
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _weaponRange;
        }

        public void SetTarget(GameObject target)
        {
            GetComponent<ActionScheduleController>().StartAction(this);
            _target = target.GetComponent<HealthController>();
        }
        
        public HealthController GetTarget()
        {
            return _target;
        }

        private void RemoveTarget()
        {
            _target = null;
        }

        public void Cancel()
        {
            RemoveTarget();
            _animationController.ResetTriggerAnimation("attack");
            _animationController.PlayTriggerAnimation("cancelAttack");
        }

        //bug: bug
        #region Server
        [ServerRpc(RequireOwnership = false)]
        private void SubmitLookAtTargetRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            var lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);
            //transform.LookAt(_target.transform);
            SubmitLookAtTargetRequestClientRpc();
        }
        #endregion
        
        #region Client
        [ClientRpc]
        private void SubmitLookAtTargetRequestClientRpc(ServerRpcParams rpcParams = default)
        {
            var lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);    
        }
        #endregion
    }
}