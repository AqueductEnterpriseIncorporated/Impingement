using Impingement.Core;
using Impingement.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Combat
{
    public class CombatController : NetworkBehaviour, IAction
    {
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _rotateSpeed = 5f;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private Weapon _defaultWeapon = null;
        [SerializeField] private string _weaponName = "";
        private MovementController _movementController;
        private AnimationController _animationController;
        private HealthController _target;
        private float _timeSinceLastAttack = Mathf.Infinity;
        private Weapon _currentWeapon = null;
        
        private void Start()
        {
            _movementController = GetComponent<MovementController>();
            _animationController = GetComponent<AnimationController>();
            //Weapon weapon = Resources.Load<Weapon>(_weaponName);
            EquipWeapon(_defaultWeapon);
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
        
        public void EquipWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransform, _leftHandTransform, animator);
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
        
        /// <summary>
        /// Animation event
        /// </summary>
        private void Shoot()
        {
            Hit();
        }

        private void DealDamage()
        {
            if (_target == null) { return; }

            if (_currentWeapon.HasProjectile())
            {
                _currentWeapon.LaunchProjectile(_leftHandTransform, _rightHandTransform, _target);
            }
            else
            {
                _target.TakeDamage(_currentWeapon.GetDamage());
            }
        }

        
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.GetRange();
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