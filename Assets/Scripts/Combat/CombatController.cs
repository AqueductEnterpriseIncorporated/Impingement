using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using Impingement.Core;
using Impingement.enums;
using Impingement.Movement;
using Impingement.Resources;
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
        [SerializeField] private Weapon _defaultWeapon = null;
        [SerializeField] private string _weaponName = "";
        private PhotonView _photonView;
        private MovementController _movementController;
        private HealthController _healthController;
        private AnimationController _animationController;
        private HealthController _target;
        private float _timeSinceLastAttack = Mathf.Infinity;
        private LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            _movementController = GetComponent<MovementController>();
            _healthController = GetComponent<HealthController>();
            _animationController = GetComponent<AnimationController>();
            _photonView = GetComponent<PhotonView>();
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(_defaultWeapon);
            return _defaultWeapon;
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
            _currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(_rightHandTransform, _leftHandTransform, animator);
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
            return !combatTarget.GetComponent<HealthController>().IsDead() && combatTarget != null;
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
            if (_currentWeapon.value.HasProjectile())
            {
                if(!_photonView.IsMine){ return; }
                _currentWeapon.value.LaunchProjectile(_leftHandTransform, _rightHandTransform, _target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.value.GetRange();
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
                yield return _currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(enumStats stat)
        {
            if (stat == enumStats.Damage)
            {
                yield return _currentWeapon.value.GetPercentageBonus();
            }
        }
    }
}