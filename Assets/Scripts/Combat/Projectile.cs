
using System;
using Impingement.Combat;
using UnityEngine;

namespace MyNamespace
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 1f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] private float  _maxLifeTime = 10f;
        [SerializeField] private float _lifeAfterImpact = 2f;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private GameObject _hitEffect = null;
        private HealthController _target = null;
        private float _damage = 0f;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        private void Update()
        {
            if(_target == null) { return; }

            if (_isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        public void SetTarget(HealthController target, float damage)
        {
            _target = target;
            _damage = damage;
            
            Destroy(gameObject, _maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsuleCollider = _target.GetComponent<CapsuleCollider>();
            if (targetCapsuleCollider == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * targetCapsuleCollider.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<HealthController>(out var healthController))
            {
                if (healthController != _target) { return; }
                if(_target.IsDead()) { return; }
                healthController.TakeDamage(_damage);

                _speed = 0;
                
                if (_hitEffect != null)
                {
                    Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
                }

                foreach (var toDestroy in _destroyOnHit)
                {
                    Destroy(toDestroy);
                }
                
                Destroy(gameObject, _lifeAfterImpact);
            }
        }
    }
}
