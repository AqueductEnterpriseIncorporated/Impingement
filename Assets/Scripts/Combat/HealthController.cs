using System;
using Impingement.Core;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Combat
{
    public class HealthController : MonoBehaviour, IPunObservable
    {
        [SerializeField] private float _healthPoints = 100f;
        [SerializeField] private bool _isDead;
        private PhotonView _photonView;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            
        }

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
                _photonView.RPC(nameof(DieRPC), RpcTarget.AllBufferedViaServer);

                //DieRPC();
            }
        }

        [PunRPC]
        private void DieRPC()
        {
            if (_isDead) { return; }

            AnimationController animationController = GetComponent<AnimationController>();
            animationController.PlayTriggerAnimation("die");
            //_photonView.RPC(nameof(animationController.PlayTriggerAnimation), RpcTarget.AllBufferedViaServer, "die");
            GetComponent<ActionScheduleController>().CancelCurrentAction();
            _photonView.RPC(nameof(SyncHealthState), RpcTarget.AllBufferedViaServer, true);
        }

        [PunRPC]
        private void SyncHealthState(bool value)
        {
            _isDead = value;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_isDead);
            }
            else
            {
                _isDead = (bool)stream.ReceiveNext();
            }
        }
    }
}