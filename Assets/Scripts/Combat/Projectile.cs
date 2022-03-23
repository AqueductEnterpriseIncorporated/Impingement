using System.Collections;
using Impingement.Attributes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Impingement.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onHit;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] private bool _isExplosive = false;
        [SerializeField] private float  _maxLifeTime = 10f;
        [SerializeField] private float _lifeAfterImpact = 2f;
        [SerializeField] private float _boundsForce = 50f;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private Collider _currentCollider;

        private bool _processTranslate = true;
        private GameObject _instigator = null;
        private HealthController _target = null;
        private Vector3 _direction;
        private float _lifeTimeTimer = Mathf.Infinity;
        private float _damage = 0f;

        private void Start()
        {
            //if(_target == null || _direction == default) { return; }
            if (_target == null)
            {
                var targetRotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_direction),
                    100f * Time.deltaTime);
                transform.rotation = targetRotation;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            else
            {
                transform.LookAt(GetAimLocation());
            }
        }

        private void Update()
        {
            //if(_target == null || _direction == default) { return; }
            if(!_processTranslate) { return; }
            if (_isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        public void SetDirection(Vector3 direction, GameObject instigator, float damage)
        {
            _direction = direction;
            _direction.y = 0;
            _instigator = instigator;
            _damage = damage;
            SelfDestroy();
        }

        public void SetTarget(HealthController target, GameObject instigator, float damage)
        {
            _target = target;
            _instigator = instigator;
            _damage = damage;

            SelfDestroy();
        }

        private void SelfDestroy()
        {
            if (PhotonNetwork.InRoom)
            {
                _photonView.RPC(nameof(DestroyGameObjectRPC), RpcTarget.AllViaServer, _maxLifeTime);
            }
            else
            {
                DestroyGameObjectRPC(_maxLifeTime);
            }
        }

        private Vector3 GetAimLocation()
        {
            if (_target == null)
            {
                return _direction;
            }

            return _target.GetCombatTarget().GetAimPoint().position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other == _collider || other == _currentCollider) { return; }
            
            if (other.TryGetComponent<HealthController>(out var healthController))
            {
                if(_instigator == healthController.gameObject) { return; }

                if (_target != null && _target.IsPlayer)
                {
                    if (healthController != _target) { return; }
                }
                //if(_target.IsDead()) { return; }
                
                _rigidbody.constraints = RigidbodyConstraints.FreezePosition;
                _speed = 0;
                
                _onHit.Invoke();
                
                SpawnVFX();

                DestroyObjects();

                healthController.TakeDamage(_instigator, _damage);
            }
            else if(other.gameObject != _instigator)
            {
                if (_isExplosive)
                {
                    SpawnVFX();
                    DestroyObjects();
                    return;
                }

                _rigidbody.isKinematic = false;
                
                _rigidbody.freezeRotation = false;
                _currentCollider.enabled = false;
                _collider.enabled = true;
                _processTranslate = false;
                _rigidbody.useGravity = true;
                _rigidbody.AddForce(Vector3.forward * _boundsForce);
            }
        }

        private void SpawnVFX()
        {
            if (_hitEffect != null)
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.Instantiate("VFX/" + _hitEffect.name, GetAimLocation(), transform.rotation);
                }
                else
                {
                    Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
                }
            }
        }

        private void DestroyObjects()
        {
            if (PhotonNetwork.InRoom)
            {
                _photonView.RPC(nameof(DestroyGameObjectsRPC), RpcTarget.AllViaServer);
                _photonView.RPC(nameof(DestroyGameObjectRPC), RpcTarget.AllViaServer, _lifeAfterImpact);
            }
            else
            {
                _processTranslate = false;
                DestroyGameObjectsRPC();
                DestroyGameObjectRPC(_lifeAfterImpact);
            }
        }

        [PunRPC]
        private void DestroyGameObjectRPC(float lifeTime)
        {
            StartCoroutine(WaitForSeconds(lifeTime));
        }
        
        [PunRPC]
        private void DestroyGameObjectsRPC()
        {
            foreach (var toDestroy in _destroyOnHit)
            {
                Destroy(toDestroy);
            }
        }

        private IEnumerator WaitForSeconds(float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject, lifeTime);
        }
    }
}
