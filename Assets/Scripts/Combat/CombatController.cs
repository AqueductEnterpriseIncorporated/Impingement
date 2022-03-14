using System.Collections.Generic;
using GameDevTV.Utils;
using Impingement.Attributes;
using Impingement.Core;
using Impingement.enums;
using Impingement.Movement;
using Impingement.Stats;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Combat
{
    public class CombatController : MonoBehaviour, IAction, IModifierProvider
    {
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _rotateSpeed = 5f;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeaponConfig = null;
        private PhotonView _photonView;
        private MovementController _movementController;
        private HealthController _healthController;
        private AnimationController _animationController;
        private HealthController _target;
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;
        private ActionScheduleController _actionScheduleController;
        private float _timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _actionScheduleController = GetComponent<ActionScheduleController>();
            _movementController = GetComponent<MovementController>();
            _healthController = GetComponent<HealthController>();
            _animationController = GetComponent<AnimationController>();
            _photonView = GetComponent<PhotonView>();
            _currentWeaponConfig = defaultWeaponConfig;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }
        
        private void Update()
        {
            if(_healthController.IsDead()) { return; }
            _timeSinceLastAttack += Time.deltaTime;
            if(_target == null) { return; }

            if (_target.IsDead())
            {
                _target = null;
                return;
            }
            
            if (!GetIsInRange(_target.transform))
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

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeaponConfig = weaponConfig;
            _currentWeapon.value = AttachWeapon(weaponConfig);
        }

        private Weapon AttachWeapon(WeaponConfig weaponConfig)
        {
            Animator animator = GetComponent<Animator>();
            return weaponConfig.Spawn(_rightHandTransform, _leftHandTransform, animator);
        }

        private void LookAtTarget()
        {
            //transform.LookAt(_target.transform);
            var lookRotation = Quaternion.LookRotation(_target.transform.position - transform.position);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, lookRotation, _rotateSpeed * Time.deltaTime);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }

            if (!_movementController.CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform))
            {
                return false;
            }

            return !combatTarget.GetComponent<HealthController>().IsDead();
        }

        private void AttackBehavior()
        {
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                _animationController.ResetTriggerAnimation("cancelAttack");
                _animationController.PlayTriggerAnimation("attack");
                //_photonView.RPC(nameof(_animationController.ResetTriggerAnimation), RpcTarget.All, "cancelAttack");
                //_photonView.RPC(nameof(_animationController.PlayTriggerAnimation), RpcTarget.All, "attack");
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

            var damage = GetComponent<BaseStats>().GetStat(enumStats.Damage);

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }
            
            if (_currentWeaponConfig.HasProjectile())
            {
                if(!_photonView.IsMine){ return; }
                _currentWeaponConfig.LaunchProjectile(_leftHandTransform, _rightHandTransform, _target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetRange();
        }
        
        public void SetTarget(GameObject target)
        {
            _actionScheduleController.StartAction(this);
            _target = target.GetComponent<HealthController>();
        }
        
        public HealthController GetTarget()
        {
            return _target;
        }

        public void RemoveTarget()
        {
            _target = null;
        }

        public void Cancel()
        {
            RemoveTarget();
            
            _animationController.ResetTriggerAnimation("attack");
            _animationController.PlayTriggerAnimation("cancelAttack");
            //_photonView.RPC(nameof(_animationController.ResetTriggerAnimation), RpcTarget.All, "cancelAttack");
           // _photonView.RPC(nameof(_animationController.PlayTriggerAnimation), RpcTarget.All, "attack");
        }
        
        public IEnumerable<float> GetAdditiveModifiers(enumStats stat)
        {
            if (stat == enumStats.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(enumStats stat)
        {
            if (stat == enumStats.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}