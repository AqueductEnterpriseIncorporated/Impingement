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
        [SerializeField] private float  _maxLifeTime = 10f;
        [SerializeField] private float _lifeAfterImpact = 2f;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private PhotonView _photonView;
        private GameObject _instigator = null;
        private HealthController _target = null;
        private float _lifeTimeTimer = Mathf.Infinity;
        private float _damage = 0f;

        private void Start()
        {
            if(_target == null) { return; }
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

        public void SetTarget(HealthController target,GameObject instigator, float damage)
        {
            _target = target;
            _instigator = instigator;
            _damage = damage;

            _photonView.RPC(nameof(DestroyGameObjectRPC), RpcTarget.AllViaServer, _maxLifeTime);
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
                
                _speed = 0;
                
                _onHit.Invoke();
                
                if (_hitEffect != null)
                {
                    PhotonNetwork.Instantiate("VFX/" + _hitEffect.name, GetAimLocation(), transform.rotation);
                }

                _photonView.RPC(nameof(DestroyGameObjectsRPC), RpcTarget.AllViaServer);

                
                _photonView.RPC(nameof(DestroyGameObjectRPC), RpcTarget.AllViaServer, _lifeAfterImpact);
                
                healthController.TakeDamage(_instigator, _damage);
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
